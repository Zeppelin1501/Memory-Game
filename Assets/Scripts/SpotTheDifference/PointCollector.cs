using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PointCollector : MonoBehaviour
{
    public Image[] pointCollected; // Array of Image components to update
    public Sprite[] pointCollectedSprites; // Array of Sprites to replace on the images
    public GameObject gameWin;
    public AudioClip winAudioClip; // Reference to the AudioClip to be played

    private int pointsCollected = 0; // Counter for the number of points collected
    private AudioSource[] audioSources; // Array to hold all AudioSource components in the scene
    private GameObject bgAudio; // Reference to the "BGAudio" GameObject

    void Start()
    {
        // Find all AudioSource components in the scene
        audioSources = FindObjectsOfType<AudioSource>();

        // Find the "BGAudio" GameObject in the scene
        bgAudio = GameObject.Find("BGAudio");
    }

    // Method to call when a correct point is collected
    public void UpdatePointCollector()
    {
        if (pointsCollected < pointCollected.Length)
        {
            // Replace the sprite on the image at the current point index
            pointCollected[pointsCollected].sprite = pointCollectedSprites[pointsCollected];
            pointsCollected++;

            // Optionally, you can add logic here to do something special when all points are collected
            if (pointsCollected >= pointCollected.Length)
            {
                Debug.Log("All points collected!");

                // Set the "BGAudio" GameObject as inactive
                if (bgAudio != null)
                {
                    bgAudio.SetActive(false);
                }

                // Start coroutine to handle the delay before showing the win panel and playing the audio
                StartCoroutine(HandleWinSequence());
            }
        }
    }

    // Coroutine to handle the win sequence with a delay
    IEnumerator HandleWinSequence()
    {
        yield return new WaitForSeconds(1f); // Wait for 0.5 seconds

        // Play the win audio on all AudioSources
        PlayWinAudioOnAllSources();

        // Show the win panel
        gameWin.SetActive(true);

        StartCoroutine(WaitWinScreen());
    }

    // Method to play the win audio clip on all AudioSources in the scene
    private void PlayWinAudioOnAllSources()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource != null && winAudioClip != null)
            {
                audioSource.PlayOneShot(winAudioClip);
            }
        }
    }

    IEnumerator WaitWinScreen()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Spot_Start");
    }
}
