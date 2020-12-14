using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCube : MonoBehaviour
{
    [Range(0, 1)] public float isoLevel = 0.3f;
    [Range(1, 100)] public int numPointsPerAxis = 30;

    Vector3 InterpolateVerts(Vector4 v1, Vector4 v2)
    {
        float t = (isoLevel - v1.w) / (v2.w - v1.w);
        //return v1.xyz + t * (v2.xyz - v1.xyz);
        //t = 0.5f;
        return Vector3.Lerp(new Vector3(v1.x, v1.y, v1.z), new Vector3(v2.x, v2.y, v2.z), t);
    }

    int indexFromCoord(int x, int y, int z)
    {
        return y * numPointsPerAxis * numPointsPerAxis + z * numPointsPerAxis + x;
    }

    public void March(Vector4[] scalars, Vector3Int point, List<Vector3> vertices, List<int> triangles)
    {
        if (point.x >= numPointsPerAxis - 1 || point.y >= numPointsPerAxis - 1 || point.z >= numPointsPerAxis - 1)
        {
            return;
        }


        // 8 current corners of the cube
        Vector4[] cubeCorners = {
        scalars[indexFromCoord(point.x, point.y, point.z)],
        scalars[indexFromCoord(point.x + 1, point.y, point.z)],
        scalars[indexFromCoord(point.x + 1, point.y, point.z + 1)],
        scalars[indexFromCoord(point.x, point.y, point.z + 1)],
        scalars[indexFromCoord(point.x, point.y + 1, point.z)],
        scalars[indexFromCoord(point.x + 1, point.y + 1, point.z)],
        scalars[indexFromCoord(point.x + 1, point.y + 1, point.z + 1)],
        scalars[indexFromCoord(point.x, point.y + 1, point.z + 1)]
        };

        // Calculate unique index for each cube configuration.
        // There are 256 possible values
        // A value of 0 means cube is entirely inside surface; 255 entirely outside.
        // The value is used to look up the edge table, which indicates which edges of the cube are cut by the isosurface.
        int cubeIndex = 0;
        if (cubeCorners[0].w < isoLevel) cubeIndex |= 1;
        if (cubeCorners[1].w < isoLevel) cubeIndex |= 2;
        if (cubeCorners[2].w < isoLevel) cubeIndex |= 4;
        if (cubeCorners[3].w < isoLevel) cubeIndex |= 8;
        if (cubeCorners[4].w < isoLevel) cubeIndex |= 16;
        if (cubeCorners[5].w < isoLevel) cubeIndex |= 32;
        if (cubeCorners[6].w < isoLevel) cubeIndex |= 64;
        if (cubeCorners[7].w < isoLevel) cubeIndex |= 128;

        // Create triangles for current cube configuration
        for (int i = 0; MarchingCubeTables.triangulation[cubeIndex, i] != -1; i += 3)
        {
            // Get indices of corner points A and B for each of the three edges
            // of the cube that need to be joined to form the triangle.
            int a0 = MarchingCubeTables.cornerIndexAFromEdge[MarchingCubeTables.triangulation[cubeIndex, i]];
            int b0 = MarchingCubeTables.cornerIndexBFromEdge[MarchingCubeTables.triangulation[cubeIndex, i]];

            int a1 = MarchingCubeTables.cornerIndexAFromEdge[MarchingCubeTables.triangulation[cubeIndex, i + 1]];
            int b1 = MarchingCubeTables.cornerIndexBFromEdge[MarchingCubeTables.triangulation[cubeIndex, i + 1]];

            int a2 = MarchingCubeTables.cornerIndexAFromEdge[MarchingCubeTables.triangulation[cubeIndex, i + 2]];
            int b2 = MarchingCubeTables.cornerIndexBFromEdge[MarchingCubeTables.triangulation[cubeIndex, i + 2]];


            Vector3 vertexA = InterpolateVerts(cubeCorners[a0], cubeCorners[b0]);
            Vector3 vertexB = InterpolateVerts(cubeCorners[a1], cubeCorners[b1]);
            Vector3 vertexC = InterpolateVerts(cubeCorners[a2], cubeCorners[b2]);

            AddTriangle(vertexA, vertexB, vertexC, vertices, triangles);
        }   
    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, List<Vector3> vertices, List<int> triangles)
    {
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);

        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
    }
}
