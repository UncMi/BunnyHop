using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chunk : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    int vertexIndex = 0;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();


    
    void Start()
    {


        AddVoxelDataToChunk(transform.position + transform.right);
        CreateMesh();
    }
    void AddVoxelDataToChunk(Vector3 pos)
    {

        for (int j = 0; j < 6; j++)
        {
            for (int i = 0; i < 6; i++)
            {
                int triangleIndex = VoxelData.voxelTris[j, i];
                vertices.Add(VoxelData.voxelVerts[triangleIndex] + pos);
                triangles.Add(vertexIndex);

                uvs.Add(VoxelData.voxelUvs[i]);

                vertexIndex++; 
            }
        }
    }

    void CreateMesh() 
    {

        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray()
        };

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}
