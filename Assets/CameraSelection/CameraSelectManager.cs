using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class CameraSelectManager : MonoBehaviour
{
    public TMP_Dropdown cameraDropdown;  // Reference to the Dropdown UI element
    public GameObject toggleObject;  // Reference to the GameObject to toggle
    private string selectedCamera;
    private string saveFilePath;

    // Start is called before the first frame update
    void Start()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "CameraSettings");

        // Check if the folder exists, and create it if it doesn't
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        saveFilePath = Path.Combine(folderPath, "selectedCamera.json");
        Debug.Log("Save file path: " + saveFilePath);

        PopulateCameraDropdown();
        LoadSelectedCamera();
    }

    // Populates the dropdown with the names of available camera devices
    void PopulateCameraDropdown()
    {
        // Clear any existing options
        cameraDropdown.ClearOptions();

        // Get the available camera devices
        WebCamDevice[] devices = WebCamTexture.devices;

        // Create a list to store the camera names
        List<string> cameraNames = new List<string>();

        // Loop through the devices and add the names to the list
        foreach (WebCamDevice device in devices)
        {
            cameraNames.Add(device.name);
        }

        // Add the camera names to the dropdown
        cameraDropdown.AddOptions(cameraNames);

        // Add listener for dropdown value change
        cameraDropdown.onValueChanged.AddListener(delegate { SaveSelectedCamera(); });
    }

    // Saves the selected camera to a JSON file
    void SaveSelectedCamera()
    {
        selectedCamera = cameraDropdown.options[cameraDropdown.value].text;
        File.WriteAllText(saveFilePath, JsonUtility.ToJson(new CameraSaveData(selectedCamera)));
    }

    // Loads the selected camera from the JSON file
    void LoadSelectedCamera()
    {
        if (File.Exists(saveFilePath))
        {
            CameraSaveData cameraData = JsonUtility.FromJson<CameraSaveData>(File.ReadAllText(saveFilePath));
            selectedCamera = cameraData.cameraName;
            Debug.Log("Loaded camera name: " + selectedCamera);

            // Set the dropdown to the saved value if it exists in the list
            int index = cameraDropdown.options.FindIndex(option => option.text == selectedCamera);
            if (index >= 0)
            {
                cameraDropdown.value = index;
                Debug.Log("Dropdown value set to: " + selectedCamera);
            }
            else
            {
                Debug.LogWarning("Saved camera not found in the current device list: " + selectedCamera);
            }
        }
        else
        {
            Debug.LogWarning("No save file found. Default camera will be used.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle the visibility of the GameObject when the "Z" key is pressed
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (toggleObject != null)
            {
                toggleObject.SetActive(!toggleObject.activeSelf);
            }
            else
            {
                Debug.LogWarning("No toggle object assigned.");
            }
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