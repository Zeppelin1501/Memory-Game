using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PrefabInstantiator : MonoBehaviour
{
    [System.Serializable]
    public class ButtonPrefabSet
    {
        public Button button;
        public GameObject[] prefabs;
    }

    public ButtonPrefabSet[] buttonPrefabSets; // Public for Unity Editor assignment
    public RectTransform[] targetPositions; // Public for Unity Editor assignment
    public GameObject[] collidableImages; // Public for Unity Editor assignment
    public Button retakeButton; // Public for Unity Editor assignment

    private List<GameObject> instantiatedPrefabs = new List<GameObject>();

    private void Start()
    {
        // Add listeners to the buttons
        for (int i = 0; i < buttonPrefabSets.Length; i++)
        {
            int index = i; // Capture the current index
            buttonPrefabSets[i].button.onClick.AddListener(() => OnButtonClicked(index));
        }

        // Add BoxCollider2D to collidable images
        /* foreach (GameObject image in collidableImages)
         {
             if (image.GetComponent<BoxCollider2D>() == null)
             {
                 image.AddComponent<BoxCollider2D>();
             }
         }*/

        // Add listener to retake button
        retakeButton.onClick.AddListener(RetakePhotos);
    }

    private void OnButtonClicked(int index)
    {
        if (index < 0 || index >= buttonPrefabSets.Length || targetPositions.Length == 0)
        {
            Debug.LogWarning("Invalid button index or no target positions assigned!");
            return;
        }

        ButtonPrefabSet set = buttonPrefabSets[index];

        // Ensure that there are prefabs assigned for this button
        if (set.prefabs.Length == 0)
        {
            Debug.LogWarning("No prefabs assigned for button at index " + index);
            return;
        }

        // Instantiate each prefab in the array at a random target position
        foreach (GameObject prefab in set.prefabs)
        {
            RectTransform targetPosition = targetPositions[Random.Range(0, targetPositions.Length)];
            GameObject instantiatedPrefab = Instantiate(prefab, targetPosition);
            instantiatedPrefabs.Add(instantiatedPrefab); // Track instantiated objects

            RectTransform instantiatedRectTransform = instantiatedPrefab.GetComponent<RectTransform>();

            // Ensure the instantiated prefab has necessary components
            if (instantiatedPrefab.GetComponent<BoxCollider2D>() == null)
            {
                instantiatedPrefab.AddComponent<BoxCollider2D>();
            }
            if (instantiatedPrefab.GetComponent<Rigidbody2D>() == null)
            {
                var rb = instantiatedPrefab.AddComponent<Rigidbody2D>();
                rb.isKinematic = true; // Prevent physics forces but allow collision detection
            }

            if (instantiatedRectTransform != null)
            {
                instantiatedRectTransform.anchoredPosition = Vector2.zero; // Center within the target
                instantiatedRectTransform.localScale = Vector3.one; // Reset scale to avoid issues
            }
            else
            {
                Debug.LogWarning("Instantiated prefab does not have a RectTransform component.");
            }

            // Add CollisionHandler component to the instantiated prefab
            if (instantiatedPrefab.GetComponent<CollisionHandler>() == null)
            {
                instantiatedPrefab.AddComponent<CollisionHandler>();
            }
        }
    }

    private void RetakePhotos()
    {
        // Destroy all instantiated prefabs
        foreach (GameObject prefab in instantiatedPrefabs)
        {
             Destroy(prefab);
        }

        // Clear the list
        instantiatedPrefabs.Clear();
    }
}

public class CollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object has a tag "Wall"
        if (collision.gameObject.CompareTag("Walls"))
        {
            // Handle the collision
            Debug.Log($"{gameObject.name} collided with {collision.gameObject.name}");

            // Destroy the current game object
            Destroy(gameObject);
        }
    }
}