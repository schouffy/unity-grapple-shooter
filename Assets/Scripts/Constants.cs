using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Constants
{
    public enum PoolTag
    {
        PlayerLaserBullet,
        TurretLaserBullet,
        PhysicsBullet
    }

    

    public static String PlayerTag => "Player";
    private static GameObject _playerObject = null;
    public static GameObject Player
    {
        get
        {
            if (_playerObject == null)
                _playerObject = GameObject.FindWithTag(PlayerTag);
            return _playerObject;
        }
    }
}
