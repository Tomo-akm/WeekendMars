using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IHealth
{
    [Header("体力設定")]
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float currentHealth;
    [SerializeField] private int moneyValue = 10;

    [Header("攻撃設定")]
    [SerializeField] private int baseDamage = 1;
    [SerializeField] private float attackInterval = 2.5f;

    [Header("スコア")]
    [SerializeField] private int scoreValue = 100;

    public UnityEvent onEnemyDestroyed;
    public UnityEvent<float> onHealthChanged;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private bool isAttackingBase = false;
    private Coroutine attackCoroutine = null;
    private Spawner spawner;

    // ★ どのウェーブで出現したかを記録する変数を追加
    public int waveOrigin = -1;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        spawner = FindFirstObjectByType<Spawner>();
    }

    public void TakeDamage(float damageAmount)
    {
        if (!IsAlive()) return;
        currentHealth -= damageAmount;
        SEPlayer.instance.PlaydamageSE();
        currentHealth = Mathf.Max(0, currentHealth);
        onHealthChanged?.Invoke(currentHealth / maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // ▼▼▼ この部分を変更 ▼▼▼
        // ★ Spawnerに自分の所属ウェーブ番号を渡すように変更
        if (spawner != null)
        {
            spawner.OnEnemyDefeated(this.waveOrigin);
        }
        else
        {
            Debug.LogError("Spawnerが見つかりません！通知できませんでした。");
        }
        // ▲▲▲ ここまで変更 ▲▲▲

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        onEnemyDestroyed?.Invoke();
        GameManager.instance.AddMoney(moneyValue, transform.position);
        GameManager.instance.AddScore(scoreValue);
        SEPlayer.instance.PlayEnemyDieSE();
        
        Destroy(gameObject);
        Debug.Log(gameObject.name + "が死亡しました");
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public void ReachBase()
    {
        if (isAttackingBase || !IsAlive()) return;
        isAttackingBase = true;
        attackCoroutine = StartCoroutine(AttackBaseCoroutine());
        Debug.Log($"{gameObject.name}が拠点に到達！攻撃開始");
    }

    private IEnumerator AttackBaseCoroutine()
    {
        while (IsAlive() && MainTower.instance != null && MainTower.instance.IsAlive())
        {
            MainTower.instance.TakeDamage(baseDamage);
            Debug.Log($"{gameObject.name}が拠点を攻撃！ダメージ: {baseDamage}");
            yield return new WaitForSeconds(attackInterval);
        }
    }

    public bool IsAttackingBase()
    {
        return isAttackingBase;
    }
}