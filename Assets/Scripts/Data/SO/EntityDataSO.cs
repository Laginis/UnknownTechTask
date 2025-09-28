using UnityEngine;

[CreateAssetMenu(menuName = "SO/Entity Data")]
public class EntityDataSO : ScriptableObject    //TODO: Add min/max range?
{
    [field: SerializeField] public EntityType EntityType { get; private set; }
    [field: SerializeField, Min(0)] public int AttackRange { get; set; }
    [field: SerializeField, Min(0)] public int MoveRange { get; set; }
}
