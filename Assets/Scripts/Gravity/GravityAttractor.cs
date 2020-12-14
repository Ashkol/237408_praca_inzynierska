using UnityEngine;

namespace MiniAstro.Physics.SphericalGravity
{
    // Don't put more than one GravityAttractor on the scene
    public class GravityAttractor : MonoBehaviour
    {
        // negative value is pull towards the attractor
        public float gravity = -10f;

        public void Attract(Rigidbody rgbody, bool rotate)
        {
            Vector3 targetDir = (rgbody.transform.position - transform.position).normalized;
            Vector3 bodyUp = rgbody.transform.up;

            if (rotate)
                rgbody.MoveRotation(Quaternion.FromToRotation(bodyUp, targetDir) * rgbody.transform.rotation);
            rgbody.AddForce(targetDir * gravity);
        }
    }
}

