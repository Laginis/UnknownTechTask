using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Map Config")]
public class MapDataSO : ScriptableObject
{
    [field: SerializeField, Range(1, 50)] public int MaxWidth { get; private set; }
    [field: SerializeField, Range(1, 50)] public int MaxHeight { get; private set; }

    private int width;
    private int height;

    public int Width
    {
        get { return width; }
        set { width = Mathf.Clamp(value, 0, MaxWidth); }
    }

    public int Height
    {
        get { return height; }
        set { height = Mathf.Clamp(value, 0, MaxHeight); }
    }

    [Serializable]
    public struct TileSetting
    {
        [field: SerializeField] public TileDataSO TileData { get; private set; }
        [field: SerializeField, Range(0, 1)] public float SpawnWeight { get; private set; }
    }

    [field: SerializeField] public TileSetting[] TileSettings { get; private set; }

    [Serializable]
    public struct EntitySetting //TODO: Add params to support random spawn amount
    {
        [field: SerializeField] public EntityDataSO EntityData { get; private set; }
    }
    [field: SerializeField] public EntitySetting[] EntitySettings { get; private set; }

}
