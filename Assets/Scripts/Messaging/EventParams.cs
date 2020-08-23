using UnityEngine;

public enum EventType
{
    CollectibleAcquired,
    SummonExtractionShip,
    PlayerHealthUpdated,
    PlayerScoreUpdated,
    GameOver,
    LevelEnd,
    CheckpointReached,
    ExplosionNearby,
    TogglePause,
    PlayerPrefsUpdated
}

public abstract class EventParam
{
}

public class CollectibleEventParam : EventParam
{
    public Collectible Collectible { get; set; }
}

public class IntegerEventParam : EventParam
{
    public int Value { get; set; }
}

public class HealthEventParam : EventParam
{
    public int Health { get; set; }
    public Vector3? ProjectileDirection { get; set; }
}

public class CheckpointReachedEventParam : EventParam
{
    public Vector3 RespawnPosition { get; set; }
}

public class ExplosionNearbyEventParam : EventParam
{
    public Vector3 Position { get; set; }
}

public class GameOverEventParam : EventParam
{
    public float FadeToBlackTime { get; set; }
}

public class EndLevelEventParam : EventParam
{
    public float FadeToBlackTime { get; set; }
}
