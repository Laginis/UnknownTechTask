using System.Collections.Generic;
using UnityEngine;

public class PathData
{
    public Vector2Int StartPos;
    public Vector2Int TargetPos;
    public Vector2Int MoveRange;
    public Vector2Int AttackRange;
    public Vector2Int UnreachableRange;
    public List<Vector2Int> Path = new();

    public bool IsReachable()
    {
        return UnreachableRange.x - UnreachableRange.y == 0;
    }

    public bool CanAttack()
    {
        return AttackRange.y > 0;
    }

    public bool CanWalk()
    {
        return MoveRange.y > 0;
    }

    public void Clear()
    {
        StartPos = default;
        TargetPos = default;
        Path.Clear();
        MoveRange = default;
        AttackRange = default;
    }

    public bool IsValid()
    {
        return Path != null
            && Path.Count != 0;
    }

    public Vector2Int SelectedMoveTile()
    {
        return CanWalk() ? Path[MoveRange.y - 1] : StartPos;
    }

    public Vector2Int SelectedAttackTile()
    {
        return CanAttack() ? Path[AttackRange.y - 1] : SelectedMoveTile();
    }
}
