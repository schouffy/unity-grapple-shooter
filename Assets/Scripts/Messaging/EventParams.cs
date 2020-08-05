using UnityEngine;

public enum EventType
{
    CollectibleAcquired,
    SummonExtractionShip,
    PlayerHealthUpdated,
    PlayerScoreUpdated,
    GameOver,
    LevelEnd,
    CheckpointReached
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

public class CheckpointReachedEventParam : EventParam
{
    public Vector3 RespawnPosition { get; set; }
}
