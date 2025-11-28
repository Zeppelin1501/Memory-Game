using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;

public class PhotoCaptureMultiCapture : MonoBehaviour
{
    [SerializeField] private Camera photoCamera;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private RawImage displayImage;
    [SerializeField] private GameObject[] savingPanel;
    [SerializeField] private Button takePhotoButton;
    [SerializeField] private int countdownDuration = 3;
    public CameraFlash flash;

    private void Start()
    {
        // Assign the button click listener
        takePhotoButton.onClick.AddListener(StartPhotoCapture);
    }

    private IEnumerator TakePhotoAfterCountdown()
    {
        // Disable the button
        takePhotoButton.interactable = false;

        for (int i = countdownDuration; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        countdownText.text = "Smile!";
        yield return new WaitForSeconds(1);

        countdownText.text = "";
        CapturePhoto();
        flash.TriggerFlash();

        // Wait for 0.5 seconds
        yield return new WaitForSeconds(1f);

        // Change the active state of the saving panels
        savingPanel[0].SetActive(false);
        savingPanel[1].SetActive(true);

        // Re-enable the button
        takePhotoButton.interactable = true;
    }

    public void StartPhotoCapture()
    {
        StartCoroutine(TakePhotoAfterCountdown());
    }

    private void CapturePhoto()
    {
        int width = 2160;
        int height = 3840;

        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        photoCamera.targetTexture = renderTexture;
        Texture2D photo = new Texture2D(width, height, TextureFormat.RGB24, false);

        photoCamera.Render();
        RenderTexture.active = renderTexture;
        photo.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        photo.Apply();

        photoCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // Display the captured photo
        displayImage.texture = photo;

        // Save the photo
        SavePhoto(photo);
    }

    private void SavePhoto(Texture2D photo)
    {
        string folderPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "IYD");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, "photo_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");
        byte[] bytes = photo.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Photo saved to: " + filePath);
    }
}
