namespace MiniAstro.TerrainGeneration
{
    using System.Collections.Generic;
    using UnityEngine;

    public class TerrainTool : MonoBehaviour
    {
        public enum ToolMode
        {
            Dig,
            Fill,
            Flatten
        }
        public ToolMode mode = default;
        public ToolMarker markerPrefab;
        protected ToolMarker marker;
        [SerializeField] protected MeshGenerator terrain;
        [SerializeField] protected MapGenerator map;
        //protected int layerMask = 1 << 8;
        public float radius = 1f;
        [SerializeField] protected float range = 100f;
        [SerializeField] protected float strength = 0.2f;
        public Camera cam;
        //protected PhysicsRaycaster raycaster;
        protected Vector3 HitPosition;
        public ComputeShader diggingShader;
        protected List<Chunk> neighbouringChunks = new List<Chunk>();
        protected int raycastIgnoreMask = 1 << 18;
        [Header("Audio")]
        public AudioSource audioSource;

        public int RaycastIgnoreMask
        {
            get { return raycastIgnoreMask; }
        }

        // Samples for flattening
        protected Vector3 faceNormalSample;
        protected Vector3 diggingPointSample;
        protected bool samplesSet = false;

        void Awake()
        {
            strength /= 1000;
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            // Ignore 16th, 17th layer
            //raycastIgnoreMask = 3 << 16;
            //raycastIgnoreMask = 1 << 18;
            //raycastIgnoreMask = ~raycastIgnoreMask;
        }



