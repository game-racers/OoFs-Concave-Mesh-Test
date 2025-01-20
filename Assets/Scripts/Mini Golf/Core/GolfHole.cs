using DG.Tweening;
using gameracers.Control;
using gameracers.MiniGolf.Aesthetics;
using gameracers.MiniGolf.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.MiniGolf.Core
{
    public class GolfHole : MonoBehaviour
    {
        [SerializeField] ClawMovement claw;
        [SerializeField] float moveDur = 1f;
        [SerializeField] float clawVertMovement = 10f;
        float timer;
        bool isEndLevel = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<MiniGolfPlayerController>() == null) return;

            MiniGolfPlayerController pc = other.GetComponent<MiniGolfPlayerController>();

            pc.StopInput(true);
            claw.gameObject.SetActive(true);
            claw.transform.position = transform.parent.Find("Claw Spawn").position;
            claw.transform.DOMove(claw.transform.position + Vector3.down * clawVertMovement, moveDur);
            claw.OpenClaw();
            timer = Time.time;
            isEndLevel = true;
        }

        private void Update()
        {
            if (isEndLevel == true)
            {
                if (Time.time - timer > moveDur)
                {
                    claw.CloseClaw();
                    claw.GrabTransform(GameObject.FindWithTag("Player").transform);
                    claw.transform.DOMove(claw.transform.position + Vector3.up * clawVertMovement, moveDur);
                    //scoreBoard.gameObject.SetActive(true);
                }
            }
        }
    }
}
