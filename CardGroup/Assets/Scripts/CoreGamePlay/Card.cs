using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Card : MonoBehaviour
{
    [Header("Visual References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backFace;
    [SerializeField] private Button button;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color RemovedColor;

    [Header("Transition Settings")]
    [SerializeField] private float flipSpeed = 0.2f;

    public int ID { get; private set; }
    public bool IsMatched { get; private set; }

    private CardEventChannel _onCardFlipped;
    private bool _isFlipped;
    private Coroutine _flipRoutine;

    public void Initialize(int id, Sprite icon, CardEventChannel channel)
    {
        ID = id;
        iconImage.sprite = icon;
        _onCardFlipped = channel;

        IsMatched = false;
        _isFlipped = false;
        if (button != null) button.interactable = true;

        if (_flipRoutine != null) StopCoroutine(_flipRoutine);
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        backFace.color = defaultColor; 
        backFace.gameObject.SetActive(true);
        iconImage.gameObject.SetActive(false);
    }

    public void OnCardClicked()
    {
        if (_isFlipped || IsMatched) return;

        _isFlipped = true;
        if (_flipRoutine != null) StopCoroutine(_flipRoutine);
        _flipRoutine = StartCoroutine(FlipRoutine(false)); 

        _onCardFlipped.Raise(this);
    }

    public void SetClickable(bool clickable)
    {
        if (button != null && !IsMatched)
            button.interactable = clickable;
    }

    public void Unflip()
    {
        _isFlipped = false;
        if (_flipRoutine != null) StopCoroutine(_flipRoutine);
        _flipRoutine = StartCoroutine(FlipRoutine(true)); 
    }

    private IEnumerator FlipRoutine(bool showBack)
    {
        yield return StartCoroutine(RotateTo(90f));

        backFace.gameObject.SetActive(showBack);
        iconImage.gameObject.SetActive(!showBack);

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
        transform.localRotation = endRot;
    }

    public void MatchAndDisable()
    {
        IsMatched = true;
        if (button != null) button.interactable = false;

        if (_flipRoutine != null) StopCoroutine(_flipRoutine);
        transform.localRotation = Quaternion.identity;

        iconImage.gameObject.SetActive(false);
        backFace.gameObject.SetActive(true);
        backFace.color = RemovedColor;
    }
}