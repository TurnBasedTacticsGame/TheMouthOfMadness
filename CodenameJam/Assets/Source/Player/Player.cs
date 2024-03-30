using UnityEngine;

namespace Source.Player
{
    public class Player : MonoBehaviour
    {
        public Transform targetPosition;
        public Transform targetFlashlightPosition;
        public float timeWaited;

        public float movementSpeed;
        public float rotationSpeed;
    }
}
