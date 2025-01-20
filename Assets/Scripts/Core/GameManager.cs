using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Core
{
    public class GameManager : MonoBehaviour
    {
        GameState gameState = GameState.FreeRoam;
        GameState lastState = GameState.FreeRoam;
        [SerializeField] GameObject pauseMenu;


        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                if (gameState == GameState.PauseScreen)
                {
                    ChangeGameState(lastState);
                    pauseMenu.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    return;
                }
                else
                {
                    lastState = gameState;
                    ChangeGameState(GameState.PauseScreen);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    pauseMenu.SetActive(true);
                    return;
                }
            }
        }

        public void ChangeGameState(GameState newState)
        {
            gameState = newState;
            EventListener.GameStateChange(newState);
        }
    }

    public enum GameState
    {
        FreeRoam,
        PauseScreen, 
        MiniGolf, 
        Combat,
    }
}
