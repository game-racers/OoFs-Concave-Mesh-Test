using DG.Tweening;
using gameracers.MiniGolf.Core;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] float duration = 1f;
    float timer = Mathf.Infinity;
    [SerializeField] GameObject mainMenuVCam;
    [SerializeField] GameObject dressingVCam;
    [SerializeField] GameObject dialogueManager;
    [SerializeField] GameObject dressCanvas;
    [SerializeField] GameObject menuBG;
    [SerializeField] GameObject optionsTemp;
    [SerializeField] Image blackScreen;
    bool goTo;

    public void ExitGame()
    {
        Application.Quit();
    }

    public void DressingRoom(bool isEnter)
    {
        blackScreen.DOColor(Color.black, duration);
        goTo = isEnter;
        timer = Time.time;
    }

    private void Update()
    {
        if (timer != Mathf.Infinity)
        {
            if (Time.time - timer > duration)
            {
                if (goTo)
                {
                    mainMenuVCam.SetActive(false);
                    dressingVCam.SetActive(true);
                    dressCanvas.SetActive(true);
                }
                else
                {
                    dressCanvas.SetActive(false);
                    dressingVCam.SetActive(false);

                    if (GolfGameManager.ggm.GetGameState() == MiniGolfState.GameStart)
                    {
                        mainMenuVCam.SetActive(true);
                        menuBG.SetActive(true);
                        dialogueManager.SetActive(true);
                    }
                    else if (GolfGameManager.ggm.GetGameState() == MiniGolfState.PauseScreen)
                    {
                        optionsTemp.SetActive(true);
                    }
                }
                blackScreen.DOColor(Color.clear, duration);
                timer = Mathf.Infinity;
            }
        }
    }
}