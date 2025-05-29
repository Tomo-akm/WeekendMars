using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Vector2 gridPosition;
    public UIManager uiManager;

    private void Awake()
    {
        // 自動的にグリッド位置を設定（整数に丸めることで正確な位置を保証）
        gridPosition = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }

    private void OnMouseDown()
    {
        uiManager.ShowTowerUI(gridPosition);

        // タワー操作を停止（タワーがドラッグされないように）
        if (uiManager.TowerPlacement != null)
        {
            uiManager.TowerPlacement.enabled = false;
        }
    }
}