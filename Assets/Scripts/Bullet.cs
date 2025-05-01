using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 5f;
    
    void Start()
    {
        // 数秒後に自動で消える
        Destroy(gameObject, lifetime);
    }
    
    // 何かに当たったら
    void OnTriggerEnter2D(Collider2D other)
    {
        // Enemyタグを持つオブジェクトに当たった場合
        if (other.CompareTag("Enemy"))
        {
            // ここに敵へのダメージ処理などを追加できます
            Debug.Log("敵に命中！");
            
            // 弾を破壊
            Destroy(gameObject);
        }
    }
}
