using UnityEngine;

public class BulletTargetSeek : MonoBehaviour
{
    // 目標位置
    public Vector2 targetPosition;
    
    // 弾の速度
    public float speed = 5f;
    
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
        Vector2 direction = (targetPosition - currentPosition).normalized;
        
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
            Debug.Log("敵に命中！");
            
            // ここに敵へのダメージ処理などを追加
            
            // 弾を破壊
            Destroy(gameObject);
            //衝突した敵を破壊
            Destroy(other.gameObject);
        }
    }
}
