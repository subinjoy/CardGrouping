using UnityEngine;

[CreateAssetMenu(menuName = "Game/LayoutConfiguration")]
public class LayoutConfig : ScriptableObject
{
    public int Rows;
    public int Columns;
    public Vector2 CellSize;
    public Vector2 Spacing;
    public int TotalPairs => (Rows * Columns) / 2;
}
