using UnityEngine;
using System.Collections.Generic; // Add this line for using List<>

[CreateAssetMenu(fileName = "LevelSO", menuName = "Scriptable Objects/LevelSO")]
public class LevelSO : ScriptableObject
{
    [Header("Levels stats")]
    public List<LevelPOJO> Levels = new List<LevelPOJO>();
}
