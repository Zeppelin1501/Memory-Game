using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HintManager : MonoBehaviour
{
    public Button hintBtn;
    public Image hintPoint;
    public GameManager_SpotDifference gameManager; 

    public AudioSource audioSource; 
    public AudioClip hintAudioClip; 

    public float moveDuration = 1f; 
    public float stayDuration = 3f; 
    public float buttonDisableDuration = 15f; 

    private Vector2 originalHintPosition;

    void Start()
    {
        if (hintBtn != null)
        {
            hintBtn.onClick.AddListener(ShowHint);
        }
        else
        {
            Debug.LogError("Hint button is not assigned.");
        }

        if (hintPoint != null)
        {
            originalHintPosition = hintPoint.rectTransform.anchoredPosition;
        }
        else
        {
            Debug.LogError("Hint point is not assigned.");
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned.");
        }

        if (hintAudioClip == null)
        {
            Debug.LogError("Hint AudioClip is not assigned.");
        }
    }

    void ShowHint()
    {
        // Play the hint audio if available
        if (audioSource != null && hintAudioClip != null)
        {
            audioSource.PlayOneShot(hintAudioClip);
        }

        Button firstInteractableButton = gameManager.GetFirstInteractableButton();
        if (firstInteractableButton != null)
        {
            StartCoroutine(MoveHintToButton(firstInteractableButton.GetComponent<RectTransform>()));
        }
        else
        {
            Debug.Log("No interactable buttons available for hint.");
        }
    }

    IEnumerator MoveHintToButton(RectTransform targetRectTransform)
    {
        Vector2 targetPosition = targetRectTransform.anchoredPosition;

        hintPoint.rectTransform.DOAnchorPos(targetPosition, moveDuration).SetEase(Ease.Linear);

        yield return new WaitForSeconds(moveDuration);

        yield return new WaitForSeconds(stayDuration);

        hintPoint.rectTransform.DOAnchorPos(originalHintPosition, moveDuration).SetEase(Ease.Linear);

        yield return new WaitForSeconds(moveDuration);

        hintBtn.interactable = false;
        yield return new WaitForSeconds(buttonDisableDuration);
        hintBtn.interactable = true;
    }
}
