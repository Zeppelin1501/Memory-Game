using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;

public class PhotoCapture : MonoBehaviour
{
    [SerializeField] private Camera photoCamera;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private RawImage displayImage;
    [SerializeField] private GameObject[] savingPanel;
    [SerializeField] private Button takePhotoButton;
    [SerializeField] private int countdownDuration = 5;

    public AudioSource audioSource;
    public AudioClip shutterAudio;
    public CameraFlash flash;

    private bool isCapturing = false; // Flag to prevent multiple triggers

    private void Start()
    {
        // Assign the button click listener
        takePhotoButton.onClick.AddListener(StartPhotoCapture);
    }

    private IEnumerator TakePhotoAfterCountdown()
    {
        // Disable the button and set capturing flag
        takePhotoButton.interactable = false;
        isCapturing = true;

        for (int i = countdownDuration; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        countdownText.text = "Smile!";
        countdownText.text = "";
        yield return new WaitForSeconds(1);

        flash.TriggerFlash();

        // Play the shutter audio
        audioSource.PlayOneShot(shutterAudio);

        CapturePhoto();

        // Wait for 0.5 seconds
        yield return new WaitForSeconds(1f);

        // Change the active state of the saving panels
        savingPanel[0].SetActive(false);
        savingPanel[1].SetActive(true);

        // Re-enable the button and reset capturing flag
        takePhotoButton.interactable = true;
        isCapturing = false;
    }

    public void StartPhotoCapture()
    {
        if (!isCapturing) // Check if the capturing process is already running
        {
            StartCoroutine(TakePhotoAfterCountdown());
        }
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

        // Save the photo to persistent data path
        SavePhoto(photo);
    }

    private void SavePhoto(Texture2D photo)
    {
        // Get the persistent data path
        string folderPath = Application.persistentDataPath;

        // Ensure the directory exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Create a unique filename based on date and time
        string filePath = Path.Combine(folderPath, "DCT_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");

        // Convert the photo to PNG format
        byte[] bytes = photo.EncodeToPNG();

        // Write the bytes to file
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Photo saved to: " + filePath);
    }
}
