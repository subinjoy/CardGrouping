using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LevelStartChannel", menuName = "Events/LevelStartChannel")]
public class LevelStartChannel : ScriptableObject
{
    private Action<int> OnLevelStarted;

    public void Raise(int levelNumber)
    {
        OnLevelStarted?.Invoke(levelNumber);
    }

    public void Subscribe(Action<int> listener)
    {
        OnLevelStarted += listener;
    }

    public void Unsubscribe(Action<int> listener)
    {
        OnLevelStarted -= listener;
    }
}