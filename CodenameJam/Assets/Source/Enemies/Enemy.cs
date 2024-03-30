using System;
using System.Collections.Generic;
using System.Threading;
using Source.Players;
using UniDi;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Source.Enemies
{
    public class Enemy : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Rigidbody2D rb;

        [SerializeField] private float damage = 1;
        [SerializeField] private float movementSpeed = 1;

        [Inject] private Player player;

        private List<Vector3> waypoints = new();
        private NavMeshPath path;

        private float pathFindCooldown = 0.5f;
        private float pathFindTimer;

        private void Start()
        {
            path = new NavMeshPath();
            pathFindTimer = Random.Range(0, 0.5f);
        }

        private void Update()
        {
            pathFindTimer += Time.deltaTime;
            if (pathFindTimer > pathFindCooldown)
            {
                var destination = player.transform.position;
                TryFindPath(destination);

                pathFindTimer = 0;
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
    }
}
