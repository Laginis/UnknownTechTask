using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Camera cam;
    private IHighlightable lastHighlight = null;
    private ITile lastTile = null;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (InputBlocker.IsLocked) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out ITile tile))
            {
                lastTile = tile;
            }

            TryHighlight(hit);
        }
        else
        {
            lastTile = null;
            TryRemoveLastHighlight();
        }

        TryClickTile();
    }

    private void TryRemoveLastHighlight()
    {
        if (lastHighlight != null)
        {
            lastHighlight.SetHighlightActive(false);
            lastHighlight = null;
        }
    }

    private void TryHighlight(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent(out IHighlightable highlight))
        {
            if (lastHighlight != highlight)
            {
                highlight.SetHighlightActive(true);
                lastHighlight?.SetHighlightActive(false);
                lastHighlight = highlight;
            }
        }
    }

    private void TryClickTile()
    {
        if (Mouse.current.leftButton.isPressed && lastTile != null)
        {
            EventBus.Publish(new TileClickedEvent(lastTile));
        }
    }
}
