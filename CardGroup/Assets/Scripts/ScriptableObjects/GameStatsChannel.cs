using UnityEngine;
using System;

[CreateAssetMenu(fileName = "GameStatsChannel", menuName = "Events/GameStatsChannel")]
public class GameStatsChannel : ScriptableObject
{
    private Action<GameStats> OnStatsChanged;

    public void Raise(GameStats stats) => OnStatsChanged?.Invoke(stats);
    public void Subscribe(Action<GameStats> listener) => OnStatsChanged += listener;
    public void Unsubscribe(Action<GameStats> listener) => OnStatsChanged -= listener;
}

public struct GameStats
{
    public int score;
    public int matches;
    public int turns;
}