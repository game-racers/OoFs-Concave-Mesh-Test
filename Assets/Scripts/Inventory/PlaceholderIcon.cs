using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace gameracers.Inventory
{
    public class PlaceholderIcon : MonoBehaviour
    {
        [SerializeField] Slot mySlot;
        Image image;

        private void Start()
        {
            image = GetComponent<Image>();
        }

        private void OnEnable()
        {
            EventListener.onChangeEquipment += CheckSlot;
        }

        private void OnDisable()
        {
            EventListener.onChangeEquipment -= CheckSlot;
        }

        private void CheckSlot(SlotTag tag)
        {
            if (tag == mySlot.myTag)
            {
                if (mySlot.myItem != null)
                    image.enabled = false;
                else
                    image.enabled = true;
            }
        }
    }
}
