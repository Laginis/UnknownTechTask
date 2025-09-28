using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapDataHandler : MonoBehaviour
{
    private readonly Dictionary<Vector2Int, ITile> tiles = new();
    private readonly Dictionary<Vector2Int, IEntity> entities = new();
    private readonly Dictionary<TileType, int> costs = new();

    public int MinCost { get; private set; } = 1;
    public int MapVersion { get; private set; } = 0;

    public static MapDataHandler Instance { get; private set; } //TODO: Replace with Zenject

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void AddTile(ITile tile)
    {
        if (tile == null) return;
        tiles.TryAdd(tile.Position, tile);
        SetTileAvailable(tile.Position);

        AddCost(tile);
    }

    private void AddCost(ITile tile)
    {
        if (!costs.ContainsKey(tile.TileType))
        {
            costs[tile.TileType] = tile.CostToWalk;
            MinCost = costs.Values.Min();
        }
    }

    public void AddEntity(IEntity entity)
    {
        if (entity == null) return;
        entities.TryAdd(entity.Position, entity);
    }

    public IEntity GetEntity(Vector2Int entityPos)
    {
        entities.TryGetValue(entityPos, out var entity);
        return entity;
    }

    public void ClearMap()
    {
        foreach (var tile in tiles.Values)
        {
            Clear(tile, tile.Position);
        }
        tiles.Clear();
        costs.Clear();
    }

    public void ClearEntities()
    {
        foreach (var entity in entities.Values)
        {
            Clear(entity, entity.Position);
        }
        entities.Clear();
    }

    private void Clear<T>(T item, Vector2Int tilePos)
    {
        SetTileAvailable(tilePos);
        TryReturnToPool(item);
    }

    public ITile GetTile(Vector2Int tilePos)
    {
        tiles.TryGetValue(tilePos, out var tile);
        return tile;
    }

    public ITile GetAnyAvailableWalkableTile()
    {
        var availableTiles = tiles.Values.Where(t => t.IsWalkable && !t.IsOccupied).ToArray();
        return availableTiles.Length > 0
            ? availableTiles[Random.Range(0, availableTiles.Length)]
            : null;
    }

    public void ChangeTile(ITile tile)
    {
        if (tiles.ContainsKey(tile.Position))
        {
            tiles[tile.Position] = tile;
            AddCost(tile);
        }
    }

    public bool HasEntity(Vector2Int tilePos) => entities.ContainsKey(tilePos);

    public void RemoveEntityAt(Vector2Int tilePos)
    {
        if (entities.TryGetValue(tilePos, out var entity))
        {
            RemoveEntity(entity);
        }
    }

    public void RemoveEntity(IEntity entity)
    {
        if (entity == null) return;

        Clear(entity, entity.Position);
        entities.Remove(entity.Position);
    }

    public void RemovePlayerEntity() => RemoveEntity(GetPlayerEntity());

    public IEntity GetPlayerEntity()
    {
        foreach (var entity in entities.Values)
        {
            if (entity.EntityType == EntityType.Player) return entity;
        }
        return null;
    }

    private void TryReturnToPool<T>(T item)
    {
        if (item is IPoolable p) p.ReturnToPool();
    }

    private void SetTileAvailable(Vector2Int tilePos)
    {
        if (tiles.TryGetValue(tilePos, out var tile))
            tile.IsOccupied = false;
    }

    public int GetTileCost(Vector2Int tilePos)
    {
        if (costs.TryGetValue(GetTile(tilePos).TileType, out var cost))
            return cost;

        return int.MaxValue;
    }

    public void IncreaseMapVersion() => MapVersion += 1;
}
