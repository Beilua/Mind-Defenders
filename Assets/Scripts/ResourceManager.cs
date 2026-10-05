using System.Collections;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI starCounterText; // Drag your StarCounter text object here
    public GameObject starPrefab;           // Drag your Star UI Prefab here
    public RectTransform canvasTransform;   // Drag your Canvas object here

    [Header("Currency Settings")]
    public int currentStars = 0;

    [Header("Star Spawning Bounds (UI Anchored Positions)")]
    public float spawnInterval = 1f; // Spawns a star every 7 seconds
    public float spawnY = 600;      // Y height where star spawns at top
    public float minX = -1000f;       // Leftmost random spawn position
    public float maxX = 1000f;        // Rightmost random spawn position
    public float minY = -600;       // Lowest Y height star can land
    public float maxY = 600;        // Highest Y height star can land

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentStars = BattleData.CurrentStars;
        UpdateUI();
        StartCoroutine(SpawnStarRoutine());
    }

    IEnumerator SpawnStarRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnStar();
        }
    }

    public void SpawnStar()
    {
        // Pick a random X spawn location and random Y landing location
        float randomX = Random.Range(minX, maxX);
        float randomLandY = Random.Range(minY, maxY);

        // Instantiate inside Canvas
        GameObject newStar = Instantiate(starPrefab, canvasTransform);
        RectTransform starRect = newStar.GetComponent<RectTransform>();

        // Set starting position at top of screen
        starRect.anchoredPosition = new Vector2(randomX, spawnY);

        // Tell the star script where to stop falling
        FallingStar starScript = newStar.GetComponent<FallingStar>();
        if (starScript != null)
        {
            starScript.SetupTargetY(randomLandY);
        }
    }

    public void AddStars(int amount)
    {
        currentStars += amount;
        BattleData.CurrentStars = currentStars;
        UpdateUI();
    }

    public bool SpendStars(int amount)
    {
        if (currentStars >= amount)
        {
            currentStars -= amount;
            BattleData.CurrentStars = currentStars;
            UpdateUI();
            return true; // Successfully purchased
        }
        return false; // Not enough stars
    }

    private void UpdateUI()
    {
        if (starCounterText != null)
        {
            starCounterText.text = currentStars.ToString();
        }
    }
}