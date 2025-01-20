using UnityEngine;

public class GolfEventListener : MonoBehaviour
{
    public delegate void BallInHole(Transform player);
    public static event BallInHole onBallInHole;

    public static void BallSunkInHole(Transform player)
    {
        if (onBallInHole != null)
            onBallInHole(player);
    }
}
