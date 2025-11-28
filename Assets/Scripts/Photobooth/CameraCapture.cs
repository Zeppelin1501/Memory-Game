using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class CameraCapture : MonoBehaviour
{
    [SerializeField] private Camera photoCamera;
    [SerializeField] private Button captureButton;
    [SerializeField] private GameObject emailPanel;

    private void Start()
    {
        // Assign the button click listener
        captureButton.onClick.AddListener(CapturePhoto);
    }

    private void CapturePhoto()
    {
        // Define the resolution (4K)
        int width = 3840;
        int height = 2160;

        // Create a texture to render to
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        photoCamera.targetTexture = renderTexture;

        // Render the camera's view
        Texture2D photo = new Texture2D(width, height, TextureFormat.RGB24, false);
        photoCamera.Render();
        RenderTexture.active = renderTexture;
        photo.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        photo.Apply();

        // Clean up
        photoCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // Save the photo
        SavePhoto(photo);

        // Activate the emailPanel after a delay
        StartCoroutine(ActivateEmailPanelDelayed());
    }

    private void SavePhoto(Texture2D photo)
    {
        // Determine the folder path (My Documents/IYD)
        string folderPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "DCT_Photobooth");

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

    private IEnumerator ActivateEmailPanelDelayed()
    {
        yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds

        // Activate the emailPanel GameObject
        emailPanel.SetActive(true);
    }
}
