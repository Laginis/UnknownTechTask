using UnityEngine;

[CreateAssetMenu(menuName = "SO/Tile Data")]
public class TileDataSO : ScriptableObject
{
    [field: SerializeField] public TileType TileType { get; private set; }
    [field: SerializeField] public Material Material { get; private set; }
    [field: SerializeField] public Vector2 TileSize { get; private set; }
    [field: SerializeField] public bool IsWalkable { get; private set; }
    [field: SerializeField] public bool IsAttackThrough { get; private set; }
    [field: SerializeField, Min(1)] public int CostToWalk { get; private set; }
}
