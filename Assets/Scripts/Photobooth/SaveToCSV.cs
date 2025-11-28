using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Text.RegularExpressions;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class SaveToCSV : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TMP_InputField emailInputField;
    public Button closeButton;
    public Webcam webcam;
    public GameObject sendLoad;

    private float errorTextColorDuration = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        CreateFolderStructure();
        Debug.Log("Folder structure created.");
        closeButton.onClick.AddListener(ClearInputFields);
    }

    // Method to create the folder structure
    void CreateFolderStructure()
    {
        // Define folder structure for CSV files
        string csvFolderName = "DCT_Data";
        string csvMonthFolder = DateTime.Now.ToString("MMMM"); // Using full month name
        string csvDateFolder = DateTime.Now.ToString("dd"); // Using day of the month

        // Get the My Documents folder path
        string documentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        // Create the SIA folder if it doesn't exist
        string ctFolderPath = Path.Combine(documentsFolderPath, csvFolderName);
        if (!Directory.Exists(ctFolderPath))
        {
            Directory.CreateDirectory(ctFolderPath);
        }

        // Create the month folder for CSV files if it doesn't exist
        string csvMonthFolderPath = Path.Combine(ctFolderPath, csvMonthFolder);
        if (!Directory.Exists(csvMonthFolderPath))
        {
            Directory.CreateDirectory(csvMonthFolderPath);
        }

        // Create the date folder for CSV files if it doesn't exist
        string csvDateFolderPath = Path.Combine(csvMonthFolderPath, csvDateFolder);
        if (!Directory.Exists(csvDateFolderPath))
        {
            Directory.CreateDirectory(csvDateFolderPath);
        }

        Debug.Log("CSV folder structure created: " + csvDateFolderPath);
    }

    // Method to save the input data to a CSV file
    public void SaveDataToCSV()
    {
        // Get input values
        string name = nameInputField.text;
        string email = emailInputField.text;
        string photoID = GetLatestPhotoName();

        // Perform validation checks
        bool isValid = true;

        if (!IsValidName(name))
        {
            SetErrorTextColor(nameInputField);
            ShakeInputField(nameInputField.gameObject); // Shake the input field if name is invalid
            isValid = false;
        }

        if (!IsValidEmail(email))
        {
            SetErrorTextColor(emailInputField);
            ShakeInputField(emailInputField.gameObject); // Shake the input field if email is invalid
            isValid = false;
        }

        if (string.IsNullOrEmpty(name))
        {
            ShakeInputField(nameInputField.gameObject); // Shake the input field if name is empty
            isValid = false;
        }

        if (string.IsNullOrEmpty(photoID))
        {
            Debug.LogError("No valid photo found in the directory.");
            isValid = false;
        }

        if (!isValid)
        {
            Debug.Log("Validation failed. Data not saved.");
            return; // Exit if validation fails
        }

        // Define the file path
        string csvFolderName = "DCT_Data";
        string csvMonthFolder = DateTime.Now.ToString("MMMM");
        string csvDateFolder = DateTime.Now.ToString("dd");
        string documentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string csvFolderPath = Path.Combine(documentsFolderPath, csvFolderName, csvMonthFolder, csvDateFolder);
        string csvFilePath = Path.Combine(csvFolderPath, "data.csv");

        // Check if the file exists and if not, write the header
        bool fileExists = File.Exists(csvFilePath);

        try
        {
            using (StreamWriter sw = new StreamWriter(csvFilePath, true))
            {
                if (!fileExists)
                {
                    // Write the header
                    sw.WriteLine("Name,Email,Photo_ID");
                }
                // Write data
                sw.WriteLine($"{name},{email},{photoID}");
            }
            Debug.Log("Data saved successfully to " + csvFilePath);

            // Reset the scene after saving the CSV
            ResetScene();
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to save data to CSV: " + ex.Message);
        }

        sendLoad.SetActive(true);
    }

    private void SetErrorTextColor(TMP_InputField inputField)
    {
        // Change text color to red using DOTween
        inputField.textComponent.DOColor(Color.red, errorTextColorDuration)
            .SetLoops(2, LoopType.Yoyo) // Flash red color twice
            .OnComplete(() => {
                // After completing, set color back to black
                inputField.textComponent.DOColor(Color.black, 0f);
            });
    }

    private bool IsValidName(string name)
    {
        // Check if the name contains only alphabetic characters and spaces
        return Regex.IsMatch(name, @"^[a-zA-Z ]+$");
    }

    private bool IsValidEmail(string email)
    {
        // Simple email validation regex
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    private void ResetScene()
    {
        webcam.StopCamera();
    }

    private void ShakeInputField(GameObject inputField)
    {
        Vector3 originalPosition = inputField.transform.localPosition; // Store the original position

        float shakeDistance = UnityEngine.Random.Range(-10f, 10f); // Random distance within the range of -10 to 10
        inputField.transform.DOLocalMoveX(originalPosition.x + shakeDistance, 0.05f).SetLoops(5, LoopType.Yoyo)
            .OnComplete(() => inputField.transform.DOLocalMove(originalPosition, 0.1f)); // Move back to original position after shaking
    }

    private void ClearInputFields()
    {
        nameInputField.text = string.Empty;
        emailInputField.text = string.Empty;
    }

    private string GetLatestPhotoName()
    {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string iydDirectory = Path.Combine(documentsPath, "DCT_Photobooth");

        if (!Directory.Exists(iydDirectory))
        {
            Debug.LogError("Directory does not exist: " + iydDirectory);
            return null;
        }

        var directoryInfo = new DirectoryInfo(iydDirectory);
        var latestFile = directoryInfo.GetFiles()
            .Where(f => f.Extension == ".jpg" || f.Extension == ".png")
            .OrderByDescending(f => f.LastWriteTime)
            .FirstOrDefault();

        return latestFile?.Name; // Return only the name of the file
    }
}
