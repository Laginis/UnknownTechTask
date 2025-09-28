using System;
using UnityEngine;

public class AStarNode : IComparable<AStarNode>
{
    public AStarNode Parent;
    public Vector2Int Position { get; private set; }
    public int G;
    public int H;
    public int F => G + H;
    public int LastSearchId = -1;

    public AStarNode(Vector2Int nodePos)
    {
        Position = nodePos;
    }

    public int CompareTo(AStarNode other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;

        int fCompare = F.CompareTo(other.F);
        if (fCompare != 0) return fCompare;

        int hCompare = H.CompareTo(other.H);
        if (hCompare != 0) return hCompare;

        int gCompare = G.CompareTo(other.G);
        if (gCompare != 0) return gCompare;

        int xCompare = Position.x.CompareTo(other.Position.x);
        if (xCompare != 0) return xCompare;

        return Position.y.CompareTo(other.Position.y);
    }
}
