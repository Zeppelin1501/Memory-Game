using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // Public reference to the AudioSource component
    public AudioSource audioSource;

    // Public reference to the AudioClip to be played before loading the scene
    public AudioClip audioClip;

    // Public method to load a scene by name
    public void LoadLevel(string sceneName)
    {
        // Check if the sceneName is not empty or null
        if (!string.IsNullOrEmpty(sceneName))
        {
            // Check if the AudioSource and AudioClip are assigned
            if (audioSource != null && audioClip != null)
            {
                // Assign the audio clip to the AudioSource and play it
                audioSource.clip = audioClip;
                audioSource.Play();

                // Start the coroutine to wait for the audio to finish
                StartCoroutine(WaitForAudioAndLoadScene(sceneName));
            }
            else
            {
                Debug.LogWarning("AudioSource or AudioClip is not assigned.");
                // If there's no audio, directly load the scene
                SceneManager.LoadScene(sceneName);
            }
        }
        else
        {
            Debug.LogWarning("Scene name is empty or null.");
        }
    }

    // Coroutine to wait for the audio to finish playing before loading the scene
    private System.Collections.IEnumerator WaitForAudioAndLoadScene(string sceneName)
    {
        // Wait until the audio has finished playing
        while (audioSource.isPlaying)
        {
            yield return null; // Wait until the next frame
        }

        // Load the scene after the audio finishes playing
        SceneManager.LoadScene(sceneName);
    }
}
