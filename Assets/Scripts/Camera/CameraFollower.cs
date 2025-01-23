using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using gameracers.MiniGolf.Core;
using UnityEngine.VFX;

namespace gameracers.Camera
{
    public class CameraFollower : MonoBehaviour
    {
        [SerializeField] float maxDist = 7f;
        [SerializeField] float minDist = 3f;
        [SerializeField] float scrollMult = 1f;
        float startingHeight;

        [SerializeField] float mouseXRotMod = 1f;
        [SerializeField] float mouseYRotMod = 1f;

        [SerializeField] float maxForwardTilt = 315f;
        [SerializeField] float maxBackwardTilt = 45f;

        float deltaX;
        float deltaY;

        Transform player;
        Transform cam;

        bool doMove = false;


        private void OnEnable()
        {
            GolfEventListener.onChangeGameState += ToggleInput;
        }

        private void OnDisable()
        {
            GolfEventListener.onChangeGameState -= ToggleInput;
        }

        private void ToggleInput(MiniGolfState newState)
        {
            switch (newState)
            {
                case MiniGolfState.GameStart:
                    doMove = false;
                    break;
                case MiniGolfState.PauseScreen:
                    doMove = false;
                    break;
                case MiniGolfState.MiniGolf:
                    doMove = true;
                    break;
                case MiniGolfState.InBetween:
                    doMove = true;
                    break;
            }
        }

        void Start()
        {
            player = GameObject.FindWithTag("Player").transform;
            cam = player.GetChild(0);
            startingHeight = cam.localPosition.y;
        }

        void LateUpdate()
        {
            if (doMove == false) return;

            #region Zoom In and Out
            if (cam.localPosition.z < -minDist && Input.mouseScrollDelta.y > 0)
            {
                cam.localPosition += Vector3.forward * scrollMult * Time.deltaTime;
                if (cam.localPosition.z > -minDist)
                {
                    cam.localPosition = new Vector3(0f, startingHeight, -minDist);
                }
            }
            else if (cam.localPosition.z > -maxDist && Input.mouseScrollDelta.y < 0)
            {
                cam.localPosition -= Vector3.forward * scrollMult * Time.deltaTime;
                if (cam.localPosition.z < -maxDist)
                {
                    cam.localPosition = new Vector3(0f, startingHeight, -maxDist);
                }
            }
            #endregion

            #region Look Left and Right
            deltaX = Input.GetAxis("Mouse X") * mouseXRotMod * -1;
            transform.RotateAround(transform.position, Vector3.up, deltaX);
            #endregion

            #region Look Up and Down
            deltaY = Input.GetAxis("Mouse Y") * mouseYRotMod * -1;

            if (Input.GetMouseButton(0) == false)
            {
                if (transform.eulerAngles.x + deltaY < maxForwardTilt && transform.eulerAngles.x > 180)
                {
                    transform.eulerAngles = new Vector3(maxForwardTilt, transform.eulerAngles.y, 0);
                }
                else if (transform.eulerAngles.x + deltaY > maxBackwardTilt && transform.eulerAngles.x < 180f)
                {
                    transform.eulerAngles = new Vector3(maxBackwardTilt, transform.eulerAngles.y, 0);
                }
                else
                    transform.RotateAround(transform.position, transform.right, deltaY);
            }
            #endregion
        }
    }

}
