using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnGameOverMonoBehaviour : MonoBehaviour
{
    public virtual void OnEnable()
    {
        EventManager.StartListening(EventType.GameOver, (p) => this.GameOver());
    }

    public virtual void OnDisable()
    {
        EventManager.StopListening(EventType.GameOver, (p) => this.GameOver());
    }

    protected virtual void GameOver()
    {
        enabled = false;
    }
}
