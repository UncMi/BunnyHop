using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Psychonaut;


namespace Psychonaut
{
    public class GameManager : MonoBehaviour
    {

        public GameObject PauseMenu_MapEditor_Button;
        public TMP_Text PauseMenu_MapEditor_Text_OnOff;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Builder builder;
        [SerializeField] private BuildSystem buildSystem;
        [SerializeField] GameObject PauseMenu;
        [SerializeField] Rigidbody PlayerRb;

        private bool isBuildState = false;

        private bool isPaused = false;

        public bool getPauseState()
        {
            return isPaused;
        }
        private void Update()
        {
            HandlePauseMenu();
        }

        void HandlePauseMenu()
        {
            if (Input.GetButtonDown("Escape"))
            {
                isPaused = !isPaused; // Toggle the pause state

                if (isPaused)
                {
                    // Open the pause menu and pause the game
                    PauseMenu.SetActive(true);
                    Time.timeScale = 0f; // Pause the game
                    Cursor.lockState = CursorLockMode.None; // Unlock the cursor
                    Cursor.visible = true; // Make the cursor visible
                }
                else
                {
                    // Close the pause menu and resume the game
                    PauseMenu.SetActive(false);
                    Time.timeScale = 1f; // Resume the game
                    Cursor.lockState = CursorLockMode.Locked; // Lock the cursor again
                    Cursor.visible = false; // Hide the cursor
                }
            }
        }

        public void MapEditorButton()
        {

            if(!isBuildState) 
            {
                PauseMenu_MapEditor_Text_OnOff.text = "On";
                builder.enabled = true;
                buildSystem.enabled = true;
                playerController.enabled = false;
                PauseMenu.SetActive(false);

                PlayerRb.isKinematic = true; 
                PlayerRb.useGravity = false;

                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked; 
                Cursor.visible = false; 

                isBuildState = !isBuildState;
            }
            else if (isBuildState)
            {
                PauseMenu_MapEditor_Text_OnOff.text = "Off";
                builder.enabled = false;
                buildSystem.enabled = false;
                playerController.enabled = true;
                PauseMenu.SetActive(false);

                PlayerRb.isKinematic = false;
                PlayerRb.useGravity = true;

                Time.timeScale = 1f; 
                Cursor.lockState = CursorLockMode.Locked; 
                Cursor.visible = false;

                isBuildState = !isBuildState;
            }
        }
    }
}

