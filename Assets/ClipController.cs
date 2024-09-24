using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;
using Utilities;
using UnityEditor.Rendering;
using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections;


namespace Psychonaut
{
    public class ClipController : MonoBehaviour
    {
        PlayerController playerController;

        private void Awake()
        {
            // Get the PlayerController from the parent
            playerController = GetComponentInParent<PlayerController>();
            if(playerController != null )
            {
                Debug.Log("FoundTheScript");
            }
        }

        void OnTriggerStay()
        {
            Debug.Log("Whatsup");
            if (playerController != null)
            {
                
            }
            else
            {
                Debug.LogWarning("PlayerController not found in parent.");
            }
        }
    }
}

