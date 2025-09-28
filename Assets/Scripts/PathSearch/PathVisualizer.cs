using System.Collections.Generic;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    [SerializeField] private PathColorDataSO pathColorDataSO;
    private readonly Dictionary<ColorType, Color> colorMap = new();
    private MapDataHandler dataHandler;

    void Awake()
    {
        MapColors();
    }

    void Start()
    {
        dataHandler = MapDataHandler.Instance;
    }

    private void MapColors()
    {
        foreach (var colorData in pathColorDataSO.ColorSetting)
        {
            colorMap[colorData.colorType] = colorData.color;
        }
    }

    public void ShowPath(PathData pathData)
    {
        if (pathData == null) return;

        ITile currentTile;
        for (int i = 0; i < pathData.Path.Count; i++)
        {
            currentTile = dataHandler.GetTile(pathData.Path[i]);
            ChangeTileColorWithinRange(i, pathData.MoveRange, currentTile, ColorType.MoveReachable, ColorType.MoveReachableSelected);
            ChangeTileColorWithinRange(i, pathData.AttackRange, currentTile, ColorType.AttackReachable, ColorType.AttackReachableSelected);
            ChangeTileColorWithinRange(i, pathData.UnreachableRange, currentTile, ColorType.Unreachable, ColorType.UnreachableSelected);
        }

        if (pathData.Path.Count == 0)
            SetTileColor(dataHandler.GetTile(pathData.TargetPos), ColorType.UnreachableSelected);

        RemoveTileHighlight(dataHandler.GetTile(pathData.TargetPos));
    }

    private void ChangeTileColorWithinRange(int index, Vector2Int range, ITile tile, ColorType baseColor, ColorType selectedColor)
    {
        if (index >= range.x && index < range.y)
        {
            if (index == range.y - 1) SetTileColor(tile, selectedColor);
            else SetTileColor(tile, baseColor);
        }
    }

    public void HidePath(PathData pathData)
    {
        if (pathData == null) return;

        for (int i = 0; i < pathData.Path.Count; i++)
        {
            ResetTileColor(dataHandler.GetTile(pathData.Path[i]));
        }

        if (pathData.Path.Count == 0)
            ResetTileColor(dataHandler.GetTile(pathData.TargetPos));
    }

    private void SetTileColor(ITile tile, ColorType colorType)
    {
        if (tile is IColorable iColor && colorMap.TryGetValue(colorType, out var color))
        {
            iColor.SetColor(color);
        }
    }

    private void ResetTileColor(ITile tile)
    {
        if (tile is IColorable iColor)
        {
            iColor.ResetColor();
        }
    }

    private void RemoveTileHighlight(ITile tile)
    {
        if (tile is IHighlightable iHighlight)
        {
            iHighlight.SetHighlightActive(false);
        }
    }

    public void ShowDestination(PathData pathData)
    {
        if (pathData == null) return;

        if (pathData.CanWalk())
            SetTileColor(dataHandler.GetTile(pathData.SelectedMoveTile()), ColorType.MoveReachableSelected);
        if (pathData.CanAttack() && pathData.IsReachable())
            SetTileColor(dataHandler.GetTile(pathData.SelectedAttackTile()), ColorType.AttackReachableSelected);
    }
}
