using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace gameracers.MiniGolf.Aesthetics
{
    public class MoveFlag : MonoBehaviour
    {
        [SerializeField] Transform flag;

        bool doMoveFlag = false;

        [SerializeField] float moveDur = 1f;
        float timer;

        [SerializeField] float verticalMovement = 10f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                doMoveFlag = true;
                flag.DOLocalMove(Vector3.up * verticalMovement, moveDur);
                timer = Time.time;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                doMoveFlag = false;
                flag.gameObject.SetActive(true);
                flag.DOLocalMove(Vector3.zero, moveDur);
            }
        }

        private void Update()
        {
            if (!flag.gameObject.activeSelf) return;
            if (doMoveFlag == true)
            {
                if (Time.time - timer > moveDur)
                    flag.gameObject.SetActive(false);
            }
        }
    }
}
