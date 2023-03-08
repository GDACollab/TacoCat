using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicMesh : MonoBehaviour {

    private void Start() {
        Debug.Log("Test");

        CreateBasicQuadMesh(5);
        //CreateTileMesh();
        //CreateAnimationMesh();
    }

    private void CreateBasicQuadMesh(int quadCount)
    {
        Mesh mesh = new Mesh();

        // Create arrays to store the vertices, uv and triangles for the specified number of quads
        Vector3[] vertices = new Vector3[quadCount * 4];
        Vector2[] uv = new Vector2[quadCount * 4];
        int[] triangles = new int[quadCount * 6];

        // Loop through each quad
        for (int i = 0; i < quadCount; i++)
        {
            // Calculate the offset for the current quad
            int offset = i * 4;

            // Set the vertices for the current quad
            vertices[offset + 0] = new Vector3(100 * i, 0);
            vertices[offset + 1] = new Vector3(100 * i, 100);
            vertices[offset + 2] = new Vector3(100 * i + 100, 100);
            vertices[offset + 3] = new Vector3(100 * i + 100, 0);

            // Set the UVs for the current quad
            uv[offset + 0] = new Vector2(0, 0);
            uv[offset + 1] = new Vector2(0, 1);
            uv[offset + 2] = new Vector2(1, 1);
            uv[offset + 3] = new Vector2(1, 0);

            // Calculate the offset for the triangles for the current quad
            int trioffset = i * 6;
            // Set the triangles for the current quad
            triangles[trioffset + 0] = offset + 0;
            triangles[trioffset + 1] = offset + 1;
            triangles[trioffset + 2] = offset + 2;

            triangles[trioffset + 3] = offset + 0;
            triangles[trioffset + 4] = offset + 2;
            triangles[trioffset + 5] = offset + 3;
        }

        // Set the mesh data
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        // Assign the mesh to the MeshFilter component
        GetComponent<MeshFilter>().mesh = mesh;
    }


    private void CreateTileMesh() {
        Mesh mesh = new Mesh();

        int width = 4;
        int height = 4;
        float tileSize = 10;

        Vector3[] vertices = new Vector3[4 * (width * height)];
        Vector2[] uv = new Vector2[4 * (width * height)];
        int[] triangles = new int[6 * (width * height)];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                int index = i * height + j;

                vertices[index * 4 + 0] = new Vector3(tileSize * i,         tileSize * j);
                vertices[index * 4 + 1] = new Vector3(tileSize * i,         tileSize * (j + 1));
                vertices[index * 4 + 2] = new Vector3(tileSize * (i + 1),   tileSize * (j + 1));
                vertices[index * 4 + 3] = new Vector3(tileSize * (i + 1),   tileSize * j);
                
                uv[index * 4 + 0] = new Vector2(0, 0);
                uv[index * 4 + 1] = new Vector2(0, 1);
                uv[index * 4 + 2] = new Vector2(1, 1);
                uv[index * 4 + 3] = new Vector2(1, 0);

                triangles[index * 6 + 0] = index * 4 + 0;
                triangles[index * 6 + 1] = index * 4 + 1;
                triangles[index * 6 + 2] = index * 4 + 2;

                triangles[index * 6 + 3] = index * 4 + 0;
                triangles[index * 6 + 4] = index * 4 + 2;
                triangles[index * 6 + 5] = index * 4 + 3;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().mesh = mesh;

    }
}
