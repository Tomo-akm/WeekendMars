using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [Header("Tower Settings")]
    public GameObject towerPrefab;
    public float minDistance = 1.0f;
    public int maxTowers = 10;
    public int towerCost = 10; // タワー配置のコスト
    
    [Header("Placement Mode")]
    public bool useGridCells = true; // グリッドセルを使用するかどうか
    
    private Camera mainCamera;
    private UIManager uiManager;
    
    void Start()
    {
        mainCamera = Camera.main;
        uiManager = FindFirstObjectByType<UIManager>();
        
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found in scene!");
        }
    }
    
    // UIManagerから呼ばれる配置メソッド（グリッドセル使用時）
    public void PlaceTowerAtGrid(Vector2 gridPosition)
    {
        // お金をチェック
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager.instance is null!");
            return;
        }
        
        if (GameManager.instance.GetCurrentMoney() < towerCost)
        {
            Debug.Log("Not enough money to place tower!");
            return;
        }
        
        // タワー数制限をチェック
        if (GetTowerCount() >= maxTowers)
        {
            Debug.Log($"Maximum number of towers ({maxTowers}) reached!");
            if (uiManager != null)
            {
                uiManager.ShowTowerLimitPopup(); // タワー上限ポップアップを表示
            }
            return;
        }
        
        // 対応するGridCellを見つけて実際のtransform.positionを取得
        GridCell targetCell = FindGridCellAtPosition(gridPosition);
        if (targetCell == null)
        {
            Debug.LogError("GridCell not found at position: " + gridPosition);
            return;
        }
        
        Vector3 actualPosition = targetCell.transform.position;
        
        // 配置可能かチェック
        if (!CanPlaceTower(actualPosition))
        {
            Debug.Log("Cannot place tower at this position!");
            return;
        }
        
        // お金を消費
        if (GameManager.instance.SpendMoney(towerCost))
        {
            // タワーを実際の位置に配置
            PlaceTower(actualPosition);
            Debug.Log($"Tower placed! Cost: {towerCost}G");
        }
    }

    // gridPositionに対応するGridCellを見つける
    private GridCell FindGridCellAtPosition(Vector2 gridPosition)
    {
        GridCell[] allGridCells = Object.FindObjectsByType<GridCell>(FindObjectsSortMode.None);
        
        foreach (GridCell cell in allGridCells)
        {
            if (cell.gridPosition == gridPosition)
            {
                return cell;
            }
        }
        
        return null;
    }
    
    // タワーを配置できるかチェック
    bool CanPlaceTower(Vector2 position)
    {
        // すべてのタワーを取得
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        
        // 既存のタワーとの距離をチェック
        foreach (GameObject tower in towers)
        {
            float distance = Vector2.Distance(tower.transform.position, position);
            if (distance < minDistance)
            {
                return false;
            }
        }
        
        // グリッドセルに既にタワーがあるかチェック
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Tower"))
            {
                return false;
            }
        }
        
        return true;
    }

    // タワーを配置
    public void PlaceTower(Vector2 position)
    {
        // タワーを指定位置に生成
        GameObject tower = Instantiate(towerPrefab, position, Quaternion.identity);
        // タワーにTagを設定
        if (tower.tag != "Tower")
        {
            tower.tag = "Tower";
        }
        // 画像の色をリセット（赤みや色の異常を防ぐ）
        var sr = tower.GetComponent<SpriteRenderer>();
        sr.color = Color.white;
        
        Debug.Log("Tower placed at: " + position);
    }
    
    // 現在のタワー数を取得（外部からアクセス可能に変更）
    public int GetTowerCount()
    {
        return GameObject.FindGameObjectsWithTag("Tower").Length;
    }
    
    // タワー配置可能かチェック（UI表示用）
    public bool CanAffordTower()
    {
        return GameManager.instance != null && GameManager.instance.GetCurrentMoney() >= towerCost;
    }
    
    // タワーコストを取得（UI表示用）
    public int GetTowerCost()
    {
        return towerCost;
    }
}
