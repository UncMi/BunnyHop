using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psychonaut
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] float groundDistance = 0.1f;
        [SerializeField] LayerMask groundLayers;

        public bool IsGrounded { get; private set; }
        public float GroundDistance { get; private set; } = Mathf.Infinity;

        void FixedUpdate()
        {
            Debug.DrawRay(transform.position, Vector3.down * 10f, Color.red); // Extend the ray to make it more visible
            IsGrounded = Physics.CheckSphere(transform.position, groundDistance, groundLayers);
            GroundDistance = GetGroundDistance();
        }

        // Function to return the distance between the ground and the character
        public float GetGroundDistance()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayers))
            {
                return hit.distance; // Return the distance from the character to the ground
            }
            return Mathf.Infinity; // Return a very large number if no ground is hit
        }
    }
}
