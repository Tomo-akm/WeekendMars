using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class UpgradeLevel
{
    public string levelName = "レベル1";
    public float attackRate = 1f;
    public float detectionRange = 10f;
    public float shotSpeed = 12f;
    public float damage = 10f;
    public int upgradeCost = 50;
    public Sprite towerSprite; // レベルごとの見た目（オプション）
    public Color towerColor = Color.white; // レベルごとの色（オプション）
}

public class TowerUpgrade : MonoBehaviour
{
    [Header("アップグレード設定")]
    public UpgradeLevel[] upgradeLevels; // 各レベルの設定
    private int currentLevel = 0;
    
    [Header("UI要素")]
    public GameObject upgradeUI; // アップグレードUIパネル
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI statsText;
    public Button upgradeButton;
    
    private Tower towerComponent;
    private SpriteRenderer spriteRenderer;
    private bool isSelected = false;
    
    private void Start()
    {
        towerComponent = GetComponent<Tower>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 初期レベルの設定を適用
        ApplyUpgrade(0);
        
        // UIを非表示に
        if (upgradeUI != null)
            upgradeUI.SetActive(false);
            
        // アップグレードボタンのイベント設定
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(AttemptUpgrade);
    }
    
    private void Update()
    {
        // マウスクリックでタワーを選択
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                SelectTower();
            }
            else if (isSelected)
            {
                DeselectTower();
            }
        }
    }
    
    private void SelectTower()
    {
        isSelected = true;
        
        // UIを表示
        if (upgradeUI != null)
        {
            upgradeUI.SetActive(true);
            UpdateUI();
        }
        
        // 選択時のエフェクト（現在の色を少し明るくする）
        if (spriteRenderer != null && currentLevel < upgradeLevels.Length)
        {
            Color baseColor = upgradeLevels[currentLevel].towerColor;
            // 色を明るくする（RGB値を1.2倍にして、最大1.0でクランプ）
            spriteRenderer.color = new Color(
                Mathf.Min(baseColor.r * 1.2f, 1f),
                Mathf.Min(baseColor.g * 1.2f, 1f),
                Mathf.Min(baseColor.b * 1.2f, 1f),
                baseColor.a
            );
        }
    }
    
    private void DeselectTower()
    {
        isSelected = false;
        
        // UIを非表示
        if (upgradeUI != null)
            upgradeUI.SetActive(false);
        
        // 色を現在のレベルの色に戻す
        if (spriteRenderer != null && currentLevel < upgradeLevels.Length)
            spriteRenderer.color = upgradeLevels[currentLevel].towerColor;
    }
    
    private void UpdateUI()
    {
        if (currentLevel < upgradeLevels.Length - 1)
        {
            // 次のレベルがある場合
            UpgradeLevel nextLevel = upgradeLevels[currentLevel + 1];
            
            levelText.text = $"Current: {upgradeLevels[currentLevel].levelName}";
            costText.text = $"Upgrade Cost: {nextLevel.upgradeCost}G";
            
            statsText.text = $"Attack Speed: {upgradeLevels[currentLevel].attackRate} → {nextLevel.attackRate}\n" +
                           $"Range: {upgradeLevels[currentLevel].detectionRange} → {nextLevel.detectionRange}\n" +
                           $"Bullet Speed: {upgradeLevels[currentLevel].shotSpeed} → {nextLevel.shotSpeed}\n" +
                           $"Damage: {upgradeLevels[currentLevel].damage} → {nextLevel.damage}";
            
            // お金が足りるかチェックしてボタンの有効/無効を切り替え
            int currentMoney = GameManager.instance.GetCurrentMoney();
            upgradeButton.interactable = currentMoney >= nextLevel.upgradeCost;
        }
        else
        {
            // 最大レベルの場合
            levelText.text = $"Current: {upgradeLevels[currentLevel].levelName} (Max Level)";
            costText.text = "Max Level Reached";
            statsText.text = $"Attack Speed: {upgradeLevels[currentLevel].attackRate}\n" +
                           $"Range: {upgradeLevels[currentLevel].detectionRange}\n" +
                           $"Bullet Speed: {upgradeLevels[currentLevel].shotSpeed}\n" +
                           $"Damage: {upgradeLevels[currentLevel].damage}";
            upgradeButton.interactable = false;
        }
    }
    
    private void AttemptUpgrade()
    {
        if (currentLevel >= upgradeLevels.Length - 1)
            return; // すでに最大レベル
            
        UpgradeLevel nextLevel = upgradeLevels[currentLevel + 1];
        
        // お金を消費してアップグレード
        if (GameManager.instance.SpendMoney(nextLevel.upgradeCost))
        {
            currentLevel++;
            ApplyUpgrade(currentLevel);
            UpdateUI();
            
            Debug.Log($"タワーをレベル{currentLevel + 1}にアップグレードしました！");
            
            // アップグレードエフェクトを再生（オプション）
            PlayUpgradeEffect();
        }
        else
        {
            Debug.Log("お金が足りません！");
        }
    }
    
    private void ApplyUpgrade(int level)
    {
        Debug.Log($"[TowerUpgrade] ApplyUpgrade({level})開始");
        
        if (level < 0 || level >= upgradeLevels.Length)
        {
            Debug.LogError($"[TowerUpgrade] 無効なレベル: {level}");
            return;
        }
            
        UpgradeLevel upgrade = upgradeLevels[level];
        Debug.Log($"[TowerUpgrade] アップグレード適用 - {upgrade.levelName}");
        
        // タワーのパラメータを更新
        if (towerComponent != null)
        {
            towerComponent.attackRate = upgrade.attackRate;
            towerComponent.detectionRange = upgrade.detectionRange;
            towerComponent.shotSpeed = upgrade.shotSpeed;
            towerComponent.damage = upgrade.damage;
            
            Debug.Log($"[TowerUpgrade] パラメータ更新 - " +
                     $"Attack Speed: {upgrade.attackRate}, " +
                     $"Range: {upgrade.detectionRange}, " +
                     $"Bullet Speed: {upgrade.shotSpeed}, " +
                     $"Damage: {upgrade.damage}");
        }
        
        // 見た目を変更（スプライトが設定されている場合）
        if (upgrade.towerSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = upgrade.towerSprite;
            Debug.Log($"[TowerUpgrade] スプライト変更: {upgrade.towerSprite.name}");
        }
        
        // 色を変更（スプライトが設定されていない場合は色で区別）
        if (spriteRenderer != null)
        {
            spriteRenderer.color = upgrade.towerColor;
            Debug.Log($"[TowerUpgrade] 色変更: {upgrade.towerColor}");
        }
    }
    
    private void PlayUpgradeEffect()
    {
        // アップグレード時のエフェクト（パーティクルなど）を再生
        ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
        if (particles != null)
        {
            particles.Play();
        }
        
        // 音を再生
        // AudioSource.PlayClipAtPoint(upgradeSound, transform.position);
        
        // スケールアニメーション（大きくなって戻る）
        StartCoroutine(ScaleAnimation());
    }
    
    private IEnumerator ScaleAnimation()
    {
        Vector3 originalScale = transform.localScale;
        float duration = 0.3f;
        float elapsed = 0f;
        
        // 大きくなる
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
    
    // 現在のダメージを取得（弾丸に渡すため）
    public float GetCurrentDamage()
    {
        if (currentLevel < upgradeLevels.Length)
            return upgradeLevels[currentLevel].damage;
        return 10f; // デフォルト値
    }
    
    // 現在のレベルを取得
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    
    private void OnDestroy()
    {
        // ボタンイベントをクリーンアップ
        if (upgradeButton != null)
            upgradeButton.onClick.RemoveListener(AttemptUpgrade);
    }
}
