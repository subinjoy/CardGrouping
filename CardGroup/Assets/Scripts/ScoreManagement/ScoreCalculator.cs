using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    [Header("Listen to these")]
    [SerializeField] private MatchResultChannel onMatchResult;
    [SerializeField] private LevelStartChannel onLevelStarted;

    [Header("Broadcast to this")]
    [SerializeField] private GameStatsChannel onStatsUpdated;

    private int _score;
    private int _matches;
    private int _turns;

    private void OnEnable()
    {
        onMatchResult.Subscribe(HandleMatchUpdate);
        onLevelStarted.Subscribe(ResetLevelStats);
    }

    private void OnDisable()
    {
        onMatchResult.Unsubscribe(HandleMatchUpdate);
        onLevelStarted.Unsubscribe(ResetLevelStats);
    }

    private void Start()
    {
        // Load persistent score on start
        _score = PlayerPrefs.GetInt(Enums.SCORE, 0);
        Broadcast();
    }

    private void ResetLevelStats(int levelNumber)
    {
        _matches = 0;
        _turns = 0;
        Broadcast();
    }

    private void HandleMatchUpdate(bool isMatch)
    {
        _turns++;

        if (isMatch)
        {
            _matches++;
            _score += 100;
            PlayerPrefs.SetInt(Enums.SCORE, _score);
            PlayerPrefs.Save();
        }

        Broadcast();
    }

    private void Broadcast()
    {
        GameStats stats = new GameStats { score = _score, matches = _matches, turns = _turns };
        onStatsUpdated?.Raise(stats);
    }
}