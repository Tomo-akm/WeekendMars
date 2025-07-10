using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class UpgradeLevel
{
    public string levelName = "Level 1";
    public float attackRate = 1f;
    public float detectionRange = 10f;
    public float shotSpeed = 12f;
    public float damage = 10f;
    public int upgradeCost = 50;
    public Sprite towerSprite;
    public Color towerColor = Color.white;
    public float yOffset = 0f; // 手動Y補正（上に上げたい場合は正の値）
}

public class TowerUpgrade : MonoBehaviour
{
    [Header("Upgrade Settings")]
    public UpgradeLevel[] upgradeLevels;
    private int currentLevel = 0;
    
    [Header("References")]
    private Tower towerComponent;
    private SpriteRenderer spriteRenderer;
    private UIManager uiManager;

    private float baseY; // 設置時の基準Y座標
    private float baseBottomY; // 設置時のスプライト下端ワールドY座標
    private float baseCenterToBottom; // 設置時のスプライト中心から下端までの距離
    private bool baseYInitialized = false;
    
    private void Start()
    {
        towerComponent = GetComponent<Tower>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found in scene!");
        }
        // 設置時のスプライト中心から下端までの距離を記録
        if (upgradeLevels.Length > 0 && upgradeLevels[0].towerSprite != null)
        {
            Sprite s = upgradeLevels[0].towerSprite;
            float spriteHeight = s.rect.height / s.pixelsPerUnit;
            float pivotY = s.pivot.y / s.rect.height;
            baseCenterToBottom = spriteHeight * pivotY;
        }
        else
        {
            baseCenterToBottom = 0f;
        }
        // 初期レベルの設定を適用
        ApplyUpgrade(0);
    }
    
    // タワーがクリックされた時
    private void OnMouseDown()
    {
        // UIの上をクリックしていない場合のみ
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (uiManager != null)
            {
                // UIManagerにこのタワーを選択したことを通知
                uiManager.ShowTowerUpgradeUI(this);
                
                // 選択エフェクト
                SelectEffect();
            }
        }
    }
    
    // 選択時のエフェクト
    private void SelectEffect()
    {
        spriteRenderer.color = Color.white;
    }
    
    // 選択解除時のエフェクト
    public void DeselectEffect()
    {
        spriteRenderer.color = Color.white;
    }
    
    // アップグレード試行（UIManagerから呼ばれる）
    public void AttemptUpgrade()
    {
        if (currentLevel >= upgradeLevels.Length - 1)
        {
            Debug.Log("Already at max level!");
            return;
        }
            
        UpgradeLevel nextLevel = upgradeLevels[currentLevel + 1];

        // お金を消費してアップグレード
        if (GameManager.instance.SpendMoney(nextLevel.upgradeCost))
        {
            currentLevel++;
            ApplyUpgrade(currentLevel);

            Debug.Log($"Tower upgraded to level {currentLevel + 1}!");

            // アップグレードエフェクトを再生
            PlayUpgradeEffect();
            // 基本BGMの再生
            SEPlayer.instance.PlaytowerUpgradeSE();
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }
    
    private void ApplyUpgrade(int level)
    {
        if (level < 0 || level >= upgradeLevels.Length)
            return;
        UpgradeLevel upgrade = upgradeLevels[level];
        
        // タワーのパラメータを更新
        if (towerComponent != null)
        {
            towerComponent.attackRate = upgrade.attackRate;
            towerComponent.detectionRange = upgrade.detectionRange;
            towerComponent.shotSpeed = upgrade.shotSpeed;
            towerComponent.damage = upgrade.damage;
        }
        
        // 見た目を変更
        if (upgrade.towerSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = upgrade.towerSprite;
            // Inspectorで設定したyOffsetのみでY座標を補正
            if (Mathf.Abs(upgrade.yOffset) > 0.0001f) {
                transform.position = new Vector3(transform.position.x, transform.position.y + upgrade.yOffset, transform.position.z);
            }
        }
        // 色を常に白にリセット（赤みや色の異常を防ぐ）
        spriteRenderer.color = Color.white;
    }
    
    private void PlayUpgradeEffect()
    {
        // パーティクルエフェクトがあれば再生
        ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
        if (particles != null)
        {
            particles.Play();
        }
        
        // スケールアニメーション
        StartCoroutine(ScaleAnimation());
    }
    
    private IEnumerator ScaleAnimation()
    {
        Vector3 originalScale = transform.localScale;
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.3f;
            transform.localScale = originalScale * scale;
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
    
    // 現在のダメージを取得
    public float GetCurrentDamage()
    {
        if (currentLevel < upgradeLevels.Length)
            return upgradeLevels[currentLevel].damage;
        return 10f;
    }
    
    // 現在のレベルを取得
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    
    // UIManagerがタワーの選択を解除した時
    private void Update()
    {
        // UIManagerのアップグレードパネルが閉じられた時、選択エフェクトを解除
        if (uiManager != null && uiManager.towerUpgradePanel != null && 
            !uiManager.towerUpgradePanel.activeSelf)
        {
            DeselectEffect();
        }
    }
}
