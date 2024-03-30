using UnityEngine;

namespace Source.Convert2DTo3D
{
    public class Convert2DTo3D : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Collider cylinderPrefab;

        [Header("Configuration")] 
        [SerializeField] private float colliderHeight = 1000;
        
        private GameObject collider3DRoot;
        
        // Start is called before the first frame update
        void Start()
        {
            var allColliders = FindObjectsOfType<Collider2D>();

            collider3DRoot = new GameObject("Collider3DRoot");

            foreach (var collider in allColliders)
            {
                if (collider is CircleCollider2D circleCollider2D)
                {
                    var cylinder = GameObject.Instantiate(cylinderPrefab, collider.transform.position, Quaternion.identity);
                    cylinder.transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                    cylinder.transform.parent = collider3DRoot.transform;
                    var diameter = circleCollider2D.radius * 2;
                    cylinder.transform.localScale = new Vector3(diameter, colliderHeight * 2, diameter);
                } 
            }
            
            // Simulate transform along x-axis
            collider3DRoot.transform.Rotate(new Vector3(-90, 0, 0), Space.Self);
        }
    }
}
