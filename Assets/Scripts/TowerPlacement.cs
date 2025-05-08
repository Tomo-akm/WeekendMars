using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    // 配置するタワーのプレハブ
    public GameObject towerPrefab;
    
    // タワー同士の最小距離
    public float minDistance = 1.0f;
    
    // タワーの最大数
    public int maxTowers = 5;
    
    // 現在選択中のタワー
    private GameObject selectedTower = null;
    
    // カメラ参照
    private Camera mainCamera;
    
    void Start()
    {
        // メインカメラを取得
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        // マウスの位置をワールド座標に変換
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        // 選択中のタワーがある場合、マウスに追従
        if (selectedTower != null)
        {
            selectedTower.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
        }
        
        // マウスの左ボタンがクリックされたとき
        if (Input.GetMouseButtonDown(0))
        {
            // 選択中のタワーがある場合は配置
            if (selectedTower != null)
            {
                if (CanPlaceTower(mousePosition))
                {
                    // タワーを配置
                    selectedTower.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
                    selectedTower = null;
                    Debug.Log("タワーを配置しました");
                }
                else
                {
                    Debug.Log("この位置にはタワーを配置できません");
                }
            }
            else
            {
                // タワーを選択または新規配置
                GameObject clickedTower = GetTowerAtPosition(mousePosition);
                
                if (clickedTower != null)
                {
                    // 既存のタワーを選択
                    selectedTower = clickedTower;
                    Debug.Log("タワーを選択しました");
                }
                else if (GetTowerCount() < maxTowers)
                {
                    // 新規タワーを配置可能な場合
                    if (CanPlaceTower(mousePosition))
                    {
                        PlaceTower(mousePosition);
                    }
                    else
                    {
                        Debug.Log("この位置にはタワーを配置できません");
                    }
                }
                else
                {
                    Debug.Log("タワーの最大数（" + maxTowers + "）に達しています");
                }
            }
        }
        
        // 右クリックで選択キャンセル
        if (Input.GetMouseButtonDown(1) && selectedTower != null)
        {
            selectedTower = null;
            Debug.Log("タワーの選択を解除しました");
        }
    }
    
    // タワーを配置できるかチェック
    bool CanPlaceTower(Vector2 position)
    {
        // すべてのタワーを取得
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        
        // 既存のタワーとの距離をチェック
        foreach (GameObject tower in towers)
        {
            if (tower != selectedTower) // 選択中のタワー以外をチェック
            {
                float distance = Vector2.Distance(tower.transform.position, position);
                if (distance < minDistance)
                {
                    return false; // 近すぎるので配置不可
                }
            }
        }
        
        // 配置可能
        return true;
    }
    
    // タワーを配置
    void PlaceTower(Vector2 position)
    {
        // タワーを指定位置に生成
        GameObject tower = Instantiate(towerPrefab, position, Quaternion.identity);
        
        // タワーにTagを設定（検索用）
        tower.tag = "Tower";
        
        Debug.Log("タワーを配置: " + position);
    }
    
    // 指定位置にあるタワーを取得
    GameObject GetTowerAtPosition(Vector2 position)
    {
        // すべてのタワーを取得
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        
        // 一定の判定半径
        float pickRadius = 0.5f;
        
        // 各タワーとの距離をチェック
        foreach (GameObject tower in towers)
        {
            float distance = Vector2.Distance(tower.transform.position, position);
            if (distance < pickRadius)
            {
                return tower;
            }
        }
        
        return null;
    }

    // 現在のタワー数を取得
    int GetTowerCount()
    {
        return GameObject.FindGameObjectsWithTag("Tower").Length;
    }
}
