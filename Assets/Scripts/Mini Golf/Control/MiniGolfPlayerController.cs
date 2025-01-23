using gameracers.Core;
using gameracers.Inventory;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using gameracers.MiniGolf.Core;

namespace gameracers.MiniGolf.Control
{
    public class MiniGolfPlayerController : MonoBehaviour
    {
        Transform cam;
        Transform facing;
        Vector3 lastDir;

        [SerializeField] Transform powerDisplay;
        Image powerBar;

        float power = 0;
        [SerializeField] float powerMod = 1f;
        [SerializeField] float powerMax = 5f;
        bool canSwing;
        bool roundOver = false;

        bool isPaused = true;

        Rigidbody rb;

        List<Equipment> equipment = new List<Equipment>();

        int swings;

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
                canSwing = false;
                swings += 1;
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
            ResetVelocity();
            roundOver = doStop;
            rb.useGravity = !doStop;
        }

        public int GetSwings()
        {
            return swings;
        }

        public void AddSwing()
        {
            swings += 1;
        }

        public void ResetSwings()
        {
            swings = 0;
        }

        public void ResetVelocity()
        {
            rb.velocity = Vector3.zero;
        }
    }
}