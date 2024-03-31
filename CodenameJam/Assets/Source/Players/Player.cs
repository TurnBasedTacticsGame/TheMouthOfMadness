using System.Collections.Generic;
using Source.GameEvents.Core;
using UniDi;
using UnityEngine;
using UnityEngine.AI;

namespace Source.Players
{
    public class Player : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Rigidbody2D rb;

        [Header("Configuration")]
        [SerializeField] private float currentHealth = 5;
        [SerializeField] private float maxHealth = 5;
        [SerializeField] private float maxTimeSpentMoving = 8f;
        [SerializeField] public float moveCooldown = 0.25f;
        [SerializeField] public float flashlightDirectionArrowDistance = 1f;

        [Inject] private EventTracker tracker;
        [Inject] private Camera mainCamera;

        public PlayerState state = PlayerState.Moving;

        public Transform targetPosition;
        public Transform targetFlashlightPosition;
        public Transform targetFlashlightDirectionArrow;
        public Transform flashlight;

        public float movementSpeed = 5;
        public float rotationSpeed = 90;

        private List<Vector3> waypoints = new();
        private NavMeshPath path;
        private bool foundPath;

        public float timeSpentWaiting;
        public float timeSpentMoving;
        

        private void Start()
        {
            path = new NavMeshPath();
            TryFindPath(targetPosition.position);
        }

        private void Update()
        {
            switch (state)
            {
                case PlayerState.Waiting:
                {
                    timeSpentWaiting += Time.deltaTime;
                    timeSpentMoving = 0;

                    if (Time.timeScale > 0.1f)
                    {
                        break;
                    }

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        var mousePosition = GetMouseWorldPosition();
                        if (TryFindPath(mousePosition))
                        {
                            targetPosition.position = waypoints[waypoints.Count - 1];
                            foundPath = true;
                        }
                    }

                    if (Input.GetKey(KeyCode.Mouse0) && foundPath)
                    {
                        var newPosition = targetPosition.position;
                        var frontDirection = (GetMouseWorldPosition() - newPosition).normalized;
                        var frontPosition = newPosition + (frontDirection * flashlightDirectionArrowDistance);
                        targetFlashlightDirectionArrow.transform.position = frontPosition;
                        targetFlashlightDirectionArrow.transform.up = frontDirection;
                        targetFlashlightDirectionArrow.transform.right = Vector3.Cross(frontDirection, Vector3.forward).normalized;
                    }
                    
                    if (Input.GetKeyUp(KeyCode.Mouse0))
                    {
                        var mousePosition = GetMouseWorldPosition();
                        targetFlashlightPosition.position = mousePosition;
                    }

                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        state = PlayerState.Moving;
                    }

                    break;
                }
                case PlayerState.Moving:
                {
                    timeSpentMoving += Time.deltaTime;
                    timeSpentWaiting = 0;

                    if (timeSpentMoving > maxTimeSpentMoving)
                    {
                        state = PlayerState.Waiting;
                    }

                    break;
                }
            }

            UpdateMovement();
        }

        private void UpdateMovement()
        {
            var targetFlashlightRotation = Quaternion.Euler(0, 0, GetAngleDegrees(targetFlashlightPosition.position - transform.position));
            var hasReachedTargetPosition = waypoints.Count == 0;
            var hasReachedTargetRotation = Quaternion.Angle(targetFlashlightRotation, flashlight.rotation) < 0.1f;

            if (state == PlayerState.Moving && hasReachedTargetPosition && hasReachedTargetRotation)
            {
                state = PlayerState.Waiting;

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

        public enum PlayerState
        {
            Waiting,
            Moving,
        }
    }
}
