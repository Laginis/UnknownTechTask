using System.Collections.Generic;
using UnityEngine;

public class AStarCache
{
    public int MapVersion { get; private set; }
    public readonly Dictionary<Vector2Int, AStarNode> VisitedNodes = new();
    public readonly HashSet<Vector2Int> ReachablePositions = new();

    public void UpdateCache(int newMapVersion)
    {
        MapVersion = newMapVersion;
        VisitedNodes.Clear();
    }

    public bool IsValid(int mapVersion)
    {
        return MapVersion == mapVersion;
    }
}
