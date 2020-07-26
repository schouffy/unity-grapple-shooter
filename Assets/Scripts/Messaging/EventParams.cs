public enum EventType
{
    GrabCollectible,
    GameOver,
    SummonExtractionShip
}

public abstract class EventParam
{
}

public class CollectibleEventParam : EventParam
{
    public Collectible Collectible { get; set; }
}
