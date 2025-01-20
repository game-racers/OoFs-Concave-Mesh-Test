using gameracers.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace gameracers.Inventory
{
    public class InventorySystem : MonoBehaviour
    {
        //This will instead attach to each unique player, going into their own identities
        [SerializeField] Slot[] slots;
        [SerializeField] Slot[] equipmentSlots;
        [SerializeField] Transform characterEquipment;
        [SerializeField] Transform dragRef;
        [SerializeField] InventoryItem itemPrefab;

        GameObject[] equipment; //[0] is armour, [1] is weapon, [2] is ring

        InventoryItem carriedItem;
        Slot lastSlot;


        [Header("Debug Stuff")]
        [SerializeField] Item[] items;
        [SerializeField] Button giveItemBtn;

        private void OnEnable()
        {
            EventListener.onChangeGameState += PauseMenu;
        }

        private void OnDisable()
        {
            EventListener.onChangeGameState -= PauseMenu;
        }

        private void PauseMenu(GameState newState)
        {
            // remove carried item
            if (newState != GameState.PauseScreen)
            {
                if (carriedItem == null) return;

                if (carriedItem.activeSlot.myItem.myItem.itemName == null) return;
                carriedItem.activeSlot = lastSlot;
                carriedItem = null;
                lastSlot = null;
            }
        }

        public void SpawnInventoryItem(Item item = null)
        {
            Item _item = item;
            if (_item == null) //debug purposes
            {
                int random = Random.Range(0, items.Length);
                _item = items[random];
            }

            for (int i = 0; i < slots.Length; i++)
            {
                // check for an empty slot
                if (slots[i].myItem == null)
                {
                    Instantiate(itemPrefab, slots[i].transform).Initialize(_item, slots[i]);
                    break;
                }
            }
        }

        private void Update()
        {
            if (carriedItem == null) return;

            carriedItem.transform.position = Input.mousePosition;
            // TODO if using controller, follows the line
        }

        // i think we need to redo this, SetCarriedItem to Select and change 
        public void SetCarriedItem(InventoryItem newItem)
        {
            /* Sets the new item to be the carried item
             * 
             * 
             * 
             */

            // if already carrying an item, place the carried item into the new item's slot
            if (carriedItem != null)
            {
                // Test if the carried item can fit in the newItem slot
                if (newItem.activeSlot.myTag == SlotTag.None || carriedItem.myItem.itemTag == newItem.activeSlot.myTag)
                {
                    newItem.activeSlot.SetItem(carriedItem);

                    // Doff the old equipment
                    if (newItem.activeSlot.myTag != SlotTag.None)
                    {
                        EquipEquipment(newItem.activeSlot.myTag);
                    }
                }
            }


            if (newItem.myItem.itemTag != SlotTag.None)
            {
                // This doffs the item
                // maybe a switch case?
            }

            carriedItem = newItem;
            EventListener.EquipmentChange(carriedItem.myItem.itemTag);
            carriedItem.canvasGroup.blocksRaycasts = false;
            newItem.transform.SetParent(dragRef);
        }

        private void EquipEquipment(SlotTag tag)
        {
            EventListener.EquipmentChange(tag);

            switch (tag)
            {
                case SlotTag.Armour:
                    if (equipment[0] != null)
                    {
                        // Destroy the item that is there and remove the stat buffs
                        Debug.Log("Unequipped " + equipment[0].name + " on " + tag);
                        // TODO stats.removeBuffFrom(slot number or something like that);
                        Destroy(equipment[0].gameObject);
                        equipment[0] = null;
                    }
                    else
                    {
                        // Instantiate the item on the respective character inventory. 
                        if (equipmentSlots[0] == null) return;

                        if (equipmentSlots[0].myItem.myItem.equipmentPrefab != null)
                        {
                            equipment[0] = Instantiate(equipmentSlots[0].myItem.myItem.equipmentPrefab, characterEquipment).gameObject;
                            Debug.Log("Equipped " + equipment[0].name + " on " + tag);
                        }
                    }
                    break;
                case SlotTag.Weapon:
                    if (equipment[1] != null)
                    {
                        // Destroy the item that is there and remove the stat buffs
                        Debug.Log("Unequipped " + equipment[1].name + " on " + tag);
                        // TODO stats.removeBuffFrom(slot number or something like that);
                        Destroy(equipment[1].gameObject);
                        equipment[1] = null;
                    }
                    else
                    {
                        // Instantiate the item on the respective character inventory. 
                        if (equipmentSlots[1] == null) return;

                        if (equipmentSlots[1].myItem.myItem.equipmentPrefab != null)
                        {
                            equipment[1] = Instantiate(equipmentSlots[1].myItem.myItem.equipmentPrefab, characterEquipment).gameObject;
                            Debug.Log("Equipped " + equipment[1].name + " on " + tag);
                        }
                    }
                    break;
                case SlotTag.Accessory:
                    if (equipment[2] != null)
                    {
                        // Destroy the item that is there and remove the stat buffs
                        Debug.Log("Unequipped " + equipment[1].name + " on " + tag);
                        // TODO stats.removeBuffFrom(slot number or something like that);
                        Destroy(equipment[1].gameObject);
                        equipment[1] = null;
                    }
                    else
                    {
                        // Instantiate the item on the respective character inventory. 
                        if (equipmentSlots[1] == null) return;

                        if (equipmentSlots[1].myItem.myItem.equipmentPrefab != null)
                        {
                            equipment[1] = Instantiate(equipmentSlots[1].myItem.myItem.equipmentPrefab, characterEquipment).gameObject;
                            Debug.Log("Equipped " + equipment[1].name + " on " + tag);
                        }
                    }
                    break;
            }
        }

        public InventoryItem GetCarriedItem()
        {
            return carriedItem;
        }
    }
}
