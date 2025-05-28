using UnityEngine;

public class PredictiveShot : MonoBehaviour
{
    [Header("弾の設定")]
    public float speed = 12f;           // 弾の速度
    public float damage = 10f;          // ダメージ量
    public float lifetime = 3f;         // 弾の生存時間
    
    private Vector2 direction;          // 移動方向（固定）
    private bool isInitialized = false; // 初期化フラグ
    
    private void Start()
    {
        // 生存時間後に自動削除
        Destroy(gameObject, lifetime);
        
        // トレイル効果を追加
        SetupTrail();
        
        Debug.Log($"予測射撃弾を生成: 位置={transform.position}");
    }
    
    /// <summary>
    /// 弾を初期化（方向は固定、変更しない）
    /// </summary>
    public void Initialize(Vector2 fireDirection, float shotSpeed)
    {
        direction = fireDirection.normalized;
        speed = shotSpeed;
        isInitialized = true;
        
        Debug.Log($"弾を初期化: 方向={direction}, 速度={speed}");
    }
    
    private void Update()
    {
        if (!isInitialized) return;
        
        // 固定方向に移動（方向は変更しない）
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        
        // 定期的に位置をログ出力
        if (Random.value < 0.02f)
        {
            Debug.Log($"予測弾の位置: {transform.position}, 方向: {direction}");
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"予測弾が {other.name} と衝突 - タグ: {other.tag}");
        
        if (other.CompareTag("Enemy"))
        {
            // 敵のヘルスコンポーネントを取得
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // 敵にダメージを与える
                Debug.Log(other.name + "に" + damage + "ダメージ");
                enemyHealth.TakeDamage(damage);
            }
            
            // 弾を破壊
            Destroy(gameObject);
        }
    }
    
    private void SetupTrail()
    {
        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail == null)
        {
            trail = gameObject.AddComponent<TrailRenderer>();
            trail.startWidth = 0.15f;
            trail.endWidth = 0.08f;
            trail.time = 0.4f;
            trail.startColor = Color.cyan;
            trail.endColor = new Color(0f, 1f, 1f, 0.2f);
        }
    }
}
