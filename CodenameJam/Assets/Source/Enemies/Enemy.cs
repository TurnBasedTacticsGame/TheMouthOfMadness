using Source.Players;
using UniDi;
using UnityEngine;

namespace Source.Enemies
{
    public class Enemy : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Rigidbody2D rb;

        [SerializeField] private float damage = 1;
        [SerializeField] private float movementSpeed = 1;

        [Inject] private Player player;

        private void Update()
        {
            var direction = (player.transform.position - transform.position).normalized;
            rb.velocity = direction * movementSpeed;
        }
    }
}
