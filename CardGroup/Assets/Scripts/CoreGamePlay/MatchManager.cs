using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.U2D;

public class MatchManager : MonoBehaviour
{
    [Header("Level Configuration")]
    [SerializeField] private MasterLevelData masterLevelData;

    [Header("Sprite Configuration")]
    [SerializeField] private SpriteAtlas cardAtlas;
    [SerializeField] private string spriteNamePrefix = "Icon_";

    [Header("Dependencies")]
    [SerializeField] private CardPooler cardPool;
    [SerializeField] private GridLayoutGroup gridGroup;
    [SerializeField] private CardEventChannel onCardFlipped;
    [SerializeField] private MatchResultChannel onMatchResult;

    private List<Card> _activeCards = new List<Card>();
    private List<Card> _flippedPair = new List<Card>();

    private int currentLevelIndex;
    private int currentScore;
    private int matchesFound;
    private int totalMatchesRequired;
    private bool lockInput;

    private void OnEnable() => onCardFlipped.Subscribe(HandleCardFlip);
    private void OnDisable() => onCardFlipped.Unsubscribe(HandleCardFlip);

    private void Start()
    {
        LoadGame();
        GenerateBoard();
    }

    public void GenerateBoard()
    {
        LevelSettings currentSettings = masterLevelData.GetLevel(currentLevelIndex);

        cardPool.DeactivateAll();
        _activeCards.Clear();
        _flippedPair.Clear();
        matchesFound = 0;
        lockInput = false;

        gridGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridGroup.constraintCount = currentSettings.columns;
        gridGroup.cellSize = currentSettings.cellSize;
        gridGroup.spacing = currentSettings.spacing;

        int totalCards = currentSettings.rows * currentSettings.columns;
        totalMatchesRequired = totalCards / 2;

        List<int> ids = Enumerable.Range(0, totalMatchesRequired)
            .SelectMany(i => new[] { i, i })
            .OrderBy(x => Random.value).ToList();

        for (int i = 0; i < ids.Count; i++)
        {
            Card card = cardPool.Get(gridGroup.transform);
            string spriteName = spriteNamePrefix + (ids[i] + 1);
            Sprite icon = cardAtlas.GetSprite(spriteName);
            card.Initialize(ids[i], icon, onCardFlipped);
            _activeCards.Add(card);
        }
    }

    private void HandleCardFlip(Card card)
    {
        if (lockInput || _flippedPair.Contains(card)) return;

        _flippedPair.Add(card);

        if (_flippedPair.Count == 2)
        {
            lockInput = true;
            ToggleAllCards(false);
            StartCoroutine(ProcessMatchRoutine());
        }
    }

    private IEnumerator ProcessMatchRoutine()
    {
        Card c1 = _flippedPair[0];
        Card c2 = _flippedPair[1];
        _flippedPair.Clear();

        yield return new WaitForSeconds(0.6f);

        if (c1.ID == c2.ID)
        {
            c1.MatchAndDisable();
            c2.MatchAndDisable();

            matchesFound++;
            currentScore += 100;

            onMatchResult?.Raise(true);
            SaveGame();

            if (matchesFound >= totalMatchesRequired)
            {
                yield return new WaitForSeconds(1.0f);
                AdvanceLevel();
                yield break;
            }
        }
        else
        {
            yield return new WaitForSeconds(0.05f);
            c1.Unflip();
            c2.Unflip();

            onMatchResult?.Raise(false);

            yield return new WaitForSeconds(0.3f);
        }

        lockInput = false;
        ToggleAllCards(true);
    }

    private void ToggleAllCards(bool state)
    {
        foreach (var card in _activeCards)
        {
            if (!card.IsMatched)
            {
                card.SetClickable(state);
            }
        }
    }

    private void AdvanceLevel()
    {
        currentLevelIndex++;
        PlayerPrefs.SetInt(Enums.LEVEL, currentLevelIndex);
        PlayerPrefs.Save();

        lockInput = false;
        GenerateBoard();
    }

    private void SaveGame()
    {
        PlayerPrefs.SetInt(Enums.SCORE, currentScore);
        PlayerPrefs.Save();
    }

    private void LoadGame()
    {
        currentScore = PlayerPrefs.GetInt(Enums.SCORE, 0);
        currentLevelIndex = PlayerPrefs.GetInt(Enums.LEVEL, 0);
    }
}