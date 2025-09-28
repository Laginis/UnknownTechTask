using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IEntity, IPoolable, IHittable  //TODO: Maybe better to use base class?
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
        get { return entityData ? entityData.MoveRange : 1; }
        set { entityData.MoveRange = Math.Max(1, value); }
    }

    [SerializeField] private EnemyAnimator enemyAnimator;

    private EntityDataSO entityData;

    public void ReturnToPool() => ObjectPool.Instance.Return(this);

    public void SetEntityData(EntityDataSO data)
    {
        entityData = data;
    }

    public void GetHit()
    {
        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        InputBlocker.Lock();
        enemyAnimator.SetDeathAnimation();
        yield return new WaitForSeconds(3.5f);
        MapDataHandler.Instance.RemoveEntity(this);
        InputBlocker.Unlock();
    }
}
