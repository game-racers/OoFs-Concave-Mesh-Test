using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Assertions.Must;

namespace gameracers.MiniGolf.Core
{
    public class GolfGameManager : MonoBehaviour
    {
        MiniGolfState gameState = MiniGolfState.GameStart;
        MiniGolfState lastState = MiniGolfState.GameStart;
        [SerializeField] GameObject pauseMenu;
        [SerializeField] Image blackScreen;
        [SerializeField] GameObject scoreBoard;

        float startTimer = -Mathf.Infinity;
        [SerializeField] float ballEnterDur = 3.5f;
        [SerializeField] float ballEnterTimer;

        [SerializeField] float beginDur = 3f;
        [SerializeField] GameObject mainCam;

        Transform player;
        int currentHole = 0;

        [SerializeField] List<GameObject> holes = new List<GameObject>();
        [SerializeField] GameObject mainLand;
        [SerializeField] GameObject hole3Cover;
        [SerializeField] GameObject hole8Cover;

        private void Start()
        {
            player = GameObject.FindWithTag("Player").transform;
        }

        private void OnEnable()
        {
            GolfEventListener.onBallInHole += InBetweenHoles;
        }

        private void OnDisable()
        {
            GolfEventListener.onBallInHole -= InBetweenHoles;
        }

        private void InBetweenHoles(Transform player)
        {
            ChangeGameState(MiniGolfState.InBetween);
            ballEnterTimer = Time.time;
        }

        private void Update()
        {
            if (startTimer != -Mathf.Infinity)
            {
                if (Time.time - startTimer > beginDur)
                {
                    //disable main camera
                    mainCam.SetActive(false);
                    //playercam fades away from black
                    blackScreen.DOColor(Color.clear, beginDur);
                    ChangeGameState(MiniGolfState.MiniGolf);
                    startTimer = -Mathf.Infinity;
                }
            }

            if (gameState == MiniGolfState.InBetween)
            {
                if (Time.time - ballEnterTimer > ballEnterDur)
                {
                    scoreBoard.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    ballEnterTimer = Mathf.Infinity;
                }
                return;
            }    

            if (gameState == MiniGolfState.GameStart) return;

            if (Input.GetButtonDown("Cancel"))
            {
                if (gameState == MiniGolfState.PauseScreen)
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
                    ChangeGameState(MiniGolfState.PauseScreen);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    pauseMenu.SetActive(true);
                    return;
                }
            }
        }

        public void BeginGame()
        {
            // Fade screen to black
            blackScreen.DOColor(Color.black, beginDur);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // voice lines

            startTimer = Time.time;
        }

        public void NextHole()
        {
            // move player by moving claw
            holes[currentHole].SetActive(false);
            currentHole += 1;
            holes[currentHole].SetActive(true);

            if (currentHole == 3 - 1)
            {
                hole3Cover.SetActive(false);
            }
            if (currentHole > 8 - 1)
            {
                mainLand.SetActive(false);
            }
            if (currentHole == 9 - 1)
            {
                hole8Cover.SetActive(true);
            }
        }

        public void ChangeGameState(MiniGolfState newState)
        {
            gameState = newState;
            GolfEventListener.GameStateChange(newState);
        }
    }

    public enum MiniGolfState
    {
        GameStart,
        PauseScreen,
        MiniGolf,
        InBetween
    }
}