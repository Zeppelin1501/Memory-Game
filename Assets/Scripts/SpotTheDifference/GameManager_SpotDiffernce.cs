using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_SpotDifference : MonoBehaviour
{
    [System.Serializable]
    public class CorrectPoint
    {
        public Button[] correctPointButtons;
    }

    public CorrectPoint[] correctPoints; // Array of CorrectPoint classes
    public CanvasGroup[] points; // Array of CanvasGroups to control visibility

    public PointCollector pointCollector; // Reference to the PointCollector script
    public AudioSource correctPointAudioSource; // Reference to the AudioSource component
    public AudioClip correctPointAudioClip; // Reference to the AudioClip for correct point

    private List<RectTransform> buttonRectTransforms = new List<RectTransform>(); // List to store RectTransforms of all buttons

    void Start()
    {
        // Iterate over each CorrectPoint
        for (int i = 0; i < correctPoints.Length; i++)
        {
            // Get the RectTransforms of the buttons in the current CorrectPoint
            for (int j = 0; j < correctPoints[i].correctPointButtons.Length; j++)
            {
                if (correctPoints[i].correctPointButtons[j] != null)
                {
                    RectTransform buttonRectTransform = correctPoints[i].correctPointButtons[j].GetComponent<RectTransform>();
                    if (buttonRectTransform != null)
                    {
                        buttonRectTransforms.Add(buttonRectTransform);

                        // Debug the position of the button's RectTransform
                        Vector2 position = buttonRectTransform.anchoredPosition;
                        Debug.Log($"Button {i}_{j} RectTransform anchoredPosition: (x: {position.x}, y: {position.y})");
                    }
                    else
                    {
                        Debug.LogError($"Button at position {j} in correctPoints[{i}] does not have a RectTransform component.");
                    }

                    // Add listener to each button's onClick event
                    int index = i; // Use the current CorrectPoint index
                    correctPoints[i].correctPointButtons[j].onClick.AddListener(() => OnCorrectPointClicked(index));
                }
                else
                {
                    Debug.LogError($"Button at position {j} in correctPoints[{i}] is null. Please assign a valid button.");
                }
            }
        }
    }

    // This method is called when a correct point button is clicked
    void OnCorrectPointClicked(int index)
    {
        if (index >= 0 && index < points.Length)
        {
            if (points[index] != null)
            {
                points[index].alpha = 1f; // Make the corresponding canvas group visible

                // Disable interactability for all buttons in the current CorrectPoint array
                foreach (Button button in correctPoints[index].correctPointButtons)
                {
                    if (button != null)
                    {
                        button.interactable = false; // Disable the button
                    }
                }

                // Play the correct point audio if available
                if (correctPointAudioSource != null && correctPointAudioClip != null)
                {
                    correctPointAudioSource.PlayOneShot(correctPointAudioClip);
                }

                // Call the method to update the collected points
                if (pointCollector != null)
                {
                    pointCollector.UpdatePointCollector();
                }
                else
                {
                    Debug.LogError("PointCollector reference is not assigned.");
                }
            }
            else
            {
                Debug.LogError($"CanvasGroup at index {index} is null. Please assign a valid CanvasGroup.");
            }
        }
        else
        {
            Debug.LogError($"Index {index} is out of range of the points array. Check the setup in the Inspector.");
        }
    }

    // Method to get the first interactable button
    public Button GetFirstInteractableButton()
    {
        foreach (CorrectPoint cp in correctPoints)
        {
            foreach (Button button in cp.correctPointButtons)
            {
                if (button != null && button.interactable)
                {
                    return button;
                }
            }
        }
        return null;
    }

    // Method to get the RectTransforms of all buttons
    public List<RectTransform> GetAllButtonRectTransforms()
    {
        return buttonRectTransforms;
    }
}
