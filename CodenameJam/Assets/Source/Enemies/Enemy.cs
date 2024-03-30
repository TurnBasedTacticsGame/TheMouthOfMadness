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
        private enum EnemyState
        {
            Idle,
            MovingToPlayer,
        }

        [Header("Dependencies")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private AudioSource hissAudio;

        [SerializeField] private float damage = 1;
        [SerializeField] private float movementSpeed = 1;
        [SerializeField] private float aggroRange = 10;
        [SerializeField] private float deaggroRange = 15;
        [SerializeField] private float maxPathRange = 20;

        [Inject] private Player player;

        private List<Vector3> waypoints = new();
        private NavMeshPath path;

        private float pathFindCooldown = 0.5f;
        private float updateTimer;

        private EnemyState state;
        private Vector3 initialPosition;

        private void Start()
        {
            path = new NavMeshPath();
            updateTimer = Random.Range(0, 0.5f);
            initialPosition = transform.position;
        }

        private void Update()
        {
            switch (state)
            {
                case EnemyState.Idle:
                {
                    var playerDistance = (player.transform.position - transform.position).magnitude;
                    if (playerDistance < aggroRange)
                    {
                        SlowUpdatePath(player.transform.position);
                        if (path.status != NavMeshPathStatus.PathInvalid)
                        {
                            hissAudio.Play();
                            state = EnemyState.MovingToPlayer;
                        }
                    }
                    else
                    {
                        SlowUpdatePath(initialPosition);
                    }

                    break;
                }
                case EnemyState.MovingToPlayer:
                {
                    SlowUpdatePath(player.transform.position);
                    if (GetPathLength() > maxPathRange)
                    {
                        state = EnemyState.Idle;
                    }

                    break;
                }
            }

            // Always move
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

        private float GetPathLength()
        {
            var result = 0f;
            for (var i = 1; i < waypoints.Count; i++)
            {
                result += (waypoints[i - 1] - waypoints[i]).magnitude;
            }

            return result;
        }

        private void SlowUpdatePath(Vector3 destination)
        {
            updateTimer += Time.deltaTime;
            if (updateTimer > pathFindCooldown)
            {
                FindPath(destination);

                updateTimer = 0;
            }
        }

        private void FindPath(Vector3 destination)
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
            }
        }
    }
}
