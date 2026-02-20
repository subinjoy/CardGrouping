using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI matchesText;
    [SerializeField] private TextMeshProUGUI turnsText;

    [Header("Event Channels")]
    [SerializeField] private GameStatsChannel onStatsUpdated;

    private void OnEnable() => onStatsUpdated.Subscribe(UpdateUI);
    private void OnDisable() => onStatsUpdated.Unsubscribe(UpdateUI);

    private void UpdateUI(GameStats stats)
    {
        if (scoreText != null) scoreText.text = $"{Enums.SCORELABEL}: {stats.score}";
        if (matchesText != null) matchesText.text = $"{Enums.MATCHESLABEL}: {stats.matches}";
        if (turnsText != null) turnsText.text = $"{Enums.TURNSLABEL}: {stats.turns}";

        StartCoroutine(TextScaleEffect(scoreText.transform));
    }

    private IEnumerator TextScaleEffect(Transform target)
    {
        Vector3 originalScale = Vector3.one;
        target.localScale = originalScale * 1.2f;
        float elapsed = 0;
        while (elapsed < 0.2f)
        {
            elapsed += Time.deltaTime;
            target.localScale = Vector3.Lerp(target.localScale, originalScale, elapsed / 0.2f);
            yield return null;
        }
        target.localScale = originalScale;
    }
}