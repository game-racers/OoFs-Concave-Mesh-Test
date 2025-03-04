using gameracers.Core;
using gameracers.Inventory;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using gameracers.MiniGolf.Core;
using System.Runtime.InteropServices;
using DG.Tweening;
using TMPro;
using System.Net;

namespace gameracers.MiniGolf.Control
{
    public class MiniGolfPlayerController : MonoBehaviour
    {
        Transform cam;
        Transform facing;
        Transform items;
        Vector3 lastDir;

        [SerializeField] Transform powerDisplay;
        Image powerBar;

        float power = 0;
        [SerializeField] float powerMod = 1f;
        [SerializeField] float powerMax = 5f;
        bool canSwing;
        bool roundOver = false;

        bool isPaused = true;

        bool isHazard = false;

        Rigidbody rb;
        SphereCollider sc;

        List<Equipment> equipment = new List<Equipment>();

        Vector3 lastPos;

        int swings;
        [SerializeField] TextMeshProUGUI strokeCounter;
        [SerializeField] Image strikeImage;

        private void OnEnable()
        {
            GolfEventListener.onChangeGameState += ChangingState;
        }

        private void OnDisable()
        {
            GolfEventListener.onChangeGameState -= ChangingState;
        }

        private void ChangingState(MiniGolfState newState)
        {
            if (newState == MiniGolfState.PauseScreen || newState == MiniGolfState.GameStart || newState == MiniGolfState.InBetween)
            {
                isPaused = true;
            }
            else if (newState == MiniGolfState.MiniGolf)
            {
                isPaused = false;
                FreezeCharacter(false);
                lastPos = transform.position;
            }
        }

        private void UpdateEquipment(SlotTag tag)
        {
            CheckInventory();
        }

        void Start()
        {
            cam = transform.GetChild(0).GetChild(0);
            //cam.gameObject.SetActive(false);
            powerBar = powerDisplay.GetChild(0).GetComponent<Image>();
            powerDisplay.gameObject.SetActive(false);
            facing = transform.Find("Facing");
            rb = GetComponent<Rigidbody>();
            sc = GetComponent<SphereCollider>();

            CheckInventory();
        }

        void Update()
        {
            if (isPaused == true) return;

            if (roundOver == true)
            {
                powerDisplay.gameObject.SetActive(false);

                return;
            }


            // update canStrike
            if (canSwing == false || !Input.GetMouseButton(0))
            {
                if (rb.velocity.magnitude > 2f)
                {
                    canSwing = false;
                    strikeImage.color = new Color(1, 1, 1, .25f);
                }
                else
                {
                    canSwing = true;
                    strikeImage.color = new Color(1, 1, 1, 1);
                }
            }

            // Button Press
            if (Input.GetMouseButtonDown(0))
            {
                if (rb.velocity.magnitude > 2f)
                {
                    canSwing = false;
                    return;
                }
                canSwing = true;
                powerDisplay.gameObject.SetActive(true);
                power = 0;
                powerBar.fillAmount = power;
            }

            // Button Hold
            if (Input.GetMouseButton(0))
            {
                if (!canSwing) return;
                power += Input.GetAxis("Mouse Y") * powerMod;
                power = Mathf.Min(power, powerMax);
                power = Mathf.Max(power, 0);
                powerBar.fillAmount = power / powerMax;
            }

            // Button Release
            if (Input.GetMouseButtonUp(0))
            {
                powerDisplay.gameObject.SetActive(false);
                if (!canSwing) return;
                if (Mathf.Approximately(power, 0f)) return;
                Vector3 facing = transform.position - cam.position;
                facing = new Vector3(facing.x, 0, facing.z);
                rb.AddForce(facing * power);
                AudioManager.am.PlayStrikeSound();
                lastPos = transform.position;
                canSwing = false;
                AddSwing(false);
            }
        }

        private void LateUpdate()
        {
            Vector3 temp = rb.velocity;
            if (temp.magnitude < .5f)
            {
                facing.localPosition = lastDir;
                return;
            }

            facing.localPosition = Vector3.Lerp(facing.localPosition, temp.normalized, .03f);
            if (temp.magnitude > 1f)
                lastDir = temp.normalized;

            foreach (Equipment item in equipment)
            {
                item.AimFace(facing.localPosition);
            }
        }

        public void CheckInventory()
        {
            // Updates visible inventory
            if (equipment.Count == 0) return;

            equipment.Clear();

            foreach (Transform child in transform.Find("Equipment"))
            {
                if (child.GetComponent<Equipment>() != null)
                {
                    equipment.Add(child.GetComponent<Equipment>());
                    child.gameObject.layer = LayerMask.NameToLayer("Player");
                }
            }
        }

        public void StopInput(bool doStop)
        {
            roundOver = doStop;
        }

        public void FreezeCharacter(bool doStop)
        {
            if (doStop)
            {
                rb.velocity = Vector3.zero;
                roundOver = doStop;
                rb.useGravity = !doStop;
                sc.enabled = !doStop;
            }
            else
            {
                rb.velocity = Vector3.zero;
                roundOver = doStop;
                rb.useGravity = !doStop;
                sc.enabled = !doStop;
            }
        }

        public void SetHazard()
        {
            isHazard = true;
        }

        public bool GetHazard()
        {
            return isHazard;
        }

        public void ResetHazard()
        {
            isHazard = false;
        }


        public void ChangeSensitivity(float val)
        {
            powerMod = val;
        }

        public int GetSwings()
        {
            return swings;
        }

        public void AddSwing(bool doReturnToLastPos)
        {
            swings += 1;
            strokeCounter.text = swings.ToString();
            if (doReturnToLastPos)
            {
                transform.position = lastPos;
                StopInput(false);
            }
        }

        public void ReturnToLastPos(bool addSwing)
        {
            if (addSwing)
                AddSwing(false);

            transform.position = lastPos;
            StopInput(false);
        }

        public void ResetSwings()
        {
            swings = 0;
            strokeCounter.text = "0";
        }
    }
}