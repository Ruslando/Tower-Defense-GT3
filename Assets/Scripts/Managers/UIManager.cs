using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Panels")]
    [SerializeField] private GameObject turretShopPanel;
    [SerializeField] private GameObject turretUpgradePanel;
    [SerializeField] private GameObject nodeUIPanel;
    [SerializeField] private GameObject achievementPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameStartPanel;
    [SerializeField] private GameObject gameSpeedPanel;
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private GameObject coinPanel;
    [SerializeField] private GameObject wavePanel;

    [Header("Text")] 
    [SerializeField] private TextMeshProUGUI upgradeText;
    [SerializeField] private TextMeshProUGUI improveText;
    [SerializeField] private TextMeshProUGUI sellText;
    [SerializeField] private TextMeshProUGUI turretLevelText;
    [SerializeField] private TextMeshProUGUI totalCoinsText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI currentWaveText;
    [SerializeField] private TextMeshProUGUI gameOverTimeText;

    [Header("Text")]
    [SerializeField] private Button improveButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;
    
    private Node _currentNodeSelected;

    private void HandleStartGame()
    {
        gameSpeedPanel.SetActive(true);
        timerPanel.SetActive(true);
        coinPanel.SetActive(true);
        wavePanel.SetActive(true);
    }

    private void HandleEndGame(Kart kart)
    {
        gameSpeedPanel.SetActive(false);
        timerPanel.SetActive(false);
        coinPanel.SetActive(false);
        wavePanel.SetActive(false);
    }

    private void Start()
    {
        ShowGameStartPanel();
    }

    private void Update()
    {
        totalCoinsText.text = CurrencySystem.Instance.TotalCoins.ToString();

        if(KartManager.Instance.CurrentLap < KartManager.Instance.GetMaxLaps())
        {
            currentWaveText.text = $"Lap: {KartManager.Instance.CurrentLap + 1}";
        } else {
            currentWaveText.text = $"Finished!";
        }

        UpdateTime();

        if(_currentNodeSelected != null)
        {
            UpdateButtons();
            UpdateUpgradeText();
            UpdateImproveText();
            UpdateTurretLevel();
            UpdateSellValue();
        }
    }

    private void UpdateTime()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(LevelManager.Instance.GetCurrentTime());
        string formattedTime = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        timeText.text = formattedTime;
    }

    private void UpdateButtons()
    {
        if(_currentNodeSelected != null)
        {
            if(_currentNodeSelected.Turret != null)
            {
                TurretImprovement improvementData = _currentNodeSelected.Turret.turretImprovements[0];
                improveButton.interactable = improvementData.CanBuyNextLevel() && !improvementData.IsMaxLevel();

                TurretUpgrade upgrade = _currentNodeSelected.Turret.turretUpgrade;
                upgradeButton.interactable = upgrade.CanBuyNextLevel() && !upgrade.IsMaxLevel();
            }
        }
    }

    public void SlowTime()
    {
        Time.timeScale = 0.5f;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    public void FastTime()
    {
        Time.timeScale = 2f;
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);

        // Convert currentTime to TimeSpan
        TimeSpan timeSpan = TimeSpan.FromSeconds(LevelManager.Instance.GetCurrentTime());

        gameOverTimeText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);;
    }

    public void HideGameOverPanel()
    {
        gameOverPanel.SetActive(false);
    }

    public void ShowGameStartPanel()
    {
        gameStartPanel.SetActive(true);
    }

    public void HideGameStartPanel()
    {
        gameStartPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        LevelManager.Instance.RestartGame();
        HideGameOverPanel();
        HideGameStartPanel();
    }
    
    public void OpenAchievementPanel(bool status)
    {
        achievementPanel.SetActive(status);
    }
    
    public void CloseTurretShopPanel()
    {
        turretShopPanel.SetActive(false);
    }

    public void CloseNodeUIPanel()
    {
        nodeUIPanel.SetActive(false);
    }

    public void EditTurret()
    {
        _currentNodeSelected.Turret.SetIsEditing(true);
        CloseNodeUIPanel();
    }

    public void ImproveTurret()
    {
        _currentNodeSelected.Turret.ImproveTurret();
    }
    
    public void UpgradeTurret()
    {
        _currentNodeSelected.Turret.UpgradeTurret();
    }

    public void SellTurret()
    {
        _currentNodeSelected.SellTurret();
        _currentNodeSelected = null;
        CloseNodeUIPanel();
    }
    
    private void ShowNodeUI()
    {
        nodeUIPanel.SetActive(true);
    }

    private void UpdateImproveText()
    {
        if(!_currentNodeSelected.Turret.turretImprovements[0].IsMaxLevel()){
            improveText.text = $"{_currentNodeSelected.Turret.turretImprovements[0].GetNextLevelCost()}";
        } else {
            improveText.text = "-";
        }
    }

    private void UpdateUpgradeText()
    {
        if(!_currentNodeSelected.Turret.turretUpgrade.IsMaxLevel())
        {
            upgradeText.text = _currentNodeSelected.Turret.turretUpgrade.GetNextLevelCost().ToString();
        } else {
            improveText.text = "-";
        }
    }

    private void UpdateTurretLevel()
    {
        int improvLevel = _currentNodeSelected.Turret.turretImprovements[0].currentLevel + 1;
        string turretType = _currentNodeSelected.Turret.turretUpgrade.GetCurrentTurretType().ToString();
        turretLevelText.text = $"{turretType} : Level {improvLevel}";
    }

    private void UpdateSellValue()
    {
        int sellAmount = _currentNodeSelected.Turret.GetResellValue();
        sellText.text = sellAmount.ToString();
    }
    
    private void NodeSelected(Node nodeSelected)
    {
        _currentNodeSelected = nodeSelected;
        if (_currentNodeSelected.IsEmpty())
        {
            turretShopPanel.SetActive(true);
        }
        else
        {
            ShowNodeUI();
        }
    }
    
    private void OnEnable()
    {
        Node.OnNodeSelected += NodeSelected;
        LevelManager.OnStartGame += HandleStartGame;
        Kart.OnAllLapsCompleted += HandleEndGame;
    }

    private void OnDisable()
    {
        Node.OnNodeSelected -= NodeSelected;
        LevelManager.OnStartGame -= HandleStartGame;
        Kart.OnAllLapsCompleted -= HandleEndGame;
    }
}
