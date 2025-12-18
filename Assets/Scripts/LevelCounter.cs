using TMPro;
using UnityEngine;

public class LevelCounter : MonoBehaviour
{
    private EnemySpawn machine;
    public static LevelCounter Instance;
    [SerializeField] private TMP_Text levelCounter;

    public int LevelCount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    // Update is called once per frame


    public void AddLevel()
    {

        LevelCount++;
        UpdateUI();
    }

    private void UpdateUI()
    {
       if (levelCounter != null)
       {
            levelCounter.text = "Round: " + LevelCount;
       }

    }
}
