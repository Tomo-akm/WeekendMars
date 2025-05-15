using UnityEngine;

public class BulletTargetSeek : MonoBehaviour
{
    // 目標位置
    public Vector2 targetPosition;
    
    // 目標の敵オブジェクト
    public GameObject targetEnemy;
    
    // 弾のパラメータ
    public float speed = 5f;          // 弾の速度
    public float damage = 10f;        // 攻撃力
    public float rotationSpeed = 200f; // 旋回速度（度/秒）
    
    // 弾の生存時間
    public float lifetime = 5f;
    
    // 弾の現在の方向ベクトル
    private Vector2 currentDirection;
    
    void Start()
    {
        // 数秒後に自動で消える
        Destroy(gameObject, lifetime);
        
        // 初期の方向を計算
        if (targetEnemy != null)
        {
            // 敵がいる場合は敵の方向
            currentDirection = ((Vector2)targetEnemy.transform.position - (Vector2)transform.position).normalized;
        }
        else
        {
            // 敵がいない場合は目標位置の方向
            currentDirection = (targetPosition - (Vector2)transform.position).normalized;
        }
        
        // 初期の回転を設定
        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    void Update()
    {
        Vector2 currentPosition = transform.position;
        Vector2 targetPos;
        
        // 敵が存在するかチェック
        if (targetEnemy != null)
        {
            // 敵の現在位置
            targetPos = targetEnemy.transform.position;
        }
        else
        {
            // 敵がいない場合は設定された目標位置
            targetPos = targetPosition;
        }
        
        // 目標への方向
        Vector2 directionToTarget = (targetPos - currentPosition).normalized;
        
        // 現在の方向から目標方向へ徐々に回転
        currentDirection = Vector2.Lerp(currentDirection, directionToTarget, rotationSpeed * Time.deltaTime / 100f);
        currentDirection.Normalize();
        
        // 弾を現在の方向に移動
        transform.position = (Vector2)transform.position + currentDirection * speed * Time.deltaTime;
        
        // 弾の回転を方向に合わせる
        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // 目標に非常に近づいたら消滅（当たったとみなす）
        float distanceToTarget = Vector2.Distance(currentPosition, targetPos);
        if (distanceToTarget < 0.1f)
        {
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("弾が " + other.name + " に触れました");
        
        // Enemyタグを持つオブジェクトに当たった場合
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("敵に命中！ ダメージ: " + damage);
            
            // 将来的にダメージを与える処理を追加
            // 現在は敵を直接破壊
            Destroy(other.gameObject);
            
            // 弾を破壊
            Destroy(gameObject);
        }
    }
}
