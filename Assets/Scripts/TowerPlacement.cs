using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerPlacement : MonoBehaviour
{
    [Header("タワー設置設定")]
    public GameObject towerPrefab; // 設置するタワーのプレハブ
    public int towerCost = 30; // タワーの設置費用
    public LayerMask placementLayer; // 設置可能なレイヤー
    
    [Header("UI要素")]
    public Button placeTowerButton; // タワー設置ボタン
    public TextMeshProUGUI towerCostText; // 費用表示
    
    private bool isPlacingTower = false;
    private GameObject previewTower; // プレビュー用のタワー
    
    private void Start()
    {
        // ボタンのイベント設定
        if (placeTowerButton != null)
        {
            placeTowerButton.onClick.AddListener(OnPlaceTowerButtonClick);
            UpdateButtonState();
        }
        
        // 費用表示を更新
        if (towerCostText != null)
        {
            towerCostText.text = $"タワー設置: {towerCost}G";
        }
    }
    
    private void Update()
    {
        if (isPlacingTower)
        {
            UpdateTowerPlacement();
        }
        
        // UIの更新（お金が変わった時のため）
        UpdateButtonState();
    }
    
    private void OnPlaceTowerButtonClick()
    {
        if (GameManager.instance.GetCurrentMoney() >= towerCost)
        {
            StartTowerPlacement();
        }
    }
    
    private void StartTowerPlacement()
    {
        isPlacingTower = true;
        
        // プレビュー用タワーを作成
        previewTower = Instantiate(towerPrefab);
        previewTower.GetComponent<Tower>().enabled = false; // 射撃を無効化
        previewTower.GetComponent<TowerUpgrade>().enabled = false; // アップグレードを無効化
        
        // 半透明にする
        SpriteRenderer sr = previewTower.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color color = sr.color;
            color.a = 0.5f;
            sr.color = color;
        }
    }
    
    private void UpdateTowerPlacement()
    {
        // マウス位置を取得
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        
        // グリッドにスナップ（オプション）
        mousePos.x = Mathf.Round(mousePos.x);
        mousePos.y = Mathf.Round(mousePos.y);
        
        // プレビュータワーを移動
        if (previewTower != null)
        {
            previewTower.transform.position = mousePos;
            
            // 設置可能かチェック
            bool canPlace = CanPlaceAtPosition(mousePos);
            
            // 色を変更
            SpriteRenderer sr = previewTower.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color color = canPlace ? Color.green : Color.red;
                color.a = 0.5f;
                sr.color = color;
            }
        }
        
        // 左クリックで設置
        if (Input.GetMouseButtonDown(0))
        {
            if (CanPlaceAtPosition(mousePos))
            {
                PlaceTower(mousePos);
            }
        }
        
        // 右クリックでキャンセル
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }
    
    private bool CanPlaceAtPosition(Vector3 position)
    {
        // 設置位置に他のタワーがないかチェック
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.4f);
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Tower"))
                return false;
        }
        
        // 設置可能エリアかチェック（レイヤーマスクを使用）
        if (placementLayer != 0)
        {
            Collider2D hit = Physics2D.OverlapPoint(position, placementLayer);
            return hit != null;
        }
        
        return true;
    }
    
    private void PlaceTower(Vector3 position)
    {
        // お金を消費
        if (GameManager.instance.SpendMoney(towerCost))
        {
            // タワーを設置
            GameObject newTower = Instantiate(towerPrefab, position, Quaternion.identity);
            newTower.tag = "Tower"; // タグを設定
            
            Debug.Log($"タワーを設置しました: {position}");
        }
        
        // 設置モードを終了
        CancelPlacement();
    }
    
    private void CancelPlacement()
    {
        isPlacingTower = false;
        
        // プレビュータワーを削除
        if (previewTower != null)
        {
            Destroy(previewTower);
            previewTower = null;
        }
    }
    
    private void UpdateButtonState()
    {
        // お金が足りるかチェックしてボタンを有効/無効化
        if (placeTowerButton != null)
        {
            placeTowerButton.interactable = GameManager.instance.GetCurrentMoney() >= towerCost;
        }
    }
}
