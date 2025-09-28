using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm //TODO: Use Jobs or Coroutines
{
    private readonly MapDataHandler dataHandler;
    private int searchId = 0;
    private readonly AStarCache cache = new();
    private readonly MinHeap<AStarNode> openSet = new(250);
    private readonly HashSet<Vector2Int> targets = new();
    private readonly List<Vector2Int> DIRECTIONS = new()
    {
        new(0, 1), //NORTH
        new(0, -1), //SOUTH
        new(1, 0), //EAST
        new(-1, 0) //WEST
    };
    private readonly HashSet<Vector2Int> ReachablePositions = new(); //TODO: Move to AStarCache

    public AStarAlgorithm(MapDataHandler handler)
    {
        dataHandler = handler;
    }

    public List<Vector2Int> FindMovePath(Vector2Int startPos, Vector2Int targetPos)
    {
        return Find(startPos, targetPos, 0);
    }

    public List<Vector2Int> FindAttackPath(Vector2Int startPos, Vector2Int targetPos, int attackRange)
    {
        return Find(startPos, targetPos, attackRange);
    }

    private List<Vector2Int> Find(Vector2Int startPos, Vector2Int targetPos, int range)
    {
        searchId++;

        if (!IsTargetReachable(startPos, targetPos, range)) return new();

        if (!cache.IsValid(dataHandler.MapVersion))
        {
            cache.UpdateCache(dataHandler.MapVersion);
        }

        openSet.Clear();

        if (!cache.VisitedNodes.TryGetValue(startPos, out var startNode))
            cache.VisitedNodes[startPos] = startNode = new(startPos);

        if (startNode.LastSearchId != searchId)
        {
            startNode.G = 0;
            startNode.Parent = null;
            startNode.LastSearchId = searchId;
        }

        startNode.H = CalculateDistance(startPos, targetPos, dataHandler.MinCost);

        openSet.AddUpdate(startNode);
        cache.VisitedNodes[startPos] = startNode;

        while (openSet.Count > 0)
        {
            var currentNode = openSet.Pop();

            if (targets.Contains(currentNode.Position))
                return GetPathFrom(currentNode);

            foreach (var neighborPos in GetNeighbors(currentNode.Position, targetPos))
            {
                if (!cache.VisitedNodes.TryGetValue(neighborPos, out var neighborNode))
                    neighborNode = new(neighborPos);

                if (neighborNode.LastSearchId != searchId)
                {
                    neighborNode.G = int.MaxValue;
                    neighborNode.Parent = null;
                    neighborNode.LastSearchId = searchId;
                }

                int newG = currentNode.G + dataHandler.GetTileCost(neighborPos);
                if (newG > neighborNode.G)
                    continue;

                neighborNode.G = newG;
                neighborNode.H = CalculateDistance(neighborPos, targetPos, dataHandler.MinCost);
                neighborNode.Parent = currentNode;

                openSet.AddUpdate(neighborNode);
                cache.VisitedNodes[neighborPos] = neighborNode;
            }
        }

        return new();
    }

    private int CalculateDistance(Vector2Int startPos, Vector2Int targetPos, int minCost)
    {
        int dx = Mathf.Abs(targetPos.x - startPos.x);
        int dy = Mathf.Abs(targetPos.y - startPos.y);

        return minCost * (dx + dy);
    }

    private List<Vector2Int> GetPathFrom(AStarNode node)
    {
        List<Vector2Int> path = new();
        while (node != null)
        {
            path.Add(node.Position);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }

    private IEnumerable<Vector2Int> GetNeighbors(Vector2Int currentPos, Vector2Int targetPos)
    {
        foreach (var direction in DIRECTIONS)
        {
            var tile = dataHandler.GetTile(currentPos + direction);
            if (IsTileValid(tile, targetPos)) yield return tile.Position;
        }
    }

    private bool IsTileValid(ITile tile, Vector2Int targetPos)
    {
        return tile != null
            && tile.IsWalkable
            && (!tile.IsOccupied || tile.Position == targetPos);
    }

    private void UpdateTargets(Vector2Int targetPos, int range)
    {
        targets.Clear();
        targets.Add(targetPos);

        if (range <= 0) return;

        Vector2Int currentTargetPos;
        ITile tile;
        foreach (var direction in DIRECTIONS)
        {
            for (int i = 1; i < range; i++)
            {
                currentTargetPos = targetPos + (i * direction);
                tile = dataHandler.GetTile(currentTargetPos);
                if (tile == null || !tile.IsAttackThrough || tile.IsOccupied)
                    break;

                targets.Add(currentTargetPos);
            }
        }
    }

    public void UpdateReachablePositions(Vector2Int startPos)   //TODO: Update on event?
    {
        ReachablePositions.Clear();
        ReachablePositions.Add(startPos);

        Queue<Vector2Int> queue = new();
        queue.Enqueue(startPos);

        ITile currentTile;
        Vector2Int currentPos;
        while (queue.Count > 0)
        {
            currentPos = queue.Dequeue();
            foreach (var direction in DIRECTIONS)
            {
                currentTile = dataHandler.GetTile(currentPos + direction);

                if (currentTile != null
                    && currentTile.IsWalkable
                    && !ReachablePositions.Contains(currentTile.Position))
                {
                    queue.Enqueue(currentTile.Position);
                    ReachablePositions.Add(currentTile.Position);
                }
            }
        }
    }

    private bool IsTargetReachable(Vector2Int startPos, Vector2Int targetPos, int range)
    {
        if (ReachablePositions.Count == 0) UpdateReachablePositions(startPos);
        UpdateTargets(targetPos, range);
        foreach (var target in targets)
        {
            if (ReachablePositions.Contains(target)) return true;
        }
        return false;
    }

}
