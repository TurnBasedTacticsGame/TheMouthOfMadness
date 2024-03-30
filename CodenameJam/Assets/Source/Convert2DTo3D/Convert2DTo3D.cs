using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Source.Convert2DTo3D
{
    public class Convert2DTo3D : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Collider cylinderPrefab;
        [SerializeField] private Collider cubePrefab;
        [SerializeField] private Material defaultMeshMaterial;

        [Header("Configuration")] 
        [SerializeField] private float colliderHeight = 1000;
        
        private GameObject collider3DRoot;
        private List<Mesh> meshes = new ();

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
                    var diameter = circleCollider2D.radius * 2;
                    cylinder.transform.localScale = new Vector3(diameter, colliderHeight, diameter); // Cylinder has height 1 up, 1 down.
                    cylinder.transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                    
                    cylinder.transform.SetParent(collider3DRoot.transform);
                } 
                else if (collider is BoxCollider2D boxCollider2D)
                {
                    var cube = GameObject.Instantiate(cubePrefab, collider.transform.position, Quaternion.identity);
                    cube.transform.localScale = new Vector3(boxCollider2D.size.x, colliderHeight * 2, boxCollider2D.size.y); // Cube has height 0.5 up, 0.5 down
                    cube.transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                    
                    cube.transform.SetParent(collider3DRoot.transform);
                } 
                else if (collider is PolygonCollider2D polygonCollider2D)
                {
                    var meshObject = new GameObject("MeshCollider3D");
                    
                    var meshFilter = meshObject.AddComponent<MeshFilter>();
                    var mesh = polygonCollider2D.CreateMesh(false, false);

                    var meshData = new MeshData();
                    meshData.Vertices = mesh.vertices.ToList();
                    
                    // PolygonCollider is translated by a position already when converted to mesh, shortcut to translate back
                    for (var i = 0; i < meshData.Vertices.Count; i++)
                    {
                        meshData.Vertices[i] -= collider.transform.position;
                    }

                    for (var i = 0; i < mesh.vertices.Length; i++)
                    {
                        
                    }
                    
                    // Duplicate vertices, increase or decrease height of both, consider normals
                    var originalVertices = meshData.Vertices.Count;
                    for (var i = 0; i < originalVertices; i++)
                    {
                        var near1 = meshData.Vertices[i] + new Vector3(0,0, -1);
                        var far1 = meshData.Vertices[i] + new Vector3(0,0, 1);
                        
                        var near2 = meshData.Vertices[i+1] + new Vector3(0,0, -1);
                        var far2 = meshData.Vertices[i+1] + new Vector3(0,0, 1);
                        
                        var quad = new Quad
                        {
                            BottomLeft = near2,
                            BottomRight = near1,
                            TopLeft = far2,
                            TopRight = far1
                        };
                        
                        quad.Triangulate(meshData);
                    }
                    
                    /*
                    var triangles = mesh.triangles.ToList();
                    var originalTrianglesCount = triangles.Count;
                    
                    // Make triangle for far end -> make boxes between all new vertices
                    for (var i = 0; i < originalTrianglesCount; i+=3)
                    {
                        // Recreate original triangles but with new vertices (old + offset)
                        var newIndex1 = triangles[i] + originalVerticesCount;
                        var newIndex2 = triangles[i+1] + originalVerticesCount;
                        var newIndex3 = triangles[i+2] + originalVerticesCount;
                        
                        // Flip indices to flip implicit normals (faces in opposite direction -> triangle indices reverse clock rotation)
                        triangles.Add(newIndex1);
                        triangles.Add(newIndex3);
                        triangles.Add(newIndex2);
                    }
                    */

                    mesh.SetVertices(meshData.Vertices);
                    mesh.SetTriangles(meshData.Triangles, 0);

                    meshes.Add(mesh);
                    meshFilter.sharedMesh = mesh;

                    var meshRenderer = meshObject.AddComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = defaultMeshMaterial;
                    
                    meshObject.transform.position = collider.transform.position;
                    meshObject.transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                    meshObject.transform.SetParent(collider3DRoot.transform);
                }
            }
            
            // Simulate transform along x-axis
            collider3DRoot.transform.Rotate(new Vector3(-90, 0, 0), Space.Self);
        }


        private void OnDestroy()
        {
            // Cleanup meshes, not part of prefab
            for (var i = meshes.Count - 1; i > 0; i--)
            {
                Destroy(meshes[i]);
            }
        }
        
        private class MeshData
        {
            public List<Vector3> Vertices = new();
            public List<Vector3> Normals = new();
            public List<Vector2> Uv = new();
            public List<int> Triangles = new();
        }

        private struct Quad
        {
            public Vector3 BottomLeft;
            public Vector3 TopLeft;
            public Vector3 TopRight;
            public Vector3 BottomRight;

            public Vector2 UvOffset;
            public Vector2 UvSize;

            public void Triangulate(MeshData meshData)
            {
                var verticesStartIndex = meshData.Vertices.Count;

                meshData.Vertices.Add(BottomLeft);
                meshData.Uv.Add(UvOffset);

                meshData.Vertices.Add(TopLeft);
                meshData.Uv.Add(UvOffset + Vector2.up * UvSize.y);

                meshData.Vertices.Add(TopRight);
                meshData.Uv.Add(UvOffset + Vector2.right * UvSize.x + Vector2.up * UvSize.y);

                meshData.Vertices.Add(BottomRight);
                meshData.Uv.Add(UvOffset + Vector2.right * UvSize.x);

                // Triangle 1
                meshData.Triangles.Add(verticesStartIndex + 0);
                meshData.Triangles.Add(verticesStartIndex + 1);
                meshData.Triangles.Add(verticesStartIndex + 2);

                // Triangle 2
                meshData.Triangles.Add(verticesStartIndex + 2);
                meshData.Triangles.Add(verticesStartIndex + 3);
                meshData.Triangles.Add(verticesStartIndex + 0);

                var normal = Vector3.Cross(TopRight - TopLeft, BottomLeft - TopLeft);
                for (var i = 0; i < 4; i++)
                {
                    meshData.Normals.Add(normal);
                }
            }
        }
    }
}
