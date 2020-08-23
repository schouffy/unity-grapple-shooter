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
        PhysicsBullet,
        LaserGenericImpact,
        ReboundPlayerLaserBullet
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

    public static String ShipTag => "ExtractionShip";
    private static GameObject _shipObject = null;
    public static GameObject Ship
    {
        get
        {
            if (_shipObject == null)
                _shipObject = GameObject.FindWithTag(ShipTag);
            return _shipObject;
        }
    }

    public class PlayerPrefs
    {
        public static String MouseSensitivityX => "MouseSensitivityX";
        public static String MouseSensitivityY => "MouseSensitivityY";
    }
}

public enum EnemyStatus
{
    Idle,
    Attacking
}
