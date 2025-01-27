using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Assertions.Must;
using gameracers.MiniGolf.Control;
using TMPro;
using gameracers.MiniGolf.Aesthetics;
using Unity.VisualScripting;

namespace gameracers.MiniGolf.Core
{
    public class GolfGameManager : MonoBehaviour
    {
        [SerializeField] MiniGolfState gameState = MiniGolfState.GameStart;
        MiniGolfState lastState = MiniGolfState.GameStart;
        [SerializeField] GameObject pauseMenu;
        [SerializeField] Image blackScreen;
        [SerializeField] RectTransform scoreBoard;
        Transform playerScores;
        TextMeshProUGUI playerTotal;

        float startTimer = -Mathf.Infinity;
        [SerializeField] float ballEnterDur = 3.5f;
        [SerializeField] float ballEnterTimer;

        [SerializeField] float beginDur = 3f;
        [SerializeField] GameObject mainCam;

        Transform player;
        int currentHole = 0;
        int currentHoleScore;
        int totalScore;
        ClawMovement claw;
        bool canChangeHole = false;


        [SerializeField] List<GameObject> holes = new List<GameObject>();
        [SerializeField] GameObject mainLand;
        [SerializeField] GameObject hole3Cover;
        [SerializeField] GameObject hole8Cover;

        private void Start()
        {
            player = GameObject.FindWithTag("Player").transform;
            playerScores = scoreBoard.GetChild(0).Find("Player Scores");
            playerTotal = scoreBoard.GetChild(0).Find("Player Total").GetComponent<TextMeshProUGUI>();
            claw = GameObject.FindWithTag("Claw").GetComponent<ClawMovement>();
        }

        private void OnEnable()
        {
            GolfEventListener.onBallInHole += InBetweenHoles;
            GolfEventListener.onClawDrop += RoundStart;
        }

        private void OnDisable()
        {
            GolfEventListener.onBallInHole -= InBetweenHoles;
            GolfEventListener.onClawDrop -= RoundStart;
        }

        private void InBetweenHoles(Transform player)
        {
            currentHoleScore = player.GetComponent<MiniGolfPlayerController>().GetSwings();
            player.GetComponent<MiniGolfPlayerController>().ResetSwings();
            totalScore += currentHoleScore;
            UpdateScore();

            ChangeGameState(MiniGolfState.InBetween);
            ballEnterTimer = Time.time;
        }

        private void RoundStart()
        {
            ChangeGameState(MiniGolfState.MiniGolf);
        }

        private void UpdateScore()
        {
            playerScores.GetChild(currentHole).GetComponent<TextMeshProUGUI>().text = currentHoleScore.ToString();
            playerTotal.text = totalScore.ToString();
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
                    canChangeHole = true;
                    scoreBoard.gameObject.SetActive(true);
                    ballEnterTimer = Mathf.Infinity;
                }
                if (ballEnterTimer == Mathf.Infinity && Input.anyKeyDown)
                {
                    if (canChangeHole == true) 
                        NextHole();
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
            if (canChangeHole == false) return;
            holes[currentHole].SetActive(false);
            canChangeHole = false;
            currentHole += 1;
            holes[currentHole].SetActive(true);
            scoreBoard.gameObject.SetActive(false);

            claw.MoveToPos(holes[currentHole].transform.Find("Hole Start").position);

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