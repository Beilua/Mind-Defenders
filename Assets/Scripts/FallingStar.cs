using UnityEngine;
using UnityEngine.UI;

public class FallingStar : MonoBehaviour
{
    public float fallSpeed = 800f;
    public float lifetimeAfterLanding = 3f; // Seconds before star disappears if not clicked
    public int starValue = 1; // Amount added when clicked

    private float targetY;
    private bool hasLanded = false;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // Add button click listener automatically
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnStarClicked);
        }
    }

    public void SetupTargetY(float landY)
    {
        targetY = landY;
    }

    void Update()
    {
        // Move downwards until reaching the random target Y level
        if (!hasLanded)
        {
            rectTransform.anchoredPosition -= new Vector2(0, fallSpeed * Time.deltaTime);

            if (rectTransform.anchoredPosition.y <= targetY)
            {
                hasLanded = true;
                // Destroy after sitting on screen for a while
                Destroy(gameObject, lifetimeAfterLanding); 
            }
        }
    }

    void OnStarClicked()
    {
        // Add currency to Manager
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.AddStars(starValue);
        }

        // Delete the star object
        Destroy(gameObject);
    }
}