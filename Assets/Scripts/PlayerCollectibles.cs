using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollectibles : MonoBehaviour
{
    public int Score;

    public void Collect(Collectible collectible)
    {
        Score += collectible.Value;
        Debug.Log("Collectible acquired. Score: " + Score);
    }
}
