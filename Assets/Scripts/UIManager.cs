using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Tower Placement")]
    public TowerPlacement TowerPlacement;
    public GameObject towerUIPanel;  // タワー選択パネル（TowerSelectPanel）
    public TextMeshProUGUI towerCostText; // タワーコスト表示用（オプション）
    private Vector2 selectedGridPosition;
    
    [Header("Tower Upgrade")]
    public GameObject towerUpgradePanel;  // タワーアップグレードパネル（TowerUpgradePanel）
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI statsText;
    public Button upgradeButton;
    private TowerUpgrade selectedTower;
    
    private void Start()
    {
        // アップグレードボタンのイベント設定
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnClickUpgradeButton);
        }
        
        // 初期状態でパネルを非表示
        if (towerUIPanel != null)
            towerUIPanel.SetActive(false);
        if (towerUpgradePanel != null)
            towerUpgradePanel.SetActive(false);
    }
    
    // グリッドクリック時（タワー配置用）
    public void ShowTowerUI(Vector2 gridPos)
    {
        selectedGridPosition = gridPos;
        
        // タワーアップグレードパネルを閉じる
        if (towerUpgradePanel != null)
            towerUpgradePanel.SetActive(false);
        
        // タワー選択パネルを表示
        if (towerUIPanel != null)
        {
            towerUIPanel.SetActive(true);
            
            // タワーコストを表示（オプション）
            if (towerCostText != null && TowerPlacement != null)
            {
                int cost = TowerPlacement.GetTowerCost();
                int currentMoney = GameManager.instance != null ? GameManager.instance.GetCurrentMoney() : 0;
                
                towerCostText.text = $"Cost: {cost}G (You have: {currentMoney}G)";
                
                // お金が足りない場合は赤色に
                if (currentMoney < cost)
                {
                    towerCostText.color = Color.red;
                }
                else
                {
                    towerCostText.color = Color.white;
                }
            }
        }
        else
        {
            Debug.LogError("towerUIPanel is not assigned in UIManager.");
        }
    }
    
    // タワークリック時（アップグレード用）
    public void ShowTowerUpgradeUI(TowerUpgrade tower)
    {
        // 前に選択されていたタワーの選択を解除
        if (selectedTower != null && selectedTower != tower)
        {
            selectedTower.DeselectEffect();
        }
        
        selectedTower = tower;
        
        // タワー選択パネルを閉じる
        if (towerUIPanel != null)
            towerUIPanel.SetActive(false);
        
        // タワーアップグレードパネルを表示
        if (towerUpgradePanel != null)
        {
            towerUpgradePanel.SetActive(true);
            UpdateUpgradeUI();
        }
        else
        {
            Debug.LogError("towerUpgradePanel is not assigned in UIManager.");
        }
    }
    
    // アップグレードUI更新
    private void UpdateUpgradeUI()
    {
        if (selectedTower == null) return;
        
        // GameManagerの存在確認
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager.instance is null!");
            return;
        }
        
        int currentLevel = selectedTower.GetCurrentLevel();
        
        // アップグレードレベル配列の確認
        if (selectedTower.upgradeLevels == null || selectedTower.upgradeLevels.Length == 0)
        {
            Debug.LogError("Tower has no upgrade levels configured!");
            return;
        }
        
        if (currentLevel < selectedTower.upgradeLevels.Length)
        {
            UpgradeLevel current = selectedTower.upgradeLevels[currentLevel];
            
            // 現在のレベル情報を表示
            if (levelText != null)
            {
                levelText.text = $"Level {currentLevel + 1}: {current.levelName}";
            }
            
            // 次のレベルがある場合
            if (currentLevel < selectedTower.upgradeLevels.Length - 1)
            {
                UpgradeLevel next = selectedTower.upgradeLevels[currentLevel + 1];
                
                if (costText != null)
                {
                    costText.text = $"Upgrade Cost: {next.upgradeCost}G";
                }
                
                if (statsText != null)
                {
                    statsText.text = $"Attack Rate: {current.attackRate} → {next.attackRate}\n" +
                                   $"Range: {current.detectionRange} → {next.detectionRange}\n" +
                                   $"Damage: {current.damage} → {next.damage}";
                }
                
                // ボタンを有効化
                if (upgradeButton != null)
                {
                    upgradeButton.interactable = true;
                    
                    // お金が足りない場合は無効化
                    if (GameManager.instance != null && GameManager.instance.GetCurrentMoney() < next.upgradeCost)
                    {
                        upgradeButton.interactable = false;
                    }
                }
            }
            else
            {
                // 最大レベルの場合
                if (costText != null)
                    costText.text = "MAX LEVEL";
                    
                if (statsText != null)
                {
                    statsText.text = $"Attack Rate: {current.attackRate}\n" +
                                   $"Range: {current.detectionRange}\n" +
                                   $"Damage: {current.damage}";
                }
                
                // ボタンを無効化
                if (upgradeButton != null)
                    upgradeButton.interactable = false;
            }
        }
    }
    
    // タワー配置ボタンクリック時
    public void OnClickPlaceButton()
    {
        if (TowerPlacement != null)
        {
            // お金を消費してタワーを配置
            TowerPlacement.PlaceTowerAtGrid(selectedGridPosition);
            
            // 配置したグリッドセルを占有状態にする
            GridCell[] gridCells = FindObjectsOfType<GridCell>();
            foreach (GridCell cell in gridCells)
            {
                if (cell.gridPosition == selectedGridPosition)
                {
                    cell.SetOccupied(true);
                    break;
                }
            }
            
            TowerPlacement.enabled = true; // 再びタワー操作有効にする
        }
        else
        {
            Debug.LogError("TowerPlacement is not assigned in UIManager.");
        }

        if (towerUIPanel != null)
        {
            towerUIPanel.SetActive(false);
        }
    }
    
    // アップグレードボタンクリック時
    public void OnClickUpgradeButton()
    {
        if (selectedTower != null)
        {
            selectedTower.AttemptUpgrade();
            UpdateUpgradeUI(); // UIを更新
        }
    }
    
    // パネルを閉じる（×ボタンなど用）
    public void CloseAllPanels()
    {
        if (towerUIPanel != null)
            towerUIPanel.SetActive(false);
        if (towerUpgradePanel != null)
            towerUpgradePanel.SetActive(false);
            
        // 選択されていたタワーの選択エフェクトを解除
        if (selectedTower != null)
        {
            selectedTower.DeselectEffect();
            selectedTower = null;
        }
            
        // タワー操作を再度有効化
        if (TowerPlacement != null)
            TowerPlacement.enabled = true;
    }
    
    // 外部クリック時にパネルを閉じる
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // UIをクリックしていない場合
            if (!IsPointerOverUIElement())
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                
                // 何もヒットしなかった場合、パネルを閉じる
                if (hit.collider == null)
                {
                    CloseAllPanels();
                }
            }
        }
    }
    
    // UI要素の上にマウスがあるかチェック
    private bool IsPointerOverUIElement()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }
}
