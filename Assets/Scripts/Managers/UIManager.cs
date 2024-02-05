using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [Header("Panels")]
    [SerializeField] private GameObject turretShopPanel;
    [SerializeField] private GameObject nodeUIPanel;
    [SerializeField] private GameObject achievementPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameStartPanel;

    [Header("Text")] 
    [SerializeField] private TextMeshProUGUI upgradeText;
    [SerializeField] private TextMeshProUGUI sellText;
    [SerializeField] private TextMeshProUGUI turretLevelText;
    [SerializeField] private TextMeshProUGUI totalCoinsText;
    [SerializeField] private TextMeshProUGUI lifesText;
    [SerializeField] private TextMeshProUGUI currentWaveText;
    [SerializeField] private TextMeshProUGUI gameOverTotalCoinsText;
    
    private Node _currentNodeSelected;

    private void Start()
    {
        ShowGameStartPanel();
    }

    private void Update()
    {
        totalCoinsText.text = CurrencySystem.Instance.TotalCoins.ToString();
        lifesText.text = LevelManager.Instance.TotalLives.ToString();
        currentWaveText.text = $"Wave {LevelManager.Instance.CurrentWave}";
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
        gameOverTotalCoinsText.text = CurrencySystem.Instance.TotalCoins.ToString();
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
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        // _currentNodeSelected.CloseAttackRangeSprite();
        nodeUIPanel.SetActive(false);
    }

    public void EditTurret()
    {
        _currentNodeSelected.Turret.SetIsEditing(true);
        CloseNodeUIPanel();
    }
    
    public void UpgradeTurret()
    {
        // _currentNodeSelected.Turret.TurretUpgrade.UpgradeTurret();
        _currentNodeSelected.Turret.UpgradeTurret();
        UpdateUpgradeText();
        UpdateTurretLevel();
        UpdateSellValue();
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
        UpdateUpgradeText();
        UpdateTurretLevel();
        UpdateSellValue();
    }

    private void UpdateUpgradeText()
    {
        // upgradeText.text = _currentNodeSelected.Turret.TurretUpgrade.UpgradeCost.ToString();
    }

    private void UpdateTurretLevel()
    {
        // turretLevelText.text = $"Level {_currentNodeSelected.Turret.TurretUpgrade.Level}";
    }

    private void UpdateSellValue()
    {
        // int sellAmount = _currentNodeSelected.Turret.TurretUpgrade.GetSellValue();
        // sellText.text = sellAmount.ToString();
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
    }

    private void OnDisable()
    {
        Node.OnNodeSelected -= NodeSelected;
    }
}
