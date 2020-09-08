using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerTriggerZone : MonoBehaviour
{
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Constants.PlayerTag)
        {
            OnPlayerEnter(other.gameObject);
        }
    }


    public virtual void OnPlayerEnter(GameObject player)
    {
    }
}
