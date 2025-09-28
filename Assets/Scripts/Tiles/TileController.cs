using UnityEngine;

public class TileController : MonoBehaviour, IHighlightable, IPoolable, ITile, IColorable
{
    public Vector2Int Position { get; set; }
    public bool IsOccupied { get; set; } = false;
    public bool IsWalkable => tileData ? tileData.IsWalkable : false;
    public bool IsAttackThrough => tileData ? tileData.IsAttackThrough : false;
    public int CostToWalk => tileData ? tileData.CostToWalk : int.MaxValue;
    public TileType TileType => tileData ? tileData.TileType : TileType.None;

    [SerializeField] private TileView view;
    private TileDataSO tileData;

    private void SetupView()
    {
        if (view == null || tileData == null) return;

        view.SetMaterial(tileData.Material);
        view.SetScale(new(tileData.TileSize.x, tileData.TileSize.y, tileData.TileSize.x));
        SetHighlightActive(false);
    }

    public void SetTileData(TileDataSO data)
    {
        tileData = data;
        SetupView();
    }

    public void SetHighlightActive(bool active) => view.SetHighlightActive(active);
    public float GetTileHeight() => view.GetHeight();
    public void ReturnToPool() => ObjectPool.Instance.Return(this);
    public void SetColor(Color color) => view.SetColor(color);
    public void ResetColor() => view.ResetColor();
}
