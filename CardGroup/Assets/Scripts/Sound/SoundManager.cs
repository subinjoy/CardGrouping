using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip matchSuccessSound;

    [Header("Event Channels")]
    [SerializeField] private CardEventChannel onCardFlipped;
    [SerializeField] private MatchResultChannel onMatchResult;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 0;
    }

    private void OnEnable()
    {
        onCardFlipped.Subscribe(PlayClick);
        onMatchResult.Subscribe(PlayMatchResult);
    }

    private void OnDisable()
    {
        onCardFlipped.Unsubscribe(PlayClick);
        onMatchResult.Unsubscribe(PlayMatchResult);
    }

    private void PlayClick(Card card)
    {
        if (clickSound != null)
            _audioSource.PlayOneShot(clickSound);
    }

    private void PlayMatchResult(bool isMatch)
    {
        if (isMatch)
        {
            if (matchSuccessSound != null)
                _audioSource.PlayOneShot(matchSuccessSound);
        }
    }
}