        protected void ChooseToolMode()
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                mode += 1;
                if (!System.Enum.IsDefined(typeof(ToolMode), mode))
                    mode = ToolMode.Dig;
                samplesSet = false;
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                mode -= 1;
                if (!System.Enum.IsDefined(typeof(ToolMode), mode))
                    mode = ToolMode.Flatten;
                samplesSet = false;
            }

        }

        protected void SetToolMarker(RaycastHit hit)
        {
            switch (mode)
            {
                case ToolMode.Dig:
                    marker.Marker = ToolMarker.MarkerType.down;
                    break;
                case ToolMode.Fill:
                    marker.Marker = ToolMarker.MarkerType.up;
                    break;
                case ToolMode.Flatten:
                    marker.Marker = ToolMarker.MarkerType.flat;
                    break;
            }

            if (hit.distance > range)
                marker.SetColor(Color.red);
            else
                marker.SetColor(Color.white);
        }

        protected void SetNeighbouringChunks(Vector3 hitPosition, float radius, List<Chunk> neighbours)
        {
            neighbours.Clear();
            int layerMask = 1 << 16;
            foreach (Collider col in Physics.OverlapSphere(hitPosition, radius, layerMask))
            {
                Chunk neighbour = col.gameObject.GetComponentInParent<Chunk>();
                if (neighbour != null)
                {
                    neighbours.Add(neighbour);
                }
            }
        }

        protected virtual void Dig(Chunk chunk, RaycastHit hit, float radius)
        {
            ComputeBuffer voxelsBuffer = new ComputeBuffer(chunk.numPointsPerAxis * chunk.numPointsPerAxis * chunk.numPointsPerAxis, sizeof(float) * 4);
            voxelsBuffer.SetData(chunk.scalarField);

            int threadGroupSize = 8;
            int numVoxelsPerAxis = chunk.numPointsPerAxis - 1;
            int numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float)threadGroupSize);
            float pointSpacing = chunk.boundsSize / (chunk.numPointsPerAxis - 1);

            diggingShader.SetBuffer(diggingShader.FindKernel("Digging"), "points", voxelsBuffer);
            diggingShader.SetFloat("value", -strength);
            diggingShader.SetFloat("radius", radius);
            diggingShader.SetFloat("spacing", pointSpacing);
            diggingShader.SetFloat("boundsSize", chunk.boundsSize);
            diggingShader.SetVector("centre", new Vector4(chunk.centre.x, chunk.centre.y, chunk.centre.z, radius));
            diggingShader.SetInt("numPointsPerAxis", chunk.numPointsPerAxis);
            diggingShader.SetVector("diggingPoint", new Vector4(hit.point.x, hit.point.y, hit.point.z, 0));
            diggingShader.Dispatch(diggingShader.FindKernel("Digging"), numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);
            // Pass points values to the chunk
            voxelsBuffer.GetData(chunk.scalarField);

            voxelsBuffer.Release();
        }

        protected virtual void Fill(Chunk chunk, RaycastHit hit, float radius)
        {
            ComputeBuffer voxelsBuffer = new ComputeBuffer(chunk.numPointsPerAxis * chunk.numPointsPerAxis * chunk.numPointsPerAxis, sizeof(float) * 4);
            voxelsBuffer.SetData(chunk.scalarField);

            int threadGroupSize = 8;
            int numVoxelsPerAxis = chunk.numPointsPerAxis - 1;
            int numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float)threadGroupSize);
            float pointSpacing = chunk.boundsSize / (chunk.numPointsPerAxis - 1);

            diggingShader.SetBuffer(diggingShader.FindKernel("Digging"), "points", voxelsBuffer);
            diggingShader.SetFloat("value", strength);
            diggingShader.SetFloat("radius", radius);
            diggingShader.SetFloat("spacing", pointSpacing);
            diggingShader.SetFloat("boundsSize", chunk.boundsSize);
            diggingShader.SetVector("centre", new Vector4(chunk.centre.x, chunk.centre.y, chunk.centre.z, radius));
            diggingShader.SetInt("numPointsPerAxis", chunk.numPointsPerAxis);
            diggingShader.SetVector("diggingPoint", new Vector4(hit.point.x, hit.point.y, hit.point.z, 0));
            diggingShader.SetVector("faceNormal", new Vector4(hit.normal.x, hit.normal.y, hit.normal.z, 0));
            diggingShader.Dispatch(diggingShader.FindKernel("Digging"), numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);
            // Pass points values to the chunk
            voxelsBuffer.GetData(chunk.scalarField);

            voxelsBuffer.Release();
        }

        protected virtual void Flatten(Chunk chunk, RaycastHit hit, float radius)
        {
            ComputeBuffer voxelsBuffer = new ComputeBuffer(chunk.numPointsPerAxis * chunk.numPointsPerAxis * chunk.numPointsPerAxis, sizeof(float) * 4);
            voxelsBuffer.SetData(chunk.scalarField);

            int threadGroupSize = 8;
            int numVoxelsPerAxis = chunk.numPointsPerAxis - 1;
            int numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float)threadGroupSize);
            float pointSpacing = chunk.boundsSize / (chunk.numPointsPerAxis - 1);

            diggingShader.SetBuffer(diggingShader.FindKernel("Flatten"), "points", voxelsBuffer);
            diggingShader.SetFloat("value", strength);
            diggingShader.SetFloat("radius", radius);
            diggingShader.SetFloat("spacing", pointSpacing);
            diggingShader.SetFloat("boundsSize", chunk.boundsSize);
            diggingShader.SetVector("centre", new Vector4(chunk.centre.x, chunk.centre.y, chunk.centre.z, radius));
            diggingShader.SetInt("numPointsPerAxis", chunk.numPointsPerAxis);
            diggingShader.SetVector("diggingPoint", new Vector4(hit.point.x, hit.point.y, hit.point.z, 0));
            diggingShader.SetVector("diggingPointSample", new Vector4(diggingPointSample.x, diggingPointSample.y, diggingPointSample.z, 0));
            diggingShader.SetVector("faceNormal", new Vector4(faceNormalSample.x, faceNormalSample.y, faceNormalSample.z, 0));
            diggingShader.Dispatch(diggingShader.FindKernel("Flatten"), numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);
            // Pass points values to the chunk
            voxelsBuffer.GetData(chunk.scalarField);

            voxelsBuffer.Release();
        }
    }
}



