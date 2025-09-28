using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour, IEntity, IPoolable //TODO: Maybe better to use base class?
{
    public Vector2Int Position { get; set; }
    public EntityType EntityType => entityData ? entityData.EntityType : EntityType.None;
    public int AttackRange
    {
        get { return entityData ? entityData.AttackRange : 0; }
        set { entityData.AttackRange = Math.Max(0, value); }
    }
    public int MoveRange
    {
        get { return entityData ? entityData.MoveRange : 0; }
        set { entityData.MoveRange = Math.Max(0, value); }
    }

    [SerializeField] private AgentMovement agentMovement;
    [SerializeField] private PlayerAnimator playerAnimator;

    private EntityDataSO entityData;

    public void ReturnToPool() => ObjectPool.Instance.Return(this);

    public void SetEntityData(EntityDataSO data)
    {
        entityData = data;
    }

    public void ApplyPath(PathData pathData)
    {
        if (pathData == null || !pathData.IsValid() || pathData.TargetPos == Position) return;

        MapDataHandler.Instance.GetTile(pathData.StartPos).IsOccupied = false;
        MapDataHandler.Instance.GetTile(pathData.SelectedMoveTile()).IsOccupied = true;
        Position = pathData.SelectedMoveTile();

        List<Vector2Int> movePath = new();
        for (int i = 0; i < pathData.Path.Count; i++)
        {
            if (i < pathData.MoveRange.y)
            {
                movePath.Add(pathData.Path[i]);
            }
        }
        if (movePath.Count > 0) playerAnimator.SetRunAnimation();
        InputBlocker.Lock();
        agentMovement.MoveAlongPath(movePath, () =>
        {
            Attack(pathData);
            InputBlocker.Unlock();
        });
    }

    private void Attack(PathData pathData)
    {
        playerAnimator.SetIdleAnimation();
        if (pathData.CanAttack() && pathData.IsReachable())
        {
            Vector3 targetPos = new(pathData.TargetPos.x, transform.position.y, pathData.TargetPos.y);
            Vector3 dir = (targetPos - transform.position).normalized;
            if (dir != Vector3.zero)
            {
                InputBlocker.Lock();
                transform.DORotateQuaternion(Quaternion.LookRotation(dir), 0.15f).OnComplete(() =>
                {
                    playerAnimator.SetAttackAnimation();
                    var targetEntity = MapDataHandler.Instance.GetEntity(pathData.TargetPos);
                    if (targetEntity is IHittable hittable) hittable.GetHit();
                    InputBlocker.Unlock();
                });
            }
        }
    }
}
