using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;


namespace gameracers.Inventory
{
    public class InventoryItem : MonoBehaviour, IPointerClickHandler
    {
        Image itemIcon;
        public CanvasGroup canvasGroup { get; private set; }

        public Item myItem { get; set; }
        public Slot activeSlot { get; set; }

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            itemIcon = GetComponent<Image>();
        }

        public void Initialize(Item item, Slot parent)
        {
            activeSlot = parent;
            activeSlot.myItem = this;
            myItem = item;
            itemIcon.sprite = item.sprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log(this.myItem.itemName);
                //InventorySystem.Singleton.SetCarriedItem(this);
            }
        }
    }
}
