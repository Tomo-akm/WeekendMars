using UnityEngine;

public class BulletTargetSeek : MonoBehaviour
{
    // 目標位置
    public Vector2 targetPosition;
    
    // 弾のパラメータ
    public float speed = 5f;      // 弾の速度
    public float damage = 10f;    // 攻撃力
    
    // 弾の生存時間
    public float lifetime = 5f;
    
    void Start()
    {
        // 数秒後に自動で消える
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        // 現在位置から目標位置への方向ベクトルを計算
        Vector2 currentPosition = transform.position;
        
        // 弾を目標に向かって移動
        transform.position = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);
        
        // 目標に非常に近づいたら消滅（当たったとみなす）
        float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);
        if (distanceToTarget < 0.1f)
        {
            // 目標到達のログ
            Debug.Log("弾が目標位置に到達しました");
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("弾が " + other.name + " に触れました");
        
        // Enemyタグを持つオブジェクトに当たった場合
        if (other.CompareTag("Enemy"))
        {
            //Debug.Log("敵に命中！ ダメージ: " + damage);
            
            // 将来的にダメージを与える処理を追加
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
}
