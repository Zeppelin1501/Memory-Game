using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Webcam : MonoBehaviour
{
    [SerializeField] private RawImage[] img = default;
    private WebCamTexture webCam;
    private RenderTexture renderTexture;
    private bool isCameraActive = false;
    private string saveFilePath;

    // Start is called before the first frame update
    void Start()
    {
        // Define the directory and file path
        string folderPath = Path.Combine(Application.persistentDataPath, "CameraSettings");
        saveFilePath = Path.Combine(folderPath, "selectedCamera.json");

        // Ensure the directory exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("Created directory at: " + folderPath);
        }

        // Clear the texture on each RawImage initially
        foreach (var rawImage in img)
        {
            rawImage.texture = null;
        }

        // Create or assign the RenderTexture
        if (renderTexture == null)
        {
            // Set RenderTexture dimensions to 4K (3840x2160)
            renderTexture = new RenderTexture(3840, 2160, 24);
            renderTexture.Create();
        }

        // Get the list of available camera devices
        WebCamDevice[] devices = WebCamTexture.devices;

        // Log the names of all available camera devices
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log("Camera Device " + i + ": " + devices[i].name + ", isFrontFacing: " + devices[i].isFrontFacing);
        }

        // Load the selected camera name from the JSON file
        string selectedCameraName = LoadSelectedCamera();
        if (!string.IsNullOrEmpty(selectedCameraName))
        {
            // Try to find the selected camera in the available devices
            int selectedIndex = -1;
            for (int i = 0; i < devices.Length; i++)
            {
                if (devices[i].name == selectedCameraName)
                {
                    selectedIndex = i;
                    break;
                }
            }

            // If the selected camera is found, initialize it
            if (selectedIndex != -1)
            {
                webCam = new WebCamTexture(devices[selectedIndex].name);

                // Log the name of the camera device being used
                Debug.Log("Using Camera Device: " + devices[selectedIndex].name);
                StartCamera();  // Start the camera automatically
            }
            else
            {
                Debug.LogError("Selected camera not found: " + selectedCameraName);
            }
        }
        else
        {
            Debug.LogError("No camera name found in selectedCamera.json.");
        }
    }

    // Loads the selected camera from the JSON file
    string LoadSelectedCamera()
    {
        if (File.Exists(saveFilePath))
        {
            CameraSaveData cameraData = JsonUtility.FromJson<CameraSaveData>(File.ReadAllText(saveFilePath));
            Debug.Log("Loaded camera name from JSON: " + cameraData.cameraName);
            return cameraData.cameraName;
        }
        else
        {
            Debug.LogWarning("No save file found at: " + saveFilePath + ". Using default settings.");
            return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the WebCamTexture is playing and the camera is active
        if (webCam != null && webCam.isPlaying && isCameraActive)
        {
            // Render the WebCamTexture to the RenderTexture
            Graphics.Blit(webCam, renderTexture);

            // Update the texture on each RawImage
            foreach (var rawImage in img)
            {
                rawImage.texture = renderTexture;
            }
        }
    }

    // Method to start the camera
    public void StartCamera()
    {
        if (webCam != null && !webCam.isPlaying)
        {
            webCam.Play();
            isCameraActive = true;
            Debug.Log("Camera started.");
        }
    }

    // Method to stop the camera
    public void StopCamera()
    {
        if (webCam != null && webCam.isPlaying)
        {
            webCam.Stop();
            isCameraActive = false;
            Debug.Log("Camera stopped.");
        }
    }

    // Clean up the resources
    private void OnDestroy()
    {
        if (webCam != null && webCam.isPlaying)
        {
            webCam.Stop();
        }

        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }

    // Data structure to store the camera name in JSON
    [System.Serializable]
    private class CameraSaveData
    {
        public string cameraName;

        public CameraSaveData(string cameraName)
        {
            this.cameraName = cameraName;
        }
    }
}
