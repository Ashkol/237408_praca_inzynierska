using UnityEngine;

namespace MiniAstro.Physics.SphericalGravity
{
    [RequireComponent(typeof(Light))]
    public class OrbitingBody : MonoBehaviour
    {
        [Tooltip("Day/night cycle time in minutes")]
        public float DayNightCycle = 2;
        public Transform planet;
        public Transform player;

        void Update()
        {
            Orbit();
        }

        void Orbit()
        {
            float angle = (360 / (DayNightCycle * 60)) * Time.deltaTime;
            transform.RotateAround(planet.position, planet.up, angle);
            transform.LookAt(player);
        }
    }
}