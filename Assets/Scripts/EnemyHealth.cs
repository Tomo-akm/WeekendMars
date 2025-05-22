using UnityEngine;
using UnityEngine.Events;

// 敵の体力と攻撃システム
// 敵のヘルスを管理し、ダメージ処理と死亡処理を行う
public class EnemyHealth : MonoBehaviour, IHealth
{
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float currentHealth;
    [SerializeField] private int moneyValue = 10; 
    
    // 敵が倒されたときのイベント
    public UnityEvent onEnemyDestroyed;
    
    // ヘルスが変化したときのイベント（UI更新用）
    public UnityEvent<float> onHealthChanged;
    
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        // 初期化時に最大HPを設定
        currentHealth = maxHealth;
    }

    // ダメージを受ける処理
    public void TakeDamage(float damageAmount)
    {
        if (!IsAlive())
            return;
            
        currentHealth -= damageAmount;
        
        // 0未満にならないように調整
        currentHealth = Mathf.Max(0, currentHealth);
        
        // ヘルス変化イベントを発火
        onHealthChanged?.Invoke(currentHealth / maxHealth);
        
        // 体力がゼロになったら死亡処理
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // 死亡処理
    public void Die()
    {
        // 敵破壊イベントを発火
        onEnemyDestroyed?.Invoke();
        
        // 敵を無効化または破壊
        Destroy(gameObject);
        Debug.Log(gameObject +"が死亡しました");

        // お金を追加
        GameManager.instance.AddMoney(moneyValue, transform.position);
    }
    
    // 生存確認
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
    
}