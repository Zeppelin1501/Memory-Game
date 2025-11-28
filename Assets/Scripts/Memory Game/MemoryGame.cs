using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // Import TMP namespace
using DG.Tweening; // Import DOTween namespace
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MemoryGame : MonoBehaviour
{
    #region Public Variables
    public Button[] buttons; // 16 buttons
    public Sprite[] sprites; // 16 sprites
    public Sprite backSide;  // Sprite for the back side of the buttons
    public TMP_Text timerText; // Reference to TMP_Text for displaying the timer
    public TMP_Text triesText; // Reference to TMP_Text for displaying the number of tries
    public float timer = 90f; // 1:30 minutes in seconds
    public GameObject winScreen;
    public GameObject looseScreen;

    public MemoryGameAudioManager audioManager; // Reference to the MemoryGameAudioManager
    #endregion

    #region Private Variables
    private int[] buttonState; // 0 = hidden, 1 = showing, 2 = matched
    private int firstButtonIndex = -1;
    private int secondButtonIndex = -1;
    private bool isChecking = false;
    private bool gameActive = false; // Initialize as false
    private int tries = 0; // Variable to track the number of tries
    #endregion

    #region Unity Methods
    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        UpdateGameTimer();
    }
    #endregion

    #region Game Initialization
    private void InitializeGame()
    {
        if (buttons.Length != 16 || sprites.Length != 16)
        {
            Debug.LogError("Buttons array must have exactly 16 elements and Sprites array must have exactly 16 elements.");
            return;
        }

        buttonState = new int[buttons.Length];

        // Initialize button states and add listeners
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttonState[i] = 0; // All buttons are hidden initially
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }

        // Shuffle sprites and assign them to buttons
        ShuffleSprites();
        Debug.Log("Sprites shuffled and assigned.");

        // Display shuffled sprites briefly, then hide them and start the timer
        StartCoroutine(ShowAndHideSprites());
    }
    #endregion

    #region Game Timer
    private void UpdateGameTimer()
    {
        if (gameActive)
        {
            timer -= Time.deltaTime; // Decrease timer
            if (timer <= 0)
            {
                timer = 0;
                gameActive = false; // End the game
                Debug.Log("Time's up! Game Over.");
                StartCoroutine(ShowUnmatchedButtons()); // Show unmatched buttons
            }
            UpdateTimerText(); // Update the timer display
        }
    }
    #endregion

    #region Sprite Management
    IEnumerator ShowAndHideSprites()
    {
        // Show shuffled sprites briefly
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].image.sprite = sprites[int.Parse(buttons[i].tag) - 1]; // Show the sprite
        }

        yield return new WaitForSeconds(2); // Show for 2 seconds or adjust as needed

        // Hide all sprites with the back side
        for (int i = 0; i < buttons.Length; i++)
        {
            AnimateFlip(buttons[i], backSide); // Flip to back side
        }

        // Start the timer after the flip
        gameActive = true;
        UpdateTimerText(); // Update the timer display
    }

    void ShuffleSprites()
    {
        // Create a list of tag indices (1-8), each appearing twice
        List<int> tagIndices = new List<int>();
        for (int i = 1; i <= 8; i++)
        {
            tagIndices.Add(i);
            tagIndices.Add(i);
        }

        // Shuffle the list
        for (int i = 0; i < tagIndices.Count; i++)
        {
            int temp = tagIndices[i];
            int randomIndex = Random.Range(i, tagIndices.Count);
            tagIndices[i] = tagIndices[randomIndex];
            tagIndices[randomIndex] = temp;
        }

        // Assign shuffled tag indices to buttons and set the GameObject names
        for (int i = 0; i < buttons.Length; i++)
        {
            string tagString = tagIndices[i].ToString();
            buttons[i].tag = tagString; // Use tag to store sprite index
            buttons[i].name = $"Button_{tagString}"; // Set GameObject name
            Debug.Log($"Button {i} assigned tag {buttons[i].tag} and name {buttons[i].name}");
        }
    }


    void AnimateFlip(Button button, Sprite sprite)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        Vector3 originalScale = rectTransform.localScale;

        // Flip animation
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOScaleX(0, 0.1f).SetEase(Ease.InOutQuad)) // Flip to 0 width
                .AppendCallback(() => button.image.sprite = sprite) // Change sprite
                .Append(rectTransform.DOScaleX(1, 0.1f).SetEase(Ease.InOutQuad)); // Flip back to original width
    }
    #endregion

    #region Button Handling
    void OnButtonClick(int index)
    {
        if (isChecking || buttonState[index] != 0) return; // Prevent interaction if already checking or not hidden

        audioManager.PlayButtonClick();

        Debug.Log($"Button {index} clicked");
        buttonState[index] = 1; // Show the button
        buttons[index].interactable = false; // Make button uninteractable

        // Animate flip
        AnimateFlip(buttons[index], sprites[int.Parse(buttons[index].tag) - 1]);

        if (firstButtonIndex == -1)
        {
            firstButtonIndex = index;
            Debug.Log($"First button index set to {firstButtonIndex}");
        }
        else if (secondButtonIndex == -1)
        {
            secondButtonIndex = index;
            Debug.Log($"Second button index set to {secondButtonIndex}");
            StartCoroutine(CheckMatch());
        }
    }
    #endregion

    #region Match Checking
    IEnumerator CheckMatch()
    {
        isChecking = true;

        // Increment the tries count
        tries++;
        UpdateTriesText();

        if (buttons[firstButtonIndex].tag == buttons[secondButtonIndex].tag)
        {
            // Play correct match sound
            audioManager.PlayCorrectMatch();

            yield return new WaitForSeconds(1);

            buttonState[firstButtonIndex] = 2;
            buttonState[secondButtonIndex] = 2;

            buttons[firstButtonIndex].interactable = false;
            buttons[secondButtonIndex].interactable = false;

            if (CheckAllMatchesPaired())
            {
                // Play win sound and handle win state
                audioManager.PlayWin();
                winScreen.SetActive(true);
                StartCoroutine(Win());
                gameActive = false;
            }
        }
        else
        {
            // Play incorrect match sound
            audioManager.PlayIncorrectMatch();

            yield return new WaitForSeconds(1);

            AnimateFlip(buttons[firstButtonIndex], backSide);
            AnimateFlip(buttons[secondButtonIndex], backSide);
            buttonState[firstButtonIndex] = 0;
            buttonState[secondButtonIndex] = 0;

            buttons[firstButtonIndex].interactable = true;
            buttons[secondButtonIndex].interactable = true;
        }

        firstButtonIndex = -1;
        secondButtonIndex = -1;
        isChecking = false;
    }

    IEnumerator Win()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Memory_Start");
    }

    bool CheckAllMatchesPaired()
    {
        foreach (int state in buttonState)
        {
            if (state != 2) // If any button is not matched
            {
                return false; // Not all matches are paired
            }
        }
        return true; // All matches are paired
    }
    #endregion

    #region Game Over Handling
    // Coroutine to display remaining unmatched buttons all at once
    IEnumerator ShowUnmatchedButtons()
    {
        // Gather unmatched button indices
        List<int> unmatchedButtons = new List<int>();
        for (int i = 0; i < buttonState.Length; i++)
        {
            if (buttonState[i] != 2)
            {
                unmatchedButtons.Add(i);
            }
        }

        // Show all unmatched buttons simultaneously
        foreach (int index in unmatchedButtons)
        {
            AnimateFlip(buttons[index], sprites[int.Parse(buttons[index].tag) - 1]);
        }

        yield return new WaitForSeconds(2f); // Wait to allow players to see unmatched buttons

        Debug.Log("Game Over! Here are the remaining unmatched pairs.");
        looseScreen.SetActive(true);
        audioManager.PlayLose();
        StartCoroutine(Loose());
    }

    IEnumerator Loose()
    {
        yield return new WaitForSeconds(5);
        
        SceneManager.LoadScene("Memory_Start");
    }
    #endregion

    #region Timer Management
    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timer / 60); // Calculate minutes
        int seconds = Mathf.FloorToInt(timer % 60); // Calculate seconds
        timerText.text = $"{minutes:00}:{seconds:00}"; // Display as "Time: MM:SS"
    }

    void UpdateTriesText()
    {
        triesText.text = $"Tries: {tries}"; // Update the number of tries display
    }
    #endregion
}
