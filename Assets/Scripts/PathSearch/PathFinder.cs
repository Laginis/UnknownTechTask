using UnityEngine;

[RequireComponent(typeof(PathVisualizer))]
public class PathFinder : MonoBehaviour
{
    private AStarAlgorithm aStar;
    private PathVisualizer pathVisualizer;

    void Awake()
    {
        pathVisualizer = GetComponent<PathVisualizer>();
    }

    void Start()
    {
        aStar = new(MapDataHandler.Instance);
    }

#if UNITY_EDITOR
    [ContextMenu("Speed Test 0,0 -> 15,11")]
    void Find()
    {
        for (int i = 0; i < 10; i++)
            aStar.FindMovePath(new(0, 0), new(14, 10));

        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 10; i++)
            aStar.FindMovePath(new(0, 0), new(14, 10));
        watch.Stop();
        Debug.Log($"Avg ticks: {watch.ElapsedTicks / 10f}");
        Debug.Log($"Avg ms: {watch.ElapsedMilliseconds / 10f}");

    }
#endif

    public PathData FindMovePath(Vector2Int startPos, Vector2Int targetPos, int moveRange)
    {
        PathData pathData = new()
        {
            StartPos = startPos,
            TargetPos = targetPos,
            Path = aStar.FindMovePath(startPos, targetPos)
        };

        if (pathData.Path.Count > 0)
        {
            pathData.MoveRange = new(0, Mathf.Min(pathData.Path.Count, moveRange));
            pathData.AttackRange = new(0, 0);
            pathData.UnreachableRange = new(pathData.MoveRange.y, pathData.Path.Count);
        }

        pathVisualizer.ShowPath(pathData);
        return pathData;
    }

    public PathData FindAttackPath(Vector2Int startPos, Vector2Int targetPos, int moveRange, int attackRange)
    {
        PathData pathData = new()
        {
            StartPos = startPos,
            TargetPos = targetPos,
            Path = aStar.FindAttackPath(startPos, targetPos, attackRange)
        };

        if (pathData.Path.Count > 0)
        {
            var initPathSize = CompleteAttackPath(pathData);
            if (initPathSize == pathData.Path.Count)
            {
                pathData.MoveRange = new(0, 0);
                pathData.AttackRange = new(pathData.MoveRange.y, Mathf.Min(pathData.Path.Count, pathData.MoveRange.y + attackRange));
                pathData.UnreachableRange = new(pathData.AttackRange.y, pathData.Path.Count);
            }
            else
            {
                pathData.MoveRange = new(0, Mathf.Min(initPathSize, moveRange));
                int attackOffset = pathData.MoveRange.y > 0 ? 1 : 0;
                pathData.AttackRange = new(pathData.MoveRange.y, Mathf.Min(pathData.Path.Count, pathData.MoveRange.y - attackOffset + attackRange)); //we should take new position into account
                pathData.UnreachableRange = new(pathData.AttackRange.y, pathData.Path.Count);
            }
        }

        pathVisualizer.ShowPath(pathData);
        return pathData;
    }

    public void ResetPath(PathData pathData)
    {
        if (pathData == null) return;

        pathVisualizer.HidePath(pathData);
        pathData.Clear();
    }

    public void ShowDestination(PathData pathData)
    {
        if (pathData == null) return;

        pathVisualizer.HidePath(pathData);
        pathVisualizer.ShowDestination(pathData);
    }

    private Vector2Int GetDirection(Vector2Int startPos, Vector2Int targetPos)
    {
        var direction = targetPos - startPos;
        if (direction.x != 0) direction.x /= Mathf.Abs(direction.x);
        if (direction.y != 0) direction.y /= Mathf.Abs(direction.y);
        return direction;
    }

    private int CompleteAttackPath(PathData pathData)
    {
        var lastTilePos = pathData.Path[^1];
        var initPathSize = pathData.Path.Count;
        if (lastTilePos != pathData.TargetPos)
        {
            var direction = GetDirection(lastTilePos, pathData.TargetPos);
            Vector2Int currentPos = lastTilePos;
            while (currentPos != pathData.TargetPos)
            {
                currentPos += direction;
                pathData.Path.Add(currentPos);
            }
        }
        return initPathSize;
    }

    public void UpdateReachablePositions(Vector2Int startPos)
    {
        var player = MapDataHandler.Instance.GetPlayerEntity();
        if (player != null)
        {
            aStar.UpdateReachablePositions(startPos);
        }
    }
}
