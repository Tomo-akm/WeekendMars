using UnityEngine;

public class PredictiveShot : MonoBehaviour
{
    private Vector2 moveDirection;
    private float moveSpeed;
    private float damage = 10f;
    
    public void Initialize(Vector2 direction, float speed, float damageValue = 10f)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        damage = damageValue;
        
        Debug.Log($"[PredictiveShot] 初期化 - 受け取ったダメージ値: {damageValue}, 設定されたダメージ: {damage}");
    }
    
    private void Update()
    {
        // 弾を移動
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        
        // 画面外に出たら削除（パフォーマンス対策）
        if (Vector2.Distance(transform.position, Vector2.zero) > 50f)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 敵に当たった場合
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"敵に{damage}ダメージを与えました");
            }
            
            // 弾を削除
            Destroy(gameObject);
        }
    }
}
