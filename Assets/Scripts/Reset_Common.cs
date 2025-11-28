using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reset_Common : MonoBehaviour
{
    public float inactivityLimit = 180f; // 3 minutes
    private float inactivityTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        // Check for touch or mouse input
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            ResetInactivityTimer();
        }

        // Increment the inactivity timer
        inactivityTimer += Time.deltaTime;

        // Check if the inactivity timer has exceeded the limit
        if (inactivityTimer >= inactivityLimit)
        {
            ResetScene();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Quit the application
            Application.Quit();
        }
    }

    // Reset the inactivity timer
    void ResetInactivityTimer()
    {
        inactivityTimer = 0f;
    }

    // Reset the current scene
    public void ResetScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
