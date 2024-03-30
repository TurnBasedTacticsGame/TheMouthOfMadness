using Source.GameEvents.Core;
using UniDi;
using Unity.Mathematics;
using UnityEngine;
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

        private void Update()
        {
            if (state == PlayerState.Waiting)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    var mousePosition = GetMouseWorldPosition();
                    targetPosition.position = mousePosition;
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
            }

            if (state == PlayerState.Moving)
            {
                var targetFlashlightRotation = Quaternion.LookRotation(targetFlashlightPosition.position - transform.position, Vector3.back);
                var hasReachedTargetPosition = (targetPosition.position - transform.position).sqrMagnitude < 0.1f;
                var hasReachedTargetRotation = Quaternion.Angle(targetFlashlightRotation, flashlight.rotation) < 0.1f;

                if (hasReachedTargetPosition && hasReachedTargetRotation)
                {
                    state = PlayerState.Waiting;
                }

                // Todo Use pathfinding
                rb.velocity = Vector3.ClampMagnitude(targetPosition.position - transform.position, 1) * movementSpeed;
                flashlight.rotation = Quaternion.RotateTowards(flashlight.rotation, targetFlashlightRotation, rotationSpeed * Time.deltaTime);
            }
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
