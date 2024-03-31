using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        [SerializeField] private Animator animator;
        [SerializeField] private AudioClipData hissingClips;

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
        private static readonly int Moving = Animator.StringToHash("Moving");

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
                    animator.SetBool(Moving, false);
                    
                    var playerDistance = (player.transform.position - transform.position).magnitude;
                    if (playerDistance < aggroRange)
                    {
                        SlowUpdatePath(player.transform.position);
                        if (path.status != NavMeshPathStatus.PathInvalid)
                        {
                            state = EnemyState.MovingToPlayer;

                            PlayHissAudio().Forget();
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
                    animator.SetBool(Moving, true);
                    
                    SlowUpdatePath(player.transform.position);
                    if (GetPathLength() > maxPathRange || (player.transform.position - transform.position).magnitude > deaggroRange)
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

        private async UniTask PlayHissAudio()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(0.1f, 1f)), true);

            hissAudio.clip = hissingClips.AudioClips[Random.Range(0, hissingClips.AudioClips.Length)];
            hissAudio.Play();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent<Player>(out var player))
            {
                player.TryTakeDamage(damage);
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
