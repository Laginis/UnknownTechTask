public class TileClickedEvent
{
    public ITile Tile { get; private set; }
    public TileClickedEvent(ITile tile)
    {
        Tile = tile;
    }
}
