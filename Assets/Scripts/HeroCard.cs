using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Hero Settings")]
    public GameObject heroPrefab; 
    public int starCost = 50;

    [Header("UI References")]
    public Canvas canvas; 
    public Image heroIconImage; // Assign child Hero Icon image if applicable

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip errorSound;
    public AudioClip successSound;

    private GameObject ghostHero;

    void Start()
    {
        // Auto-assign Canvas if missing (crucial for dynamically created cards)
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Ensure Canvas reference exists
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        // Check if player has enough stars before allowing drag
        if (ResourceManager.Instance != null && ResourceManager.Instance.currentStars < starCost)
        {
            PlayErrorSound();
            return;
        }

        // Create drag preview
        ghostHero = new GameObject("HeroGhost");
        ghostHero.transform.SetParent(canvas.transform, false);
        ghostHero.transform.SetAsLastSibling();

        Image ghostImage = ghostHero.AddComponent<Image>();
        
        // Dynamic Sprite Resolution: Finds the actual hero sprite even on dynamic cards
        Sprite spriteToUse = GetHeroSprite();
        if (spriteToUse != null) 
        {
            ghostImage.sprite = spriteToUse;
        }

        ghostImage.raycastTarget = false; // Prevents blocking pointer rays
        ghostHero.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);

        UpdateGhostPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostHero != null)
        {
            UpdateGhostPosition(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ghostHero != null)
        {
            // Check if pointer is over a valid grid Tile
            Tile targetTile = GetTileUnderPointer(eventData);

            if (targetTile != null && !targetTile.isOccupied)
            {
                // Attempt to spend stars
                if (ResourceManager.Instance != null && ResourceManager.Instance.SpendStars(starCost))
                {
                    // Spawn hero as child of the Tile so it centers automatically
                    GameObject newHero = Instantiate(heroPrefab, targetTile.transform);
                    RectTransform heroRect = newHero.GetComponent<RectTransform>();
                    heroRect.anchoredPosition = Vector2.zero; // Snaps directly to tile center

                    // Mark tile as taken
                    targetTile.isOccupied = true;

                    // Play success sound
                    PlaySuccessSound();
                }
                else
                {
                    PlayErrorSound();
                }
            }
            else
            {
                // Dropped outside grid OR on an already occupied spot
                PlayErrorSound();
            }

            Destroy(ghostHero);
        }
    }

    private Sprite GetHeroSprite()
    {
        // 1. Check explicit heroIconImage reference
        if (heroIconImage != null && heroIconImage.sprite != null)
        {
            return heroIconImage.sprite;
        }

        // 2. Search child objects for the hero portrait image
        Image[] childImages = GetComponentsInChildren<Image>();
        foreach (Image img in childImages)
        {
            if (img.gameObject != gameObject && img.sprite != null)
            {
                return img.sprite;
            }
        }

        // 3. Fallback to main object Image component
        Image rootImage = GetComponent<Image>();
        if (rootImage != null && rootImage.sprite != null)
        {
            return rootImage.sprite;
        }

        return null;
    }

    private Tile GetTileUnderPointer(PointerEventData eventData)
    {
        foreach (GameObject hoveredObj in eventData.hovered)
        {
            Tile tile = hoveredObj.GetComponent<Tile>();
            if (tile != null)
            {
                return tile;
            }
        }
        return null;
    }

    private void UpdateGhostPosition(PointerEventData eventData)
    {
        if (canvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );
        ghostHero.GetComponent<RectTransform>().anchoredPosition = localPoint;
    }

    private void PlayErrorSound()
    {
        if (audioSource != null && errorSound != null)
        {
            audioSource.PlayOneShot(errorSound);
        }
    }

    private void PlaySuccessSound()
    {
        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }
    }
}