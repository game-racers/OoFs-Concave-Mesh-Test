using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using gameracers.MiniGolf.Control;
using TMPro;
using gameracers.MiniGolf.Aesthetics;
using gameracers.Dialogue;
using Cinemachine;

namespace gameracers.MiniGolf.Core
{
    public class GolfGameManager : MonoBehaviour
    {
        public static GolfGameManager ggm;

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
        [SerializeField] GameObject mainMenuVCam;

        Transform player;
        int currentHole = 0;
        int currentHoleScore;
        int totalScore;
        ClawMovement claw;
        bool canChangeHole = false;


        [SerializeField] List<GameObject> holes = new List<GameObject>();
        [SerializeField] GameObject mainLand;
        [SerializeField] GameObject hole3Cover;

        [SerializeField] float cutsceneDur = 10f;
        [SerializeField] float cutsceneTransitionDur = 1f;
        bool didCutsceneStart = false;
        bool didCutsceneFade;
        float cutsceneTimer = Mathf.Infinity;
        CinemachineVirtualCamera cutsceneCam;

        bool endGame = false;

        private void Awake()
        {
            if (ggm == null)
                ggm = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            //Application.targetFrameRate = 60;

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

        private void InBetweenHoles(Transform entity)
        {
            if (!GameObject.ReferenceEquals(player.gameObject, entity.gameObject))
                endGame = true;
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

        private void ResetScore()
        {
            foreach (Transform child in playerScores)
            {
                child.GetComponent<TextMeshProUGUI>().text = "";
            }

            playerTotal.text = "";
        }

        private void Update()
        {
            if (endGame == true)
            {
                // reveal Game Over Screen
                ChangeGameState(MiniGolfState.EndScreen);
                UpdateScore();
                scoreBoard.gameObject.SetActive(true);
                mainLand.SetActive(true);
                return;
            }

            switch (gameState)
            {
                case (MiniGolfState.GameStart):

                    if (startTimer != -Mathf.Infinity)
                    {
                        if (Time.time - startTimer > beginDur)
                        {
                            // switch cams
                            mainMenuVCam.SetActive(false);

                            //playercam fades away from black
                            blackScreen.DOColor(Color.clear, beginDur);
                            DialogueManager.dm.NextHoleText(0);
                            ChangeGameState(MiniGolfState.Cutscene);
                            cutsceneCam = holes[currentHole].transform.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
                            cutsceneCam.gameObject.SetActive(true);
                            DOTween.To(() => cutsceneCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition, x => cutsceneCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = x, 1, cutsceneDur);
                            cutsceneTimer = Time.time;
                            startTimer = -Mathf.Infinity;
                        }
                    }

                    return;
                case (MiniGolfState.PauseScreen):
                    PauseGame();
                    break;
                case (MiniGolfState.MiniGolf):
                    PauseGame();
                    break;
                case (MiniGolfState.InBetween):

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
                    if (ballEnterTimer == Mathf.Infinity && currentHole == 9 - 1)
                    {
                        NextHole();
                    }

                    return;
                case (MiniGolfState.Cutscene):
                    if (Time.time - cutsceneTimer > cutsceneTransitionDur && didCutsceneStart == false) 
                    {
                        didCutsceneStart = true;
                        didCutsceneFade = false;
                        cutsceneCam = holes[currentHole].transform.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
                    }

                    if (Time.time - cutsceneTimer > cutsceneDur - cutsceneTransitionDur && didCutsceneFade == false)
                    {
                        didCutsceneFade = true;
                        blackScreen.DOColor(Color.black, cutsceneTransitionDur);
                    }

                    if (Time.time - cutsceneTimer > cutsceneDur)
                    {
                        cutsceneTimer = Mathf.Infinity;
                        cutsceneCam.gameObject.SetActive(false);
                        cutsceneCam = null;
                        blackScreen.DOColor(Color.clear, cutsceneDur);
                        ChangeGameState(MiniGolfState.MiniGolf);
                    }

                    break;
                case (MiniGolfState.EndScreen):

                    if (gameState == MiniGolfState.EndScreen)
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
                            {
                                blackScreen.DOColor(Color.black, beginDur);
                                // Credits is already displayed due to the Play Button actually deactivates "Main Menu" and activates "Credits" under the Main Menu Canvas
                                blackScreen.DOColor(Color.clear, beginDur);
                                ChangeGameState(MiniGolfState.GameStart);
                                ResetScore();
                            }
                        }
                    }

                    break;
            }
        }

