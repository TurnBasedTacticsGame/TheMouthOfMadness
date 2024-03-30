using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Source.Components
{
    [ExecuteAlways]
    public class PolygonColliderShadow : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private PolygonCollider2D polygonCollider;
        [SerializeField] private ShadowCaster2D shadowCaster;

        private FieldInfo meshField;
        private FieldInfo shapePathField;
        private MethodInfo generateShadowMeshMethod;

        private void OnEnable()
        {
            meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
            shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
            generateShadowMeshMethod = typeof(ShadowCaster2D)
                .Assembly
                .GetType("UnityEngine.Rendering.Universal.ShadowUtility")
                .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);

            UpdateShadow();
        }

        public void UpdateShadow()
        {
            polygonCollider = GetComponent<PolygonCollider2D>();
            shadowCaster = GetComponent<ShadowCaster2D>();

            if (!polygonCollider || !shadowCaster)
            {
                return;
            }

            var referenceVertices = polygonCollider.points;
            var vertices = new Vector3[referenceVertices.Length];

            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = referenceVertices[i];
            }

            shapePathField.SetValue(shadowCaster, vertices);
            meshField.SetValue(shadowCaster, new Mesh());
            generateShadowMeshMethod.Invoke(shadowCaster, new object[]
            {
                meshField.GetValue(shadowCaster),
                shapePathField.GetValue(shadowCaster),
            });

            shadowCaster.Update();
        }
    }
}
