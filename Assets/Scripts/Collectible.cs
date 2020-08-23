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
            EventManager.TriggerEvent(EventType.CollectibleAcquired, new IntegerEventParam { Value = Value });
            GetComponent<AudioSource>().Play();
            GetComponent<Collider>().enabled = false;
            GetComponentInChildren<Renderer>().enabled = false;
            Destroy(gameObject, 2f);
        }
    }
}
