using DG.Tweening;
using gameracers.MiniGolf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.MiniGolf.Aesthetics
{
    public class Hole8Cutscene : MonoBehaviour
    {
        [SerializeField] Transform mainIsland;
        [SerializeField] List<Transform> brokenIslandCutsceneBits = new List<Transform>();
        [SerializeField] List<Transform> finalBrokenIslandBits = new List<Transform>();
        [SerializeField] Transform tempVolcano;
        [SerializeField] Transform finalVolcano;


        [SerializeField] float stageOneDur = 3f;
        bool onStage2 = false;
        [SerializeField] float stageTwoDur = 5f;
        bool ending = false;
        float timer = Mathf.Infinity;

        [SerializeField] float speed = 5.0f;
        [SerializeField] float intensity = 0.1f;

        private void OnEnable()
        {
            GolfEventListener.onChangeGameState += PerformEarthquake;
        }

        private void OnDisable()
        {
            GolfEventListener.onChangeGameState -= PerformEarthquake;
        }

        private void PerformEarthquake(MiniGolfState state)
        {
            if (state == MiniGolfState.MiniGolf)
            {
                mainIsland.parent.gameObject.SetActive(false);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            timer = Time.time;
            // each section is already vibrating aggressively due to the Earthquake Script that is enabled on Start();
            // have the main island sink down and the 4 parts rise slowly, once that is done, have the break apart into where they belong
            mainIsland.DOLocalMoveY(-.5f, stageOneDur).SetEase(Ease.InQuad);
            foreach (Transform child in brokenIslandCutsceneBits)
            {
                child.DOLocalMoveY(0, stageOneDur).SetEase(Ease.OutQuad);
            }
            tempVolcano.DOLocalMoveY(-0.7550011f, stageOneDur + stageTwoDur).SetEase(Ease.InOutSine);

            
        }

        // Update is called once per frame
        void Update()
        {
            if (ending == true && Time.time - timer > stageOneDur + stageTwoDur + 1f)
            {
                foreach (Transform child in finalBrokenIslandBits)
                    child.gameObject.SetActive(true);
                finalVolcano.gameObject.SetActive(true);
                timer = Mathf.Infinity;
                this.gameObject.SetActive(false);
            }

            if (ending == false && Time.time - timer > stageOneDur + stageTwoDur)
            {
                mainIsland.gameObject.SetActive(false);
                foreach (Transform child in brokenIslandCutsceneBits)
                    child.GetComponent<EarthQuake>().StopQuake();
                tempVolcano.GetComponent<EarthQuake>().StopQuake();
                ending = true;
            }

            if (onStage2 == false && Time.time - timer > stageOneDur)
            {
                onStage2 = true;
                // after like 3 seconds, have the island break apart with the volcano rising
                for (int i = 0; i < brokenIslandCutsceneBits.Count; i++)
                {
                    brokenIslandCutsceneBits[i].DOLocalMove(finalBrokenIslandBits[i].localPosition, stageTwoDur).SetEase(Ease.InOutQuad);
                    brokenIslandCutsceneBits[i].DOLocalRotate(finalBrokenIslandBits[i].localEulerAngles, stageTwoDur).SetEase(Ease.InOutQuad);
                }
            }
        }
    }
}
