using System;
using AarquieSolutions.Base.Singleton;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public EnemySpawnerManager enemySpawnerManager;
    public Gem gem;

    [SerializeField] private ScriptableLevel scriptableLevelGemCounts;

    [SerializeField] private int currentLevel = 0;

    [SerializeField] private int gemsCollected = 0;

    [SerializeField] public UpdateScreen updateScreen;
    public event Action OnPaused;
    public event Action OnResumed;
    public event Action OnSceneOver;

    public BoolEvent OnLevelup;

    [SerializeField] private FloatReferencer stageTimeReferencing;

    [SerializeField] private TextMeshProUGUI levlTxt;
    [SerializeField] private Image levelImage;

    [SerializeField] private TextMeshProUGUI timerTxt;
    public WeaponManager WeaponManager;
    private bool isSceneOver = false;
    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            GameManager.Instance = this;
        }
        OnLevelup.AddListener(OnLevelComplete);
    }
    public void Pause()
    {
        OnPaused?.Invoke();
        Time.timeScale = 0.0f;
    }

    public void Resume()
    {
        OnResumed?.Invoke();
        Time.timeScale = 1.0f;
    }

    public void SceneOver()
    {
        OnSceneOver?.Invoke();
        isSceneOver = true;
        Time.timeScale = 0.0f;
    }
    public void GameOver()
    {
        SceneOver();
    }

    private void Start()
    {
        levelImage.fillAmount = 0;
        levlTxt.text = $"Level {currentLevel}";
        stageTimeReferencing.Reference = 0;
    }
    private TimeSpan timerSpan;
    private void Update()
    {
        stageTimeReferencing.Reference += Time.deltaTime;
        timerSpan = TimeSpan.FromSeconds(stageTimeReferencing.Reference);
        timerTxt.text = $"{timerSpan.Minutes}:{timerSpan.Seconds}";
    }
    public void CollectedGems()
    {
        gemsCollected += 1;


        int gemTotal = 0;
        if (currentLevel >= scriptableLevelGemCounts.LeveList.Capacity)
        {
            gemTotal = scriptableLevelGemCounts.LeveList[scriptableLevelGemCounts.LeveList.Capacity - 1];
        }
        else
        {
            gemTotal = scriptableLevelGemCounts.LeveList[currentLevel];
        }
        if (gemTotal <= gemsCollected)
        {
            OnLevelup.Invoke(true);
            gemsCollected = 0;
            currentLevel += 1;
            levelImage.fillAmount = 0;
            levlTxt.text = $"Level {currentLevel}";
        }
        levelImage.fillAmount = Mathf.InverseLerp(0, gemTotal, gemsCollected);
    }
    void OnLevelComplete(bool val)
    {
        Time.timeScale = val ? 0 : 1;
        updateScreen.gameObject.SetActive(val);
        WeaponManager.UpdateStatus();
    }
}
