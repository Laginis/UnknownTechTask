using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Transform gridParent;
    [SerializeField] private Transform entityParent;
    private MapDataHandler dataHandler;

    void Start()
    {
        dataHandler = MapDataHandler.Instance;
    }

    public void GenerateRandom(MapDataSO config)
    {
        GenerateGrid<TileController>(config, randomTiles: true);
        SpawnEntities(config);
    }

    public void GenerateGrid<T>(MapDataSO config, bool randomTiles = false)
        where T : Component, ITile
    {
        if (!IsValid(config)) return;

        dataHandler.ClearMap();
        dataHandler.ClearEntities();

        float weightSum = CalculateWeightSum(config.TileSettings);
        TileDataSO tileData;
        for (int i = 0; i < config.Width; i++)
        {
            for (int j = 0; j < config.Height; j++)
            {
                if (randomTiles) tileData = GetRandomTileData(weightSum, config.TileSettings);
                else tileData = GetDefaultTileData(config.TileSettings);

                SpawnTile<T>(tileData, new(i, j));
            }
        }
    }

    private void SpawnTile<T>(TileDataSO tileData, Vector2Int tilePos)
        where T : Component, ITile
    {
        if (tileData == null) return;

        var tile = ObjectPool.Instance.Request<T>(gridParent);
        tile.transform.position = new Vector3(tilePos.x, 0, tilePos.y);
        tile.SetTileData(tileData);
        tile.Position = tilePos;

        dataHandler.AddTile(tile);
    }

    private TileDataSO GetDefaultTileData(MapDataSO.TileSetting[] data) => data[0].TileData;

    private void SpawnEntities(MapDataSO config)
    {
        int spawnAmount = 1;    //TODO: Add support for random spawn amount
        foreach (var entitySetting in config.EntitySettings)
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                SpawnEntity(entitySetting.EntityData);
            }
        }
    }

    public void SpawnEntity(EntityDataSO entityData, ITile tile = null)
    {
        if (entityData == null) return;

        if (tile != null) RemoveEntityAt(tile.Position);

        tile ??= dataHandler.GetAnyAvailableWalkableTile();

        if (entityData.EntityType == EntityType.Player)    //only 1 player on map
        {
            dataHandler.RemovePlayerEntity();
            SpawnEntityAt<Player>(entityData, tile);
        }
        else if (entityData.EntityType == EntityType.Enemy)
        {
            SpawnEntityAt<Enemy>(entityData, tile);
        }
    }

    private void SpawnEntityAt<T>(EntityDataSO entityData, ITile tile) where T : Component, IEntity
    {
        if (tile == null || tile.IsOccupied || !tile.IsWalkable) return;

        tile.IsOccupied = true;
        Vector2Int tilePos = tile.Position;
        Vector3 entityPos = new(tilePos.x, tile.GetTileHeight(), tilePos.y);

        T entity = ObjectPool.Instance.Request<T>(entityParent);
        entity.transform.position = entityPos;
        entity.Position = tilePos;
        entity.SetEntityData(entityData);
        dataHandler.AddEntity(entity);
    }

    public void RemoveEntityAt(Vector2Int tilePos) => dataHandler.RemoveEntityAt(tilePos);

    private float CalculateWeightSum(MapDataSO.TileSetting[] data)
    {
        float allWeight = 0f;
        for (int i = 0; i < data.Length; i++)
        {
            allWeight += data[i].SpawnWeight;
        }
        return allWeight;
    }

    private TileDataSO GetRandomTileData(float weightSum, MapDataSO.TileSetting[] data)
    {
        float randValue = Random.Range(0, weightSum);
        float currentWeight = 0f;
        for (int i = 0; i < data.Length; i++)
        {
            currentWeight += data[i].SpawnWeight;
            if (randValue < currentWeight)
            {
                return data[i].TileData;
            }
        }
        return null;
    }

    private bool IsValid(MapDataSO config)
    {
        return config != null
            && config.TileSettings != null
            && config.TileSettings.Length != 0;
    }

    public void ChangeTile(ITile tile, TileDataSO newData)
    {
        if (!newData.IsWalkable) RemoveEntityAt(tile.Position);
        tile.SetTileData(newData);
        dataHandler.ChangeTile(tile);
    }

    public void IncreaseMapVersion() => dataHandler.IncreaseMapVersion();
}
