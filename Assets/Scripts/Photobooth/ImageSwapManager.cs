using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ButtonSpritePair
{
    public Button button;
    public Sprite pressedSprite;
    public Sprite notPressedSprite;
}

public class ImageSwapManager : MonoBehaviour
{
    public ButtonSpritePair[] buttonSpritePairs;

    private Button selectedButton;

    private void Start()
    {
        // Initialize selectedButton to null
        selectedButton = null;
    }

    public void OnButtonClickSingleSelection(Button clickedButton)
    {
        ButtonSpritePair clickedPair = FindButtonSpritePair(clickedButton);

        if (clickedPair != null)
        {
            if (clickedButton == selectedButton)
            {
                DeselectButton(clickedPair);
            }
            else
            {
                if (selectedButton != null)
                {
                    DeselectButton(FindButtonSpritePair(selectedButton));
                }
                SelectButton(clickedPair);
            }
        }
    }

    public void OnButtonClickMultiSelection(Button clickedButton)
    {
        ButtonSpritePair clickedPair = FindButtonSpritePair(clickedButton);

        if (clickedPair != null)
        {
            // Check if the clicked button is already selected
            if (clickedButton == selectedButton)
            {
                // Deselect it
                DeselectButton(clickedPair);
            }
            else
            {
                // Select the clicked button
                SelectButton(clickedPair);
            }
        }
    }


    private ButtonSpritePair FindButtonSpritePair(Button button)
    {
        foreach (var pair in buttonSpritePairs)
        {
            if (pair.button == button)
            {
                return pair;
            }
        }
        return null;
    }

    private void SelectButton(ButtonSpritePair pair)
    {
        // Set the new selected button
        selectedButton = pair.button;

        // Change the sprite of the clicked button to the "pressed" sprite
        if (pair.pressedSprite != null)
        {
            pair.button.GetComponent<Image>().sprite = pair.pressedSprite;
        }
    }

    private void DeselectButton(ButtonSpritePair pair)
    {
        // Reset the button's sprite to the "not pressed" sprite
        if (pair.notPressedSprite != null)
        {
            pair.button.GetComponent<Image>().sprite = pair.notPressedSprite;
        }

        selectedButton = null;
    }

    public void OnBackButtonClickSingleSelection()
    {
        foreach (var pair in buttonSpritePairs)
        {
            DeselectButton(pair);
            pair.button.interactable = true;
        }
    }



    public void OnBackButtonClickMultiSelection()
    {
        foreach (var pair in buttonSpritePairs)
        {
            if (pair.button != null && pair.button.GetComponent<Image>().sprite == pair.pressedSprite)
            {
                DeselectButton(pair);
                pair.button.interactable = true;
            }
        }
    }

}
