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
                    
                    var vertices = mesh.vertices.ToList();

                    // PolygonCollider is translated by a position already when converted to mesh, shortcut to translate back
                    for (var i = 0; i < vertices.Count; i++)
                    {
                        vertices[i] -= collider.transform.position;
                    }
                    
                    // Duplicate vertices, increase or decrease height of both, consider normals
                    var originalVerticesCount = vertices.Count;
                    for (var i = 0; i < originalVerticesCount; i++)
                    {
                        var near = vertices[i] + new Vector3(0,0, -1);
                        var far = vertices[i] + new Vector3(0,0, 1);

                        vertices[i] = near;
                        vertices.Add(far); // Add to end, where 0 -> n+1, 1, -> n+2, ... <- This is the key!
                    }
                    
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
                    
                    // Stitch the two faces together using triangles -> make box using 2 tris with current and next coord
                    for (var i = 0; i < originalVerticesCount; i++)
                    {
                        triangles.Add(i);
                        triangles.Add(i+1);
                        triangles.Add(originalVerticesCount + i);
                        
                        triangles.Add(i+1);
                        triangles.Add(originalVerticesCount + i);
                        triangles.Add(originalVerticesCount + i - 1);
                    }

                    mesh.SetVertices(vertices);
                    mesh.SetTriangles(triangles, 0);

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
    }
}
