using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct LevelSettings
{
    public string levelName;
    public int rows;
    public int columns;
    public Vector2 cellSize;
    public Vector2 spacing;
}

[CreateAssetMenu(menuName = "Game/MasterLevelData")]
public class MasterLevelData : ScriptableObject
{
    public List<LevelSettings> allLevels;

    public LevelSettings GetLevel(int index)
    {
        int levelIndex = Mathf.Clamp(index, 0, allLevels.Count - 1);
        return allLevels[levelIndex];
    }
}