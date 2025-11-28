using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections;

public class CombineInputs : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TMP_InputField emailInputField;
    public TMP_InputField numberInputField;
    public TMP_InputField inputs;

    private void Start()
    {
        // Add an event listener to the numberInputField
        numberInputField.onValueChanged.AddListener(OnNumberValueChanged);
    }

    private void OnDestroy()
    {
        // Remove the event listener when this object is destroyed
        numberInputField.onValueChanged.RemoveListener(OnNumberValueChanged);
    }

    private void OnNumberValueChanged(string newValue)
    {
        // Remove any non-numeric characters from the input
        string numericOnly = Regex.Replace(newValue, @"[^0-9]", "");

        // Update the text with the cleaned numeric value
        numberInputField.text = numericOnly;
    }

    public void CombineAndAssign()
    {
        // Get the text from each input field
        string name = nameInputField.text;
        string email = emailInputField.text;
        string number = numberInputField.text;

        // Check if any of the input fields are empty
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(number))
        {
            Debug.LogWarning("One or more input fields are empty.");

            // Shake empty input fields
            if (string.IsNullOrEmpty(name))
                ShakeInputField(nameInputField.gameObject);
            if (string.IsNullOrEmpty(email))
                ShakeInputField(emailInputField.gameObject);
            if (string.IsNullOrEmpty(number))
                ShakeInputField(numberInputField.gameObject);

            return;
        }

        // Validate email using regex
        if (!IsValidEmail(email))
        {
            Debug.LogWarning("Email is not valid.");
            ShakeInputField(emailInputField.gameObject);
            return;
        }

        // Validate number input
        if (!IsNumeric(number))
        {
            Debug.LogWarning("Number input is not valid.");
            ShakeInputField(numberInputField.gameObject);
            return;
        }

        // Combine all the inputs separated by commas
        string combinedText = $"{name}, {email}, {number}";

        // Assign the combined text to the 'inputs' field
        inputs.text = combinedText;

        // Start the coroutine to clear the input fields after 0.5 seconds
        StartCoroutine(ClearInputsAfterDelay(0.5f));
    }

    // Coroutine to clear input fields after a delay
    private IEnumerator ClearInputsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        nameInputField.text = "";
        emailInputField.text = "";
        numberInputField.text = "";
        inputs.text = "";
    }

    // Validate email using regex
    private bool IsValidEmail(string email)
    {
        string pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
        return Regex.IsMatch(email, pattern);
    }

    // Shake input field GameObject
    private void ShakeInputField(GameObject inputField)
    {
        Vector3 originalPosition = inputField.transform.localPosition; // Store the original position

        float shakeDistance = UnityEngine.Random.Range(-10f, 10f); // Random distance within the range of -10 to 10
        inputField.transform.DOLocalMoveX(originalPosition.x + shakeDistance, 0.05f).SetLoops(5, LoopType.Yoyo)
            .OnComplete(() => inputField.transform.DOLocalMove(originalPosition, 0.1f)); // Move back to original position after shaking
    }

    // Validate number input using regex
    private bool IsNumeric(string input)
    {
        string pattern = @"^\d+$"; // Matches any string that consists only of digits
        return Regex.IsMatch(input, pattern);
    }
}
