using gameracers.Core;
using gameracers.MiniGolf.Core;
using UnityEngine;

public class GolfEventListener : MonoBehaviour
{
    public delegate void BallInHole(Transform player);
    public static event BallInHole onBallInHole;

    public delegate void ChangeGameState(MiniGolfState newState);
    public static event ChangeGameState onChangeGameState;

    public delegate void ClawDrop();
    public static event ClawDrop onClawDrop;

    public static void BallSunkInHole(Transform player)
    {
        if (onBallInHole != null)
            onBallInHole(player);
    }

    public static void GameStateChange(MiniGolfState newState)
    {
        if (onChangeGameState != null)
            onChangeGameState(newState);
    }

    public static void RoundStart()
    {
        if (onClawDrop != null)
            onClawDrop();
    }
}
