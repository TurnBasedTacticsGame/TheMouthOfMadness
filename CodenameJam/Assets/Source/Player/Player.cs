using System;
using System.Collections.Generic;
using Source.GameEvents.Core;
using UniDi;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Source.Player
{
    public class Player : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Rigidbody2D rb;

        [Inject] private EventTracker tracker;
        [Inject] private Camera mainCamera;

        public PlayerState state;

        public Transform targetPosition;
        public Transform targetFlashlightPosition;
        public Transform flashlight;
        public float timeWaited;

        public float movementSpeed = 5;
        public float rotationSpeed = 90;
        public float turnWaitTime = 1;
        public float maxTurnTime = 10;

        private List<Vector3> waypoints = new();
        private NavMeshPath path;

        private void Start()
        {
            path = new NavMeshPath();
        }

        private void Update()
        {
            switch (state)
            {
                case PlayerState.Waiting:
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        var mousePosition = GetMouseWorldPosition();
                        if (TryFindPath(mousePosition))
                        {
                            targetPosition.position = waypoints[waypoints.Count - 1];
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Mouse1))
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
                    var targetFlashlightRotation = Quaternion.Euler(0, 0, GetAngleDegrees(targetFlashlightPosition.position - transform.position));
                    var hasReachedTargetPosition = waypoints.Count == 0;
                    var hasReachedTargetRotation = Quaternion.Angle(targetFlashlightRotation, flashlight.rotation) < 0.01f;

                    if (hasReachedTargetPosition && hasReachedTargetRotation)
                    {
                        state = PlayerState.Waiting;

                        break;
                    }

                    if (waypoints.Count > 0)
                    {
                        var direction = waypoints[0] - transform.position;
                        direction = waypoints.Count == 1 ? Vector3.ClampMagnitude(direction, 1) : direction.normalized;

                        rb.velocity = Vector3.ClampMagnitude(direction, 1) * movementSpeed;

                        var hasReachedCurrentWaypoint = (waypoints[0] - transform.position).sqrMagnitude < 0.01f;
                        if (hasReachedCurrentWaypoint)
                        {
                            waypoints.RemoveAt(0);
                        }
                    }

                    flashlight.rotation = Quaternion.RotateTowards(flashlight.rotation, targetFlashlightRotation, rotationSpeed * Time.deltaTime);

                    break;
                }
            }
        }

        // private bool HasRemainingMovement()
        // {
        //
        // }

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
