namespace MiniAstro.Environment
{
    using UnityEngine;
    using MiniAstro.TerrainGeneration;

    public enum MineralOreType
    {
        Hematite,
        Malachite,
        Quartz
    }

    public class MineralOre : MonoBehaviour
    {
        Chunk chunk;
        MeshGenerator terrain;
        MouseTerrainTool terrainTool;
        public MineralOreType oreType = default;
        AudioSource audioSource;

        bool isBroken = false;

        void Start()
        {
            terrain = FindObjectOfType<MeshGenerator>();
            terrainTool = FindObjectOfType<MouseTerrainTool>();
            audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            // TODO It should NOT check it every damn frame
            if (CheckIfOutsideMesh() && !isBroken)
            {
                BreakOre();
            }
            if (isBroken)
            {
                if (GetComponentInChildren<MineralOrePiece>() == null)
                {
                    Destroy(gameObject, 1f);
                }
            }
        }

        void BreakOre()
        {
            var children = gameObject.GetComponentsInChildren<Transform>();
            float force = 4f;
            for (int i = 2; i < children.Length; i++)
            {
                children[i].gameObject.AddComponent<Rigidbody>();
                children[i].GetComponent<Rigidbody>().AddForce((children[i].position - transform.position).normalized * force, ForceMode.Impulse);
                children[i].GetComponent<Rigidbody>().useGravity = false;
                children[i].GetComponent<MeshCollider>().isTrigger = true;
                children[i].gameObject.AddComponent<MineralOrePiece>();
                children[i].GetComponent<MineralOrePiece>().oreType = this.oreType;
            }
            isBroken = true;
            audioSource.Play();
        }

        void FlyPiecesTowardsPlayer()
        {
            var children = gameObject.GetComponentsInChildren<Transform>();
            float force = 40f;
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            for (int i = 2; i < children.Length; i++)
            {
                children[i].GetComponent<Rigidbody>().AddForce(-(children[i].position - player.position).normalized * force, ForceMode.Force);
            }
        }

        /// <summary>
        /// Returns true only if object's origin is slightly outside of terrain mesh
        /// </summary>
        /// <returns></returns>
        bool CheckIfOutsideMesh()
        {
            // Ignore all layers <= 17, 18th layer is terrain mesh layer
            int raycastIgnoreMask = 1 << 18;

            RaycastHit hit;
            // Include terrain mesh and default layers
            terrainTool.CastRay(out hit, raycastIgnoreMask + 1);
            float distance = Vector3.Distance(transform.position, hit.point);
            float checkSphereRadius = 0.5f;
            // I check this distance so Physics.Ovelapsphere is not called on every single MineralOre on the scene
            if (distance < checkSphereRadius * 2)
            {
                return Physics.OverlapSphere(transform.position, checkSphereRadius, raycastIgnoreMask).Length == 0;
            }
            return false;
        }
    }
}
