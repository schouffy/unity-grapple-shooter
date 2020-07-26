public enum EventType
{
    CollectibleAcquired,
    SummonExtractionShip,
    PlayerHealthUpdated,
    PlayerScoreUpdated,
    GameOver,
    LevelEnd
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
