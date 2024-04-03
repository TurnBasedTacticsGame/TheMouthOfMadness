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
            Attacked
        }

        [Header("Dependencies")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private AudioSource hissAudio;
        [SerializeField] private Animator animator;
        [SerializeField] private AudioClipData hissingClips;
        [SerializeField] private Collider2D collider;
        [SerializeField] private Collider2D damageTriggerCollider;

        [Header("Configuration")]
        [SerializeField] private float damage = 1;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float movementSpeed = 1;
        [SerializeField] private float aggroRange = 10;
        [SerializeField] private bool aggroUsingPlayerDirection;
        [SerializeField] private float aggroDirectionAngle;
        [SerializeField] private float aggroDirectionAngularWidth;
        [SerializeField] private float deaggroRange = 15;
        [SerializeField] private float maxPathRange = 20;

        [Inject] private Player player;

        private List<Vector3> waypoints = new();
        private NavMeshPath path;

        private float pathFindCooldown = 0.5f;
        private float updateTimer;
        private float anguleToPlayer;

        private EnemyState previousStateBeforeAttacking;
        private bool Attacked => attackTimer > 0;
        private float attackTimer = 1f;

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

                    if (IsEnemyAggro())
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
            
            UpdateDamageTrigger();
        }

        private void OnTriggerStay2D(Collider2D col)
        {
            if (Attacked)
                return;
            
            if (col.attachedRigidbody != null && col.attachedRigidbody.TryGetComponent<Player>(out var player))
            {
                Debug.Log("Enemy collided with player");
                var success = player.TryTakeDamage(damage);

                if ( success)
                {
                    // Disable colliders (you can run past)
                    //PlayHissAudio().Forget();
                    collider.gameObject.SetActive(false);
                    damageTriggerCollider.gameObject.SetActive(false);

                    // Set cooldown
                    attackTimer = attackCooldown;
                    
                    // Stop chasing
                    previousStateBeforeAttacking = state;
                    state = EnemyState.Attacked;
                }
            }
        }

        private void UpdateDamageTrigger()
        {
            if (!Attacked)
                return;
            
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                collider.gameObject.SetActive(true);
                damageTriggerCollider.gameObject.SetActive(true);
                state = previousStateBeforeAttacking;
                attackTimer = 0;
            }
        }

        private async UniTask PlayHissAudio()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(0.1f, 1f)), true);

            hissAudio.clip = hissingClips.AudioClips[Random.Range(0, hissingClips.AudioClips.Length)];
            hissAudio.Play();
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

        private bool IsEnemyAggro()
        {
            var playerToEnemy = (player.transform.position - transform.position);
            var playerDistance = playerToEnemy.magnitude;

            return playerDistance < aggroRange
                   && (!aggroUsingPlayerDirection || LookingAtPositionWithinBounds(player.transform.position, aggroDirectionAngle, aggroDirectionAngularWidth));
        }

        private bool LookingAtPositionWithinBounds(Vector3 position, float lookAngle, float lookAngularWidth)
        {
            var directionToPosition = (position - transform.position);
            var directionToPositionXY = new Vector3(directionToPosition.x, directionToPosition.y, 0).normalized;
            var lookBounds = CalculateLookBounds(lookAngle, lookAngularWidth);
            
            var angleToPlayer = Vector3.Angle(directionToPositionXY, lookBounds.Straight); //Whoops tried to use linear transformation (1 - dot), good lesson.  * 90;
            anguleToPlayer = angleToPlayer;
            
            return angleToPlayer <= lookBounds.AngularWidth;
        }

        private LookBounds CalculateLookBounds(float angleDeg, float angularWidthDeg)
        {
            // Can repeat use of bounds.
            var bounds = new LookBounds
            {
                Straight = new Vector3(
                    Mathf.Cos(angleDeg * Mathf.Deg2Rad),
                    Mathf.Sin(angleDeg * Mathf.Deg2Rad),
                    0 
                ),
                Left = new Vector3(
                    Mathf.Cos((angleDeg + angularWidthDeg) * Mathf.Deg2Rad),
                    Mathf.Sin((angleDeg + angularWidthDeg)  * Mathf.Deg2Rad),
                    0 
                ),
                Right = new Vector3(
                    Mathf.Cos((angleDeg - angularWidthDeg) * Mathf.Deg2Rad),
                    Mathf.Sin((angleDeg - angularWidthDeg)  * Mathf.Deg2Rad),
                    0 
                ),
                AngularWidth = angularWidthDeg
            };

            return bounds;
        }

        private struct LookBounds
        {
            public Vector3 Left;
            public Vector3 Straight;
            public Vector3 Right;
            public float AngularWidth;
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            aggroDirectionAngle = Mathf.Clamp(aggroDirectionAngle, 0, 360);
            aggroDirectionAngularWidth = Mathf.Clamp(aggroDirectionAngularWidth, 0, 180);
        }

        private void OnDrawGizmosSelected()
        {
            // Show aggro range
            if (player != null && IsEnemyAggro())
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }
            
            Gizmos.DrawWireSphere(transform.position, aggroRange);

            if (aggroUsingPlayerDirection)
            {
                var bounds = CalculateLookBounds(aggroDirectionAngle, aggroDirectionAngularWidth);
                
                Gizmos.DrawLine(transform.position, transform.position + bounds.Straight * aggroRange);
                Gizmos.DrawLine(transform.position, transform.position + bounds.Left * aggroRange);
                Gizmos.DrawLine(transform.position, transform.position + bounds.Right * aggroRange);
            }
        }
#endif
    }
}
