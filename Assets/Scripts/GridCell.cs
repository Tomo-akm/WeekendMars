using UnityEngine;

public class GridCell : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2 gridPosition;
    public bool isOccupied = false; // このセルにタワーがあるか
    
    [Header("Visual Settings")]
    public Color normalColor = new Color(1f, 1f, 1f, 0.3f);
    public Color hoverColor = new Color(0f, 1f, 0f, 0.5f);
    public Color occupiedColor = new Color(1f, 0f, 0f, 0.3f);
    
    private UIManager uiManager;
    private SpriteRenderer spriteRenderer;
    private bool isMouseOver = false;
    
    private void Awake()
    {
        // 自動的にグリッド位置を設定
        gridPosition = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        
        // スプライトレンダラーを取得または作成
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        // デフォルトのスプライトを設定（1x1の白い四角）
        if (spriteRenderer.sprite == null)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        }
        
        // 初期色を設定
        UpdateColor();
    }
    
    private void Start()
    {
        uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found in scene!");
        }
        
        // BoxCollider2Dを追加（クリック検出用）
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
        }
    }
    
    private void OnMouseEnter()
    {
        if (!isOccupied)
        {
            isMouseOver = true;
            UpdateColor();
        }
    }
    
    private void OnMouseExit()
    {
        isMouseOver = false;
        UpdateColor();
    }
    
    private void OnMouseDown()
    {
        // UIの上をクリックしていない場合のみ
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (!isOccupied && uiManager != null)
            {
                // タワー選択UIを表示
                uiManager.ShowTowerUI(gridPosition);
                
                // タワー操作を停止（タワーがドラッグされないように）
                if (uiManager.TowerPlacement != null)
                {
                    uiManager.TowerPlacement.enabled = false;
                }
            }
        }
    }
    
    // タワーが配置されたときに呼ばれる
    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
        UpdateColor();
    }
    
    // 色を更新
    private void UpdateColor()
    {
        if (spriteRenderer != null)
        {
            if (isOccupied)
            {
                spriteRenderer.color = occupiedColor;
            }
            else if (isMouseOver)
            {
                spriteRenderer.color = hoverColor;
            }
            else
            {
                spriteRenderer.color = normalColor;
            }
        }
    }
    
    // このセルにタワーがあるかチェック
    public void CheckOccupancy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Tower"))
            {
                SetOccupied(true);
                return;
            }
        }
        SetOccupied(false);
    }
}
