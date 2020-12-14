namespace MiniAstro.Environment
{
    using MiniAstro.Physics.SphericalGravity;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class OreDepositSpawner : MonoBehaviour
    {
        public List<OreDeposit> depositPrefabs = new List<OreDeposit>();
        public float radius = 25;
        public float quantity = 25;

        void Awake()
        {
            SceneManager.activeSceneChanged += OnLevelFinishedLoading;
        }

        void OnLevelFinishedLoading(Scene previousScene, Scene newScene)
        {
            SpawnOreDeposits();
        }

        void Start()
        {
            SceneManager.activeSceneChanged -= OnLevelFinishedLoading;
        }

        void SpawnOreDeposits()
        {
            Transform planet = FindObjectOfType<GravityAttractor>().transform;
            for (int i = 0; i < quantity; i++)
            {
                var deposit = Instantiate(depositPrefabs[Random.Range(0, depositPrefabs.Count)]);
                deposit.transform.parent = transform;
                float sigma = Random.Range(-90, 90);
                float phi = Random.Range(-90, 90);
                float x = radius * Mathf.Sin(phi) * Mathf.Cos(sigma);
                float y = radius * Mathf.Sin(phi) * Mathf.Sin(sigma);
                float z = radius * Mathf.Cos(phi);
                deposit.transform.position = new Vector3(x, y, z);
                Vector3 direction = (deposit.transform.position - planet.position).normalized;
                deposit.transform.up = direction;
                //deposit.transform.rotation = Quaternion.LookRotation(Vector3.Cross(, direction);
            }
        }
    }

}
