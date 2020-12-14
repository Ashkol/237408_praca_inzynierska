using UnityEngine;

namespace MiniAstro.Physics.SphericalGravity
{
    [RequireComponent(typeof(Rigidbody))]
    public class GravityBody : MonoBehaviour
    {
        public bool freezeRotation = true;
        GravityAttractor planet;
        Rigidbody rgbody;

        void Awake()
        {
            planet = FindObjectOfType<GravityAttractor>();
            rgbody = GetComponent<Rigidbody>();
            rgbody.useGravity = false;
            planet.Attract(rgbody, false);
            if (freezeRotation)
                rgbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        void FixedUpdate()
        {
            planet.Attract(rgbody, false);
        }

        void OnValidate()
        {
            if (rgbody != null)
            {
                if (freezeRotation)
                    rgbody.constraints = RigidbodyConstraints.FreezeRotation;
                else
                    rgbody.constraints = RigidbodyConstraints.None;
            }
        }
    }
}