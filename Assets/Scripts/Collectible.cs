using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int Value = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Constants.PlayerTag)
        {
            Constants.Player.GetComponent<PlayerCollectibles>().Collect(this);
            Destroy(gameObject);
        }
    }
}
