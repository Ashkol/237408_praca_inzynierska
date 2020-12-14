namespace MiniAstro.Environment
{
    using UnityEngine;
    using MiniAstro.Management;

    public class MineralOrePiece : MonoBehaviour
    {
        public MineralOreType oreType = default;


        void Awake()
        {
            LeanTween.scale(gameObject, Vector3.one * 0.5f, 1f);
            Destroy(gameObject, Random.Range(1f, 1.5f));
        }

        void Update()
        {
            float force = 40f;
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            GetComponent<Rigidbody>().AddForce(-(transform.position - player.position).normalized * force, ForceMode.Force);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            ScoreManager.instance.Add(oreType, 1);
        }
    }
}