        private void PauseGame()
        {
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
            AudioManager.am.PlayDialogue(1);

            startTimer = Time.time;
        }

        public void NextHole()
        {
            // move player by moving claw
            if (endGame == true || currentHole == holes.Count)
            {
                // reveal Game Over Screen
                ChangeGameState(MiniGolfState.EndScreen);
                UpdateScore();
                scoreBoard.gameObject.SetActive(true);
                mainLand.SetActive(true);
                return;
            }

            if (canChangeHole == false) return;
            holes[currentHole].SetActive(false);
            canChangeHole = false;
            currentHole += 1;

            if (currentHole == holes.Count)
            {
                // reveal Game Over Screen
                ChangeGameState(MiniGolfState.EndScreen);
                UpdateScore();
                scoreBoard.gameObject.SetActive(true);
                mainLand.SetActive(true);
                mainLand.layer = LayerMask.NameToLayer("Grass");
                return;
            }

            AudioManager.am.PlayDialogue(currentHole + 1);
            holes[currentHole].SetActive(true);
            scoreBoard.gameObject.SetActive(false);

            // Cutscene Stuff
            claw.MoveToPos(holes[currentHole].transform.Find("Hole Start").position);
            ChangeGameState(MiniGolfState.Cutscene);
            cutsceneTimer = Time.time;
            blackScreen.DOColor(Color.black, 1f).SetLoops(1, LoopType.Yoyo);
            didCutsceneStart = false;
            cutsceneCam = holes[currentHole].transform.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
            DOTween.To(() => cutsceneCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition, x => cutsceneCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = x, 1, cutsceneDur);

            switch (currentHole + 1)
            {
                case 1:
                    mainLand.layer = LayerMask.NameToLayer("Out of Bounds");
                    hole3Cover.SetActive(true);
                    break;
                case 2:
                    mainLand.layer = LayerMask.NameToLayer("Grass");
                    hole3Cover.SetActive(true);
                    break;
                case 3:
                    mainLand.layer = LayerMask.NameToLayer("Grass");
                    hole3Cover.SetActive(false);
                    break;
                case 4:
                    mainLand.layer = LayerMask.NameToLayer("Out of Bounds");
                    break;
                case 5:
                    mainLand.layer = LayerMask.NameToLayer("Out of Bounds");
                    break;
                case 6:
                    mainLand.layer = LayerMask.NameToLayer("Out of Bounds");
                    break;
                case 7:
                    mainLand.layer = LayerMask.NameToLayer("Out of Bounds");
                    break;
                case 8:
                    mainLand.SetActive(false);
                    break;
                case 9:
                    mainLand.SetActive(false);
                    break;
            }
        }

        public void ChangeGameState(MiniGolfState newState)
        {
            gameState = newState;
            GolfEventListener.GameStateChange(newState);
        }

        public MiniGolfState GetGameState()
        {
            return gameState;
        }

        public void ResetHole()
        {
            /*  This function is used by the cheats menu. Therefore it is never called in script. 
             *  Resets player's swings and teleports them to the start position. 
             */

            player.GetComponent<MiniGolfPlayerController>().ResetSwings();

            player.position = holes[currentHole].transform.Find("Hole Start").position;
        }

        public void ChangeFPS(int val)
        {
            /*  This function is used by the cheats menu. Therefore it is never called in script. 
             *  Changes FPS of the game based off of the dropdown option in the options menu. 
             */

            switch (val)
            {
                case 0: // 30 fps
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = 30;
                    break;
                case 1: // 60 fps
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = 60;
                    break;
                case 2: // 120 fps
                    Application.targetFrameRate = 120;
                    break;
                case 3: // 240 fps
                    Application.targetFrameRate = 240;
                    break;
                case 4: // unlimitted
                    Application.targetFrameRate = 1000;
                    break;

            }
        }
    }

    public enum MiniGolfState
    {
        GameStart,
        PauseScreen,
        MiniGolf,
        InBetween, 
        Cutscene, 
        EndScreen
    }
}