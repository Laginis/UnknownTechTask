using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    private Sequence moveSequence;
    private float moveDurationPerTile = 0.3f; //TODO: Add to UI slider


    public void MoveAlongPath(List<Vector2Int> tilePath, System.Action onComplete = null)
    {
        if (moveSequence != null && moveSequence.IsActive())
            moveSequence.Kill();

        moveSequence = DOTween.Sequence();
        List<Vector3> worldPath = ConvertToWorldPath(tilePath);

        foreach (var targetPos in worldPath)
        {
            moveSequence.Append(transform.DOMove(targetPos, moveDurationPerTile).SetEase(Ease.Linear).OnStart(() =>
                {
                    Vector3 dir = (targetPos - transform.position).normalized;
                    if (dir != Vector3.zero)
                        transform.DORotateQuaternion(Quaternion.LookRotation(dir), 0.15f);
                }));
        }

        moveSequence.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    private List<Vector3> ConvertToWorldPath(List<Vector2Int> tilePath)
    {
        List<Vector3> worldPath = new();
        foreach (var tilePos in tilePath)
        {
            worldPath.Add(new(tilePos.x, transform.position.y, tilePos.y));
        }
        return worldPath;
    }


}
