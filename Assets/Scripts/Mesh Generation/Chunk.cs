namespace MiniAstro.TerrainGeneration
{
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(MeshCollider), typeof(MeshFilter), typeof(MeshRenderer))]
    public class Chunk : MonoBehaviour
    {
        public Vector3Int coord;
        public Vector3 centre;
        public float boundsSize;
        public Mesh mesh;
        MeshCollider meshCollider;
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        public Vector4[] scalarField;
        bool isColliderGenerated;
        public int numPointsPerAxis;
        public Transform bottomPlane;
        public bool bottomPlaneActive;

        public void SetUp(Material mat, bool isColliderGenerated, float boundsSize, int numPointsPerAxis, Vector3 centre)
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();

            mesh = meshFilter.sharedMesh;
            if (mesh == null)
            {
                mesh = new Mesh();
                meshFilter.sharedMesh = mesh;
            }
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            meshRenderer.material = mat;
            this.isColliderGenerated = isColliderGenerated;

            this.boundsSize = boundsSize;
            this.numPointsPerAxis = numPointsPerAxis;
            scalarField = new Vector4[numPointsPerAxis * numPointsPerAxis * numPointsPerAxis];
            this.centre = centre;

            BoxCollider boxCollider = GetComponentInChildren<BoxCollider>();
            boxCollider.center = this.centre;
            boxCollider.size = new Vector3(boundsSize, boundsSize, boundsSize);

            if (coord.y == 0 && bottomPlaneActive)
            {
                bottomPlane.gameObject.SetActive(true);
                bottomPlane.localScale *= boundsSize;
                bottomPlane.localPosition = centre;
                bottomPlane.localPosition += Vector3.down * boundsSize / 2;
            }
            else
            {
                bottomPlane.gameObject.SetActive(false);
            }
        }

        public void UpdateChunkMesh(MarchingCube marchingCube)
        {
            List<int> triangles = new List<int>();
            List<Vector3> vertices = new List<Vector3>();
            for (int x = 0; x < marchingCube.numPointsPerAxis - 1; x++)
                for (int y = 0; y < marchingCube.numPointsPerAxis - 1; y++)
                    for (int z = 0; z < marchingCube.numPointsPerAxis - 1; z++)
                    {
                        marchingCube.March(scalarField, new Vector3Int(x, y, z), vertices, triangles);
                    }

            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();

            if (isColliderGenerated)
            {
                meshCollider.enabled = false;
                meshCollider.enabled = true;

            }
        }

        public void AssignData(ChunkData data)
        {
            this.coord = data.coord;
            this.centre = data.centre;
            this.boundsSize = data.boundsSize;
            this.scalarField = data.scalarField;
            this.numPointsPerAxis = data.numPointsPerAxis;
        }

        public void UpdateCollider()
        {
            meshCollider.cookingOptions = MeshColliderCookingOptions.WeldColocatedVertices;
            if (meshCollider.sharedMesh == null)
            {
                meshCollider.sharedMesh = mesh;
            }
            // Forced update
            meshCollider.enabled = false;
            meshCollider.enabled = true;
        }

        public ChunkData GetChunkData()
        {
            return new ChunkData(this);
        }
    }

}
