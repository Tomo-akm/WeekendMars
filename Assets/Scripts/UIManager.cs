using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TowerPlacement TowerPlacement;
    public GameObject towerUIPanel;  // UIパネルをセット（Canvas内にあること）
    private Vector2 selectedGridPosition;

    public void ShowTowerUI(Vector2 gridPos)
    {
        selectedGridPosition = gridPos;

        if (towerUIPanel != null)
        {
            towerUIPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("towerUIPanel is not assigned in UIManager.");
        }
    }

    public void OnClickPlaceButton()
    {
        if (TowerPlacement != null)
        {
            TowerPlacement.PlaceTower(selectedGridPosition);
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
}