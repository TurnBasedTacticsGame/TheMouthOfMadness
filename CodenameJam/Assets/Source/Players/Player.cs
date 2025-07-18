using System;
using System.Collections.Generic;
using Cinemachine;
using Source.GameEvents.Core;
using UniDi;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace Source.Players
{
    public class Player : MonoBehaviour
    {
        public event Action OnPlayerDeath; 

        [Header("Dependencies")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private CameraShaker cameraShaker;
        [SerializeField] private RandomAudioPlayer footstepsRandomAudioPlayer;
        [SerializeField] private AudioClipData footstepAudioData;
        [SerializeField] private Animator animator;
        [SerializeField] private Light2DScaler radialLight2D;
        [SerializeField] private Light2DScaler flashlightLight2D;

        [Header("Configuration")] 
        [SerializeField] private float currentHealth = 5;
        [SerializeField] private float maxHealth = 5;
        [SerializeField] private float damagedCooldown = 1f;
        [SerializeField] private float maxTimeSpentMoving = 8f;
        [SerializeField] public float moveCooldown = 0.25f;

        [Inject] private EventTracker tracker;
        [Inject] private Camera mainCamera;

        public PlayerState State { get; private set; }= PlayerState.Waiting;

        public TargeterUi targetPosition;
        public TargeterUi targetFlashlightPosition;
        public TargeterUi targetFlashlightDirectionArrow;
        public Transform flashlight;

        public float movementSpeed = 5;
        public float rotationSpeed = 90;

        private List<Vector3> waypoints = new();
        private NavMeshPath path;
        private bool foundPath;
        private Vector3 activeDirection;

        public float TimeSpentWaiting { get; private set; } = float.PositiveInfinity;
        public float TimeSpentMoving { get; private set; }
        
        private bool canTakeDamage = true;
        private float damageTimer;
        
        [Header("UI")]
        [SerializeField] private float flashlightDirectionArrowDistance = 1f;
        [SerializeField] private float arrowRotationSpeed = 0.1f;
        [SerializeField] private float arrowPositionSpeed = 0.1f;
        [SerializeField] private float rejectedPathShakeIntensity;
        [SerializeField] private float rejectedPathShakeDuration;
        
        private Vector3 arrowPositionVelocity;
        private Quaternion arrowRotationVelocity;
        private static readonly int Moving = Animator.StringToHash("Moving");

        private void Start()
        {
            path = new NavMeshPath();
            TryFindPath(targetPosition.transform.position);
        }

        private void Update()
        {
            switch (State)
            {
                case PlayerState.Waiting:
                {
                    animator.SetBool(Moving, false);
                    
                    TimeSpentWaiting += Time.deltaTime;
                    TimeSpentMoving = 0;

                    if (Time.timeScale > 0.1f)
                    {
                        break;
                    }

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        var mousePosition = GetMouseWorldPosition();
                        if (TryFindPath(mousePosition))
                        {
                            targetPosition.transform.position = waypoints[waypoints.Count - 1];
                            foundPath = true;
                        }
                        else
                        {
                            cameraShaker.Shake(rejectedPathShakeIntensity, rejectedPathShakeDuration);
                        }
                    }

                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        var mousePosition = GetMouseWorldPosition();
                        var front = (mousePosition - targetPosition.transform.position);
                        var frontXY = new Vector3(front.x, front.y, 0);
                        activeDirection = frontXY.normalized;
                        
                        // If mouse is too close, it causes undefined flashlight behavior.
                        if (frontXY.magnitude > 0.2f)
                        {
                            activeDirection = frontXY.normalized;
                            targetFlashlightPosition.transform.position = mousePosition;
                        } else {
                            activeDirection = Vector3.up;
                            targetFlashlightPosition.transform.position = targetPosition.transform.position + Vector3.up;
                        }
                    }
                    
                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        targetPosition.IsTargeting(false);
                        targetFlashlightDirectionArrow.IsTargeting(false);
                        foundPath = false;
                    }

                    if (foundPath)
                    {
                        var frontPosition = targetPosition.transform.position + (activeDirection * flashlightDirectionArrowDistance);
                        var angle = (Mathf.Atan2(activeDirection.y, activeDirection.x) * Mathf.Rad2Deg) - 90f;
                        targetFlashlightDirectionArrow.transform.SetPositionAndRotation(
                            Vector3.Lerp(targetFlashlightDirectionArrow.transform.position, frontPosition, arrowPositionSpeed * Time.unscaledDeltaTime), 
                            Quaternion.Lerp(targetFlashlightDirectionArrow.transform.rotation, Quaternion.Euler(new Vector3(0,0, angle)), arrowRotationSpeed * Time.unscaledDeltaTime)
                        );

                        targetPosition.IsTargeting(true);
                        targetFlashlightDirectionArrow.IsTargeting(true);
                    }

                    if (Input.GetKeyDown(KeyCode.Return) && foundPath)
                    {
                        State = PlayerState.Moving;
                        
                        targetPosition.IsTargeting(false);
                        targetFlashlightDirectionArrow.IsTargeting(false);
                        foundPath = false;
                    }
                    
                    break;
                }
                case PlayerState.Moving:
                {
                    animator.SetBool(Moving, true);
                    
                    TimeSpentMoving += Time.deltaTime;
                    TimeSpentWaiting = 0;

                    footstepsRandomAudioPlayer.StartPlayingRandom(footstepAudioData);

                    if (TimeSpentMoving > maxTimeSpentMoving)
                    {
                        State = PlayerState.Waiting;
                        footstepsRandomAudioPlayer.StopPlayingRandom();
                    }

                    break;
                }
            }

            UpdateMovement();
            UpdateDamage();
            UpdateHealth();
        }

        private void UpdateMovement()
        {
            var targetFlashlightRotation = Quaternion.Euler(0, 0, GetAngleDegrees(targetFlashlightPosition.transform.position - transform.position));
            var hasReachedTargetPosition = waypoints.Count == 0;
            var hasReachedTargetRotation = Quaternion.Angle(targetFlashlightRotation, flashlight.rotation) < 0.1f;

            if (State == PlayerState.Moving && hasReachedTargetPosition && hasReachedTargetRotation)
            {
                State = PlayerState.Waiting;

                return;
            }

            if (waypoints.Count > 0)
            {
                var direction = waypoints[0] - transform.position;
                direction = waypoints.Count == 1 ? Vector3.ClampMagnitude(direction, 1) : direction.normalized;

                rb.velocity = Vector3.ClampMagnitude(direction, 1) * movementSpeed;

                var hasReachedCurrentWaypoint = (waypoints[0] - transform.position).sqrMagnitude < 0.1f;
                if (hasReachedCurrentWaypoint)
                {
                    waypoints.RemoveAt(0);
                }
            }

            flashlight.rotation = Quaternion.RotateTowards(flashlight.rotation, targetFlashlightRotation, rotationSpeed * Time.deltaTime);
        }

        private bool TryFindPath(Vector3 destination)
        {
            var filter = new NavMeshQueryFilter();
            filter.areaMask |= 1 << 0;

            NavMesh.CalculatePath(transform.position, destination, filter, path);
            if (path.status != NavMeshPathStatus.PathInvalid)
            {
                waypoints.Clear();
                for (var i = 0; i < path.corners.Length; i++)
                {
                    waypoints.Add(path.corners[i]);
                }

                return true;
            }

            return false;
        }

        private float GetAngleDegrees(Vector3 offset)
        {
            return Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        }

        private Vector3 GetMouseWorldPosition()
        {
            var plane = new Plane(Vector3.forward, transform.position);
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out var distance))
            {
                return ray.GetPoint(distance);
            }

            return transform.position + Vector3.up;
        }
        
        // In real game, should be inverted
        public void TryTakeDamage(float damage)
        {
            if (!canTakeDamage) 
                return;
            
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            damageTimer = damagedCooldown;
            canTakeDamage = false;
        }

        public void Respawn()
        {
            currentHealth = maxHealth;
        }

        private void UpdateHealth()
        {
            var healthPercentage = currentHealth / maxHealth;
            
            radialLight2D.SetPercentage(healthPercentage);
            flashlightLight2D.SetPercentage(healthPercentage);

            if (currentHealth <= 0)
            {
                radialLight2D.SetPercentage(0);
                flashlightLight2D.SetPercentage(0);
                OnPlayerDeath?.Invoke();
            }
        }

        private void UpdateDamage()
        {
            // Should only tick when unpaused. 
            damageTimer -= Time.deltaTime;

            if (damageTimer <= 0)
            {
                canTakeDamage = true;
            }
        }

        public enum PlayerState
        {
            Waiting,
            Moving,
        }
    }
}
