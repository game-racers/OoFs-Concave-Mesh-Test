using gameracers.Movement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using gameracers.Inventory;
using gameracers.Core;

namespace gameracers.Control
{
    public class PlayerController : MonoBehaviour
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

        bool isPaused = false;

        BallMover mover;
        Rigidbody rb;

        List<Equipment> equipment = new List<Equipment>();

        int swings;

        private void OnEnable()
        {
            EventListener.onChangeGameState += ChangingState;
            EventListener.onChangeEquipment += UpdateEquipment;
        }

        private void OnDisable()
        {
            EventListener.onChangeGameState -= ChangingState;
            EventListener.onChangeEquipment -= UpdateEquipment;
        }

        private void ChangingState(GameState newState)
        {
            if (newState == GameState.PauseScreen)
            {
                isPaused = true;
            }
            else if (newState == GameState.FreeRoam || newState == GameState.MiniGolf)
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
            mover = GetComponent<BallMover>();
            powerBar = powerDisplay.GetChild(0).GetComponent<Image>();
            powerDisplay.gameObject.SetActive(false);
            facing = transform.Find("Facing");
            rb = GetComponent<Rigidbody>();

            CheckInventory();
        }

        void Update()
        {
            if (isPaused == true) return;

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
                if (!canSwing) return;
                if (Mathf.Approximately(power, 0f)) return;
                powerDisplay.gameObject.SetActive(false);
                Vector3 facing = transform.position - cam.position;
                facing = new Vector3(facing.x, 0, facing.z);
                rb.AddForce(facing * power);
                canSwing = false;
                swings += 1;
            }
        }

        private void LateUpdate()
        {
            Vector3 temp = mover.GetVelocity();
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
            mover.ResetVelocity();
        }
    }
}
