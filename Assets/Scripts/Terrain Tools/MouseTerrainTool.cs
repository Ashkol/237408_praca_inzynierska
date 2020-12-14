namespace MiniAstro.TerrainGeneration
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MouseTerrainTool : TerrainTool
    {
        void Start()
        {
            marker = Instantiate(markerPrefab);
            marker.Marker = ToolMarker.MarkerType.down;
            marker.gameObject.SetActive(false);
            Cursor.visible = false;
        }

        public bool CastRay(out RaycastHit hit, int raycastIgnoreMask = 1 << 18)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out hit, Mathf.Infinity, raycastIgnoreMask);
        }

        void Update()
        {
            // Disables marker if cursor is over UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                marker.gameObject.SetActive(false);
                return;
            }

            RaycastHit hit;
            if (CastRay(out hit))
            {

                //Cursor.visible = false;
                marker.gameObject.SetActive(true);
                AdjustMarkerPosition(hit);
                marker.transform.position = hit.point;
                Cursor.visible = false;

                ChooseToolMode();
                SetToolMarker(hit);

                if (Input.GetMouseButton(0) && hit.distance < range)
                {
                    StartAction(hit);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    samplesSet = false;
                    audioSource.Stop();
                }
            }
            else
            {
                marker.gameObject.SetActive(false);
                Cursor.visible = true;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                mode = ToolMode.Dig;
                marker.Marker = ToolMarker.MarkerType.down;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                mode = ToolMode.Fill;
                marker.Marker = ToolMarker.MarkerType.up;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                mode = ToolMode.Flatten;
                marker.Marker = ToolMarker.MarkerType.flat;
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                faceNormalSample = hit.normal;
                diggingPointSample = hit.point;
            }

        }

        void AdjustMarkerPosition(RaycastHit hit)
        {
            marker.transform.forward = hit.normal;
        }

        private void StartAction(RaycastHit hit)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
            SetNeighbouringChunks(hit.point, radius, neighbouringChunks);

            foreach (Chunk chunk in neighbouringChunks)
            {
                switch (mode)
                {
                    case ToolMode.Dig:
                        Dig(chunk, hit, radius);
                        break;
                    case ToolMode.Fill:
                        Fill(chunk, hit, radius);
                        break;
                    case ToolMode.Flatten:
                        if (samplesSet)
                            Flatten(chunk, hit, radius);
                        else
                        {
                            faceNormalSample = hit.normal;
                            diggingPointSample = hit.point;
                            Debug.Log(diggingPointSample);
                            samplesSet = true;
                        }
                        break;
                }
                int threadGroupSize = 8;
                int numVoxelsPerAxis = chunk.numPointsPerAxis - 1;
                int numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float)threadGroupSize);
                if (map != null)
                    map.AlterTerrain(chunk);
                else
                {
                    terrain.pointsBuffer.SetData(chunk.scalarField);
                    terrain.GenerateChunkMesh(chunk, numThreadsPerAxis);
                    chunk.UpdateCollider();
                }
            }
        }
    }
}
