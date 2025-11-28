using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Ensure you have this namespace

public class ButtonImageManager : MonoBehaviour
{
    public Button[] buttons;
    public GameObject[] images;
    public GameObject[] buttonGameObjects; // Array to hold the button game objects

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the arrays are correctly set up
        if (buttons.Length != images.Length || buttons.Length != buttonGameObjects.Length || buttons.Length == 0)
        {
            Debug.LogError("Buttons, images, and buttonGameObjects arrays must be of the same length and not empty.");
            return;
        }

        // Set up the button click listeners
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Local copy for the closure
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }

        // Set the first button as clicked by default
        OnButtonClick(0);
    }

    public void OnButtonClick(int index)
    {
        // Deactivate all images and make all buttons interactable
        for (int i = 0; i < images.Length; i++)
        {
            images[i].SetActive(false);
            buttons[i].interactable = true;
            buttons[i].transform.localScale = Vector3.one; // Reset scale
            buttonGameObjects[i].SetActive(false); // Deactivate all button game objects
        }

        // Activate the selected image and make the button uninteractable
        images[index].SetActive(true);
        buttons[index].interactable = false;

        // Activate the selected button game object
        buttonGameObjects[index].SetActive(true);

        // Ensure the index[0] of images is also active
       // images[0].SetActive(true);

        // Add DoTween scaling effect
        buttons[index].transform.DOScale(1.2f, 0.2f).OnComplete(() =>
        {
            buttons[index].transform.DOScale(1f, 0.2f);
        });
    }
}
