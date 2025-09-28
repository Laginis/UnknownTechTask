using UnityEngine;

public interface IEntity
{
    public Vector2Int Position { get; set; }
    public EntityType EntityType { get; }
    public int AttackRange { get; set; }
    public int MoveRange { get; set; }
    public void SetEntityData(EntityDataSO entityData);
}
