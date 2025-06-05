using UnityEngine;
using UnityEngine.Events;
using System.Collections;

// 敵の体力と攻撃システム
// 敵のヘルスを管理し、ダメージ処理と死亡処理を行う
public class EnemyHealth : MonoBehaviour, IHealth
{
    [Header("体力設定")]
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float currentHealth;
    [SerializeField] private int moneyValue = 10; 

    [Header("攻撃設定")]
    [SerializeField] private int baseDamage = 1; // 拠点に与えるダメージ
    [SerializeField] private float attackInterval = 2.5f; // 攻撃間隔（秒）
 
    // 敵が倒されたときのイベント
    public UnityEvent onEnemyDestroyed;
    
    // ヘルスが変化したときのイベント（UI更新用）
    public UnityEvent<float> onHealthChanged;
    
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

   // 拠点攻撃中かどうかのフラグ
    private bool isAttackingBase = false;
    private Coroutine attackCoroutine = null;

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

        // 着弾音
        SEPlayer.instance.PlaydamageSE();

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
        // 攻撃中の場合は攻撃を停止
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        // 敵破壊イベントを発火
        onEnemyDestroyed?.Invoke();

        // 敵を無効化または破壊
        Destroy(gameObject);
        Debug.Log(gameObject + "が死亡しました");

        // お金を追加
        GameManager.instance.AddMoney(moneyValue, transform.position);

        // 死亡時にSEを鳴らす。
        SEPlayer.instance.PlayEnemyDieSE();
    }
    
    // 生存確認
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
    
    // 拠点に到達したときの処理
    public void ReachBase()
    {   
        if (isAttackingBase || !IsAlive())
        return;

        // 拠点攻撃開始
        isAttackingBase = true;
        attackCoroutine = StartCoroutine(AttackBaseCoroutine());
        
        Debug.Log($"{gameObject.name}が拠点に到達！攻撃開始");
        }
    

     // 拠点を継続的に攻撃するコルーチン
    private IEnumerator AttackBaseCoroutine()
    {
        while (IsAlive() && MainTower.instance != null && MainTower.instance.IsAlive())
        {
            // 拠点にダメージを与える
            MainTower.instance.TakeDamage(baseDamage);
            Debug.Log($"{gameObject.name}が拠点を攻撃！ダメージ: {baseDamage}");
            
            // 攻撃間隔を待つ
            yield return new WaitForSeconds(attackInterval);
        }
    
    }

    // 拠点攻撃中かどうかを確認
    public bool IsAttackingBase()
    {
        return isAttackingBase;
    }
}