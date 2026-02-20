using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Card : MonoBehaviour
{
    [Header("Visual References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject backFace;
    [SerializeField] private Button button;

    [Header("Transition Settings")]
    [SerializeField] private float flipSpeed = 0.2f;

    public int ID { get; private set; }
    private CardEventChannel _onCardFlipped;
    private bool _isFlipped;
    private Coroutine _flipRoutine;

    public void Initialize(int id, Sprite icon, CardEventChannel channel)
    {
        ID = id;
        iconImage.sprite = icon;
        _onCardFlipped = channel;
        _isFlipped = false;
        backFace.SetActive(true);
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        button.interactable = true;
    }

    public void OnCardClicked()
    {
        if (_isFlipped) return;
        _isFlipped = true;

        if (_flipRoutine != null) StopCoroutine(_flipRoutine);
        _flipRoutine = StartCoroutine(FlipRoutine(false));     //card flips to front

        _onCardFlipped.Raise(this);
    }

    public void Unflip()
    {
        _isFlipped = false;
        if (_flipRoutine != null) StopCoroutine(_flipRoutine);
        _flipRoutine = StartCoroutine(FlipRoutine(true)); //card flips to back
    }

    private IEnumerator FlipRoutine(bool showBack)
    {
        yield return StartCoroutine(RotateTo(90f));

        backFace.SetActive(showBack);

        yield return StartCoroutine(RotateTo(0f));
        _flipRoutine = null;
    }

    private IEnumerator RotateTo(float targetAngle)
    {
        Quaternion startRot = transform.localRotation;
        Quaternion endRot = Quaternion.Euler(0, targetAngle, 0);
        float time = 0;

        while (time < 1f)
        {
            time += Time.deltaTime / flipSpeed;
            transform.localRotation = Quaternion.Lerp(startRot, endRot, time);
            yield return null;
        }
    }

    public void MatchAndDisable()
    {
        button.interactable = false;
        StartCoroutine(ScaleDown());
    }

    private IEnumerator ScaleDown()
    {
        float time = 0;
        Vector3 startScale = transform.localScale;
        while (time < 1f)
        {
            time += Time.deltaTime / 0.2f;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, time);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
