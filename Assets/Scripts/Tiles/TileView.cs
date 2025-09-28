using System.Collections.Generic;
using UnityEngine;

public class TileView : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private GameObject highlight;
    private MaterialPropertyBlock block;
    private Color defaultColor = new();

    public void SetMaterial(Material material)
    {
        block ??= new();
        meshRenderer.material = material;
        meshRenderer.GetPropertyBlock(block);
        defaultColor = material.color;
        SetColor(material.color);
    }

    public void SetScale(Vector3 scale) => transform.localScale = scale;
    public void SetHighlightActive(bool active) => highlight.SetActive(active);

    public void SetColor(Color color) //TODO change to actual color
    {
        if (block != null)
        {
            block.SetColor("_BaseColor", color);
            meshRenderer.SetPropertyBlock(block);
        }
    }

    public void ResetColor()
    {
        SetColor(defaultColor);
    }

    public float GetHeight() => meshRenderer.bounds.size.y;
}
