using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MailKit.Net.Smtp;
using MimeKit;
using DG.Tweening;
using UnityEngine.Windows.WebCam;
using UnityEngine.SceneManagement;
using System.Collections;

public class EmailSender : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField nameInputField;
    public Button sendButton;
    public GameObject sendPage;

    private string smtpHost = "box5452.bluehost.com";
    private int smtpPort = 465;
    private string senderEmail = "photobooth@events.creativetechnologyllc.com";
    private string senderPassword = "CTp#0+0Bo0tH";
    private float errorTextColorDuration = 0.5f; // Duration for error text color change
    private bool isSendingEmail = false; // Flag to track if an email is being sent

    void Start()
    {
        sendButton.onClick.AddListener(SendEmail);
    }

    private async void SendEmail()
    {
        if (isSendingEmail)
        {
            Debug.LogWarning("Email is already being sent. Please wait...");
            
            return;
        }

        isSendingEmail = true; // Set the flag to true

        string userName = nameInputField.text;
        string recipientEmail = emailInputField.text;
        string attachmentPath = GetLatestPhotoPath();
        string htmlBody = GetHtmlBody("email_DCT.html");

        if (string.IsNullOrEmpty(recipientEmail) || !IsValidEmail(recipientEmail))
        {
            Debug.LogError("Recipient email is invalid or empty.");
            SetErrorTextColor(emailInputField);
            isSendingEmail = false;
            return;
        }

        if (string.IsNullOrEmpty(userName) || !IsValidName(userName))
        {
            Debug.LogError("User name is invalid or empty.");
            SetErrorTextColor(nameInputField);
            isSendingEmail = false;
            return;
        }

        if (string.IsNullOrEmpty(attachmentPath))
        {
            Debug.LogError("No photo found to attach.");
            isSendingEmail = false;
            return;
        }

        if (string.IsNullOrEmpty(htmlBody))
        {
            Debug.LogError("Failed to read HTML body.");
            isSendingEmail = false;
            return;
        }

        // Replace the {UserName} placeholder with the actual user name
        htmlBody = htmlBody.Replace("{UserName}", userName);

        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress("DCT", senderEmail));
        message.To.Add(new MailboxAddress("Recipient Name", recipientEmail));
        message.Subject = "Smile! Your Photobooth Photos Are Ready 📸";

        BodyBuilder bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = htmlBody;

        // Embed the image and set the Content-ID
        var image = bodyBuilder.LinkedResources.Add(attachmentPath);
        image.ContentId = "myImageID";

        // Modify the HTML body to include the image as cid
        bodyBuilder.HtmlBody = htmlBody.Replace("cid:myImageID", $"cid:{image.ContentId}");

        message.Body = bodyBuilder.ToMessageBody();

        using (SmtpClient client = new SmtpClient())
        {
            try
            {
                await client.ConnectAsync(smtpHost, smtpPort, true);
                await client.AuthenticateAsync(senderEmail, senderPassword);
                await client.SendAsync(message);
                Debug.Log("Email sent successfully.");

                TriggerSceneReset();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"An error occurred while sending the email: {ex.Message}");
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(currentSceneIndex);
            }
            finally
            {
                await client.DisconnectAsync(true);
                isSendingEmail = false; // Reset the sending flag
            }
        }
    }

    private string GetLatestPhotoPath()
    {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string ctFolderPath = Path.Combine(documentsPath, "DCT_Photobooth");

        if (!Directory.Exists(ctFolderPath))
        {
            Debug.LogError("Directory does not exist: " + ctFolderPath);
            return null;
        }

        var directoryInfo = new DirectoryInfo(ctFolderPath);
        var latestFile = directoryInfo.GetFiles()
            .Where(f => f.Extension == ".jpg" || f.Extension == ".png")
            .OrderByDescending(f => f.LastWriteTime)
            .FirstOrDefault();

        return latestFile?.FullName;
    }

    private string GetHtmlBody(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError("File does not exist: " + filePath);
            return null;
        }
    }

    private bool IsValidEmail(string email)
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    private bool IsValidName(string name)
    {
        // Check if the name contains only alphabetic characters and spaces
        return Regex.IsMatch(name, @"^[a-zA-Z ]+$");
    }

    public void TriggerSceneReset()
    {
        StartCoroutine(ResetSceneAfterDelay(2f));
    }

    private IEnumerator ResetSceneAfterDelay(float delay)
    {
        sendPage.SetActive(true);
        yield return new WaitForSeconds(delay);
        ResetScene();
    }

    private void ResetScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void SetErrorTextColor(TMP_InputField inputField)
    {
        // Change text color to red using DOTween for visual feedback
        inputField.textComponent.DOColor(Color.red, errorTextColorDuration)
            .SetLoops(2, LoopType.Yoyo) // Flash red color twice
            .OnComplete(() =>
            {
                // After completing, set color back to black
                inputField.textComponent.DOColor(Color.black, 0f);
            });
    }
}
