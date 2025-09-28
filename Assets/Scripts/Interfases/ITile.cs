using UnityEngine;

public interface ITile
{
    public Vector2Int Position { get; set; }
    public TileType TileType { get; }
    public bool IsOccupied { get; set; }
    public bool IsWalkable { get; }
    public bool IsAttackThrough { get; }
    public int CostToWalk { get; }

    public void SetTileData(TileDataSO tileData);
    public float GetTileHeight();
}
