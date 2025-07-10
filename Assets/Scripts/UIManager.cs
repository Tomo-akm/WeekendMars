using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Tower Placement")]
    public TowerPlacement TowerPlacement;
    public GameObject towerUIPanel;  // タワー選択パネル（TowerSelectPanel）
    public TextMeshProUGUI towerNameText;     // タワー名表示用
    public TextMeshProUGUI towerStatsText;    // タワーステータス表示用
    public TextMeshProUGUI towerCostText;     // タワーコスト表示用
    public Button placeButton;                 // タワー配置ボタン
    private Vector2 selectedGridPosition;
    
    [Header("Tower Upgrade")]
    public GameObject towerUpgradePanel;  // タワーアップグレードパネル（TowerUpgradePanel）
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI statsText;
    public Button upgradeButton;
    private TowerUpgrade selectedTower;
    
    [Header("Tower Limit Popup")]
    public GameObject towerLimitPopupPanel; // タワー上限ポップアップ用パネル
    public TextMeshProUGUI towerLimitPopupText; // ポップアップメッセージ用
    public float popupDuration = 2f; // ポップアップ表示時間（秒）
    private Coroutine popupCoroutine;
    
    private void Start()
    {
        // アップグレードボタンのイベント設定
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnClickUpgradeButton);
        }
        
        // 配置ボタンのイベント設定
        if (placeButton != null)
        {
            placeButton.onClick.AddListener(OnClickPlaceButton);
        }
        
        // 初期状態でパネルを非表示
        if (towerUIPanel != null)
            towerUIPanel.SetActive(false);
        if (towerUpgradePanel != null)
            towerUpgradePanel.SetActive(false);
        if (towerLimitPopupPanel != null)
            towerLimitPopupPanel.SetActive(false);
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
            // タワー数上限チェック
            if (TowerPlacement != null && TowerPlacement.GetTowerCount() >= TowerPlacement.maxTowers)
            {
                ShowTowerLimitPopup();
                if (towerUIPanel != null)
                    towerUIPanel.SetActive(false);
                return;
            }
            
            towerUIPanel.SetActive(true);
            
            // タワー情報を表示
            if (TowerPlacement != null && TowerPlacement.towerPrefab != null)
            {
                TowerUpgrade towerUpgrade = TowerPlacement.towerPrefab.GetComponent<TowerUpgrade>();
                
                if (towerUpgrade != null && towerUpgrade.upgradeLevels != null && towerUpgrade.upgradeLevels.Length > 0)
                {
                    UpgradeLevel firstLevel = towerUpgrade.upgradeLevels[0];
                    
                    // タワー名表示
                    if (towerNameText != null)
                    {
                        towerNameText.text = $"レベル1: {firstLevel.levelName}";
                    }
                    
                    // タワーステータス表示（1行）
                    if (towerStatsText != null)
                    {
                        towerStatsText.text = $"威力: {firstLevel.damage}　射程距離: {firstLevel.detectionRange}　攻撃速度: {firstLevel.attackRate:F1}";
                    }
                }
                
                // タワーコストを表示
                if (towerCostText != null)
                {
                    int cost = TowerPlacement.GetTowerCost();
                    int currentMoney = GameManager.instance != null ? GameManager.instance.GetCurrentMoney() : 0;
                    
                    towerCostText.text = $"コスト: {cost}G";
                    
                    // お金が足りない場合は赤色に
                    if (currentMoney < cost)
                    {
                        towerCostText.color = Color.red;
                        // 配置ボタンを無効化
                        if (placeButton != null)
                            placeButton.interactable = false;
                    }
                    else
                    {
                        towerCostText.color = Color.white;
                        // 配置ボタンを有効化
                        if (placeButton != null)
                            placeButton.interactable = true;
                    }
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
                levelText.text = $"レベル{currentLevel + 1}: {current.levelName}";
            }
            
            // 次のレベルがある場合
            if (currentLevel < selectedTower.upgradeLevels.Length - 1)
            {
                UpgradeLevel next = selectedTower.upgradeLevels[currentLevel + 1];
                
                // 次のレベル名を含めたテキスト表示
                if (levelText != null)
                {
                    levelText.text = $"レベル{currentLevel + 1}: {current.levelName} => レベル{currentLevel + 2}: {next.levelName}";
                }
                
                if (costText != null)
                {
                    costText.text = $"コスト: {next.upgradeCost}G";
                }
                
                if (statsText != null)
                {
                    // 射程距離と攻撃速度の倍率を計算
                    float rangeMultiplier = next.detectionRange / current.detectionRange;
                    float attackRateMultiplier = next.attackRate / current.attackRate;
                    
                    statsText.text = $"威力: {current.damage} => {next.damage}　射程距離: {rangeMultiplier:F1}倍　攻撃速度: {attackRateMultiplier:F1}倍";
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
                if (levelText != null)
                {
                    levelText.text = $"レベル{currentLevel + 1}: {current.levelName} (最大レベル)";
                }
                
                if (costText != null)
                    costText.text = "最大レベル";
                    
                if (statsText != null)
                {
                    statsText.text = $"威力: {current.damage}　射程距離: {current.detectionRange}　攻撃速度: {current.attackRate:F1}";
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
            GridCell[] gridCells = Object.FindObjectsByType<GridCell>(FindObjectsSortMode.None);
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
        if (towerLimitPopupPanel != null)
            towerLimitPopupPanel.SetActive(false);
            
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
        // アップグレードパネルが開いていて選択中のタワーがある場合、ボタン状態を常に更新
        if (towerUpgradePanel != null && towerUpgradePanel.activeSelf && selectedTower != null)
        {
            UpdateUpgradeUI();
        }
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

    // タワー上限エラーポップアップ表示
    public void ShowTowerLimitPopup()
    {
        if (towerLimitPopupPanel != null && towerLimitPopupText != null)
        {
            towerLimitPopupText.text = $"タワーは{TowerPlacement.maxTowers}個までしか置けません";
            towerLimitPopupPanel.SetActive(true);
            if (popupCoroutine != null) StopCoroutine(popupCoroutine);
            popupCoroutine = StartCoroutine(HideTowerLimitPopupAfterDelay());
        }
    }

    private System.Collections.IEnumerator HideTowerLimitPopupAfterDelay()
    {
        yield return new WaitForSeconds(popupDuration);
        if (towerLimitPopupPanel != null)
            towerLimitPopupPanel.SetActive(false);
    }
}
