using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CameraFlash : MonoBehaviour
{
    [SerializeField] private Image flashImage = default;
    [SerializeField] private float flashDuration = 0.5f;

    // Method to trigger the flash animation
    public void TriggerFlash()
    {
        // Ensure the image is fully transparent at the start
        flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 0);

        // Animate the alpha value to create a flash effect
        flashImage.DOFade(1, flashDuration / 2).OnComplete(() =>
        {
            flashImage.DOFade(0, flashDuration / 2);
        });
    }
}
