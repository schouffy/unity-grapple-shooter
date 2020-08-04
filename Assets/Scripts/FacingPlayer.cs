using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingPlayer : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Constants.Player.transform.position);
    }
}
