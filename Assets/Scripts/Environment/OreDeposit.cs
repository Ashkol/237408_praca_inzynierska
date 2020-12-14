namespace MiniAstro.Environment
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class OreDeposit : MonoBehaviour
    {
        public MineralOre orePrefab;
        public int size = 5;

        public void Awake()
        {
            for (int i = 0; i < size; i++)
            {
                var ore = Instantiate(orePrefab, transform, false);
                ore.transform.localPosition = new Vector3(Random.Range(-2, 2), Random.Range(-1, 1), Random.Range(-2, 2));
            }
        }
    }
}