using UnityEngine;

public class DirectMoveBullet : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 5f;
    public float damage = 10f;
    public float lifetime = 5f;
    
    void Start()
    {
        // 数秒後に自動で消える
        Destroy(gameObject, lifetime);
        
        // 方向に応じて弾の角度を設定
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    void Update()
    {
        // 一定方向に直線移動
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Enemyタグを持つオブジェクトに当たった場合
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("敵に命中！ ダメージ: " + damage);
            
            // 敵を破壊
            Destroy(other.gameObject);
            
            // 弾を破壊
            Destroy(gameObject);
        }
    }
}
