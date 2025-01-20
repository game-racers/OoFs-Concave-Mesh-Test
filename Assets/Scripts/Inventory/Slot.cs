using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace gameracers.Inventory
{
    public class Slot : MonoBehaviour, IPointerClickHandler
    {
        public InventoryItem myItem { get; set; }
        public InventorySystem inventorySystem;

        public SlotTag myTag;

        private void Start()
        {
            inventorySystem = transform.parent.parent.Find("Inventory").GetComponent<InventorySystem>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (inventorySystem.GetCarriedItem() == null) return;
                if (myTag != SlotTag.None && inventorySystem.GetCarriedItem().myItem.itemTag != myTag) return;
                SetItem(inventorySystem.GetCarriedItem());
            }
        }

        public void SetItem(InventoryItem item)
        {
            // empties hand, but should check if the current slot already has an item, therefore doffs the armour
            //inventorySystem.GetCarriedItem() = null;

            // Reset old slot
            item.activeSlot.myItem = null;
            EventListener.EquipmentChange(item.myItem.itemTag);

            // Set current slot
            myItem = item;
            myItem.activeSlot = this;
            myItem.transform.SetParent(transform);
            myItem.canvasGroup.blocksRaycasts = true;

            if (myTag != SlotTag.None)
            {
                Debug.Log("Order Test from Slot");
                //InventorySystem.Singleton.EquipEquipment(myTag, myItem);
            }
        }
    }
}
