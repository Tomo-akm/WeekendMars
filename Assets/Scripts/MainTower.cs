using UnityEngine;
using UnityEngine.Events;
using TMPro;

// メインタワーの防衛システム - 統合管理クラス
public class MainTower : MonoBehaviour
{
    public static MainTower instance;
    
    [Header("タワー設定")]
    [SerializeField] private int maxHealth ;
    [SerializeField] private int currentHealth;

    [Header("UI要素")]
    [SerializeField] private TextMeshProUGUI healthText;    // HP表示用テキスト
    
    // タワーのヘルスが変化したときのイベント
    public UnityEvent<int, int> onTowerHealthChanged; // (current, max)
    
    // タワーが攻撃されたときのイベント
    public UnityEvent<int> onTowerAttacked; // (damage)
    
    // タワーが破壊されたときのイベント
    public UnityEvent onTowerDestroyed;
    
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDestroyed => currentHealth <= 0;
    
    private void Awake()
    {
        // シングルトンパターン
        if (instance == null)
        {
            instance = this;
            currentHealth = maxHealth;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        onTowerHealthChanged?.Invoke(currentHealth, maxHealth);
        // 初期値を設定
        UpdateHealthDisplay(currentHealth, maxHealth);
    }
    
    // タワーがダメージを受ける処理
    public void TakeDamage(int damageAmount)
    {
        if (IsDestroyed)
            return;
            
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);
        UpdateHealthDisplay(currentHealth,maxHealth);
        
        Debug.Log($"メインタワーがダメージを受けました！ダメージ: {damageAmount}, 残りHP: {currentHealth}/{maxHealth}");
        
        // イベント発火
        onTowerAttacked?.Invoke(damageAmount);
        onTowerHealthChanged?.Invoke(currentHealth, maxHealth);
        
        
        // タワーのHPが0以下になったら破壊処理とゲームオーバー
        if (currentHealth <= 0)
        {
            FindFirstObjectByType<GameManager>().GameOver();
            DestroyTower();
        }
    }
    
    // タワー破壊処理
    private void DestroyTower()
    {
        Debug.Log("メインタワーが破壊されました！");
        Destroy(gameObject);
        
    }
    
    // タワーの最大体力を設定
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        onTowerHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    

    // 敵の拠点侵入検知（2D）
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleEnemyInvasion(other.gameObject);
    }
    
    // 敵侵入処理の共通メソッド
    private void HandleEnemyInvasion(GameObject enemyObject)
    {
        EnemyHealth enemy = enemyObject.GetComponent<EnemyHealth>();
        if (enemy != null && enemy.IsAlive())
        {
            // 敵にタワー到達を通知
            enemy.ReachBase();
        }
    }
    public bool IsAlive()
    {

        return currentHealth > 0; // currentHealthフィールドがある場合

    }
    
    // HP表示を更新
    private void UpdateHealthDisplay(int currentHealth, int maxHealth)
    {
        if (healthText != null)
        {
            healthText.text =  "HP" + currentHealth.ToString() + "/" + maxHealth.ToString();
        }
    }

}