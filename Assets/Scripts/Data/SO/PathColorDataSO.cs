using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Path Colors")]
public class PathColorDataSO : ScriptableObject
{
    [Serializable]
    public struct PathColorSetting
    {
        public ColorType colorType;
        public Color color;
    }
    [field: SerializeField] public PathColorSetting[] ColorSetting { get; private set; }
}
