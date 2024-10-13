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

        public bool getPausedState()
        {
            if (isPaused) return true;
            else return false;
        }
        void HandlePauseMenu()
        {
            if (Input.GetButtonDown("Escape"))
            {
                isPaused = !isPaused;

                if (isPaused)
                {
                    PauseMenu.SetActive(true);
                    Time.timeScale = 0f; 
                    Cursor.lockState = CursorLockMode.None; 
                    Cursor.visible = true; 
                }
                else
                {
                    PauseMenu.SetActive(false);
                    Time.timeScale = 1f; 
                    Cursor.lockState = CursorLockMode.Locked; 
                    Cursor.visible = false;
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

