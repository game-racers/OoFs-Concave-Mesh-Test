using gameracers.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener : MonoBehaviour
{
    public delegate void ChangeEquipment(SlotTag tag);
    public static event ChangeEquipment onChangeEquipment;

    public delegate void ChangeGameState(GameState newState);
    public static event ChangeGameState onChangeGameState;

    public static void EquipmentChange(SlotTag tag)
    {
        if (onChangeEquipment != null)
            onChangeEquipment(tag);
    }

    public static void GameStateChange(GameState newState)
    {
        if (onChangeGameState != null)
            onChangeGameState(newState);
    }
}
