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
    [SerializeField] private int enemyBonusMultiplier = 100; //敵撃破ボーナスの係数
    [SerializeField] private float enemyBonustime = 10f; //敵撃破ボーナスの時間

    public UnityEvent onEnemyDestroyed;
    public UnityEvent<float> onHealthChanged;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private bool isAttackingBase = false;
    private Coroutine attackCoroutine = null;
    private Spawner spawner;

    // ★ どのウェーブで出現したかを記録する変数を追加
    public int waveOrigin = -1;

    //出現してからの経過時間を計測するタイマー 
    private float timeSinceSpawn = 0f;

    // 死亡フラグ
    private bool isDead = false;

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
        if (!IsAlive() || isDead) return;
        currentHealth -= damageAmount;
        SEPlayer.instance.PlaydamageSE();
        currentHealth = Mathf.Max(0, currentHealth);
        onHealthChanged?.Invoke(currentHealth / maxHealth);
        if (currentHealth <= 0.01f && !isDead)
        {
            isDead = true;
            // コライダーを無効化
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
            // 1フレーム遅延してDestroy
            StartCoroutine(DestroyAfterDelay());
            Die(); // Die()はスコアや通知用
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return null;
        Destroy(gameObject);
    }

    private void Update()
    {
        // 敵が生きている間だけタイマーを進める
        if (IsAlive())
        {
            timeSinceSpawn += Time.deltaTime;
        }
    }

    public void Die()
    {
        if (isDead == false) return; // 多重呼び出し防止
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

        // タイムアタックボーナスを計算してスコアに加算 
        CalculateTimeAttackBonus();

        onEnemyDestroyed?.Invoke();
        GameManager.instance.AddMoney(moneyValue, transform.position);
        GameManager.instance.AddScore(scoreValue);
        SEPlayer.instance.PlayEnemyDieSE();
        
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

    //ボーナス計算用のメソッド 
    private void CalculateTimeAttackBonus()
    {
        // 10秒以内に倒せたらボーナス
        if (timeSinceSpawn <= enemyBonustime)
        {
            // 残り時間が多いほどボーナスが高くなるように計算
            // (10秒 - 経過時間) * 係数
            int bonus = Mathf.FloorToInt((enemyBonustime - timeSinceSpawn) * enemyBonusMultiplier);

            // GameManagerにボーナススコアを加算してもらう
            GameManager.instance.AddScore(bonus);
            
            Debug.Log($"敵撃破ボーナス！ +{bonus}点 (経過時間: {timeSinceSpawn:F2}秒)");
        }
    }
    
}