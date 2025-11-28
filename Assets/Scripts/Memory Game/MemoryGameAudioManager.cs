using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MemoryGameAudioManager : MonoBehaviour
{
    #region Public Variables
    public AudioSource audioSource; // Reference to the AudioSource
    public AudioClip buttonClickClip; // Audio clip for button click
    public AudioClip correctMatchClip; // Audio clip for correct match
    public AudioClip incorrectMatchClip; // Audio clip for incorrect match
    public AudioClip winClip; // Audio clip for win
    public AudioClip loseClip; // Audio clip for lose
    #endregion

    #region Public Methods
    public void PlayButtonClick()
    {
        PlayAudioClip(buttonClickClip);
    }

    public void PlayCorrectMatch()
    {
        StartCoroutine(PlayClipWithDelay(correctMatchClip, 0.5f));
    }

    public void PlayIncorrectMatch()
    {
        StartCoroutine(PlayClipWithDelay(incorrectMatchClip, 0.5f));
    }

    public void PlayWin()
    {
        PlayAudioClip(winClip);
        GameObject bgAudio = GameObject.Find("BGAudio");
        if (bgAudio != null)
        {
            bgAudio.SetActive(false); // Deactivate BGAudio
        }
        //StartCoroutine(LoadSceneAfterDelay("Memory_Start", winClip.length)); // Wait for the win sound to finish
    }

    public void PlayLose()
    {
        PlayAudioClip(loseClip);
        GameObject bgAudio = GameObject.Find("BGAudio");
        if (bgAudio != null)
        {
            bgAudio.SetActive(false); // Deactivate BGAudio
        }
        //StartCoroutine(LoadSceneAfterDelay("Memory_Start", loseClip.length)); // Wait for the lose sound to finish
    }
    #endregion

    #region Private Methods
    private void PlayAudioClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private IEnumerator PlayClipWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayAudioClip(clip);
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
    #endregion
}
