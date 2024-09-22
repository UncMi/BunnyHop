using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psychonaut
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] float groundDistance = 0.5f;
        [SerializeField] LayerMask groundLayers;

        public bool IsGrounded { get; private set; }

        void Update()
        {
            Debug.DrawRay(transform.position, Vector3.down * 1f, Color.red);
            IsGrounded = Physics.CheckSphere(transform.position, groundDistance, groundLayers);
        }
    }
}