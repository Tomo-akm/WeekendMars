using UnityEngine;

public class TowerShoot : MonoBehaviour
{
    // 弾のプレハブ
    public GameObject bulletPrefab;
    
    // タワーのパラメータ
    public float attackDamage = 10f;    // 攻撃力
    public float attackSpeed = 1f;      // 攻撃速度（1秒間に何回攻撃するか）
    
    // 射撃間隔（秒）- attackSpeedから自動計算
    public float fireRate = 1f;
    
    // 弾の速度
    public float bulletSpeed = 5f;
    
    // 敵のタグ名
    public string enemyTag = "Enemy";
    
    // 射撃位置
    public Transform firePoint;
    
    // 次に射撃可能になる時間
    private float nextFireTime = 0f;
    
    // 検出範囲 - これを射程距離として使用
    public float detectionRange = 5f;
    
    // デバッグ用に線を描画するかどうか
    public bool drawDebugLines = true;
    
    void Start()
    {
        // attackSpeedからfireRateを計算
        fireRate = 1f / attackSpeed;
        
        // firePointが設定されていない場合は自分自身を使用
        if (firePoint == null)
        {
            Debug.Log("FirePointが設定されていません。タワー自身から発射します。");
            firePoint = transform;
        }
    }
    
    void Update()
    {
        // 最も近い敵を探す
        GameObject nearestEnemy = FindNearestEnemy();
        
        // 敵が範囲内にいて、射撃間隔を過ぎていれば発射
        if (nearestEnemy != null && Time.time >= nextFireTime)
        {
            // 次の射撃時間を設定
            nextFireTime = Time.time + fireRate;
            
            // 敵の方向を正確に計算
            Vector2 targetPosition = nearestEnemy.transform.position;
            Vector2 firePosition = firePoint.position;
            Vector2 direction = (targetPosition - firePosition).normalized;
            
            // デバッグ用に敵への線を描画
            if (drawDebugLines)
            {
                Debug.DrawLine(firePosition, targetPosition, Color.red, 1.0f);
                Debug.Log("弾の発射方向: " + direction);
            }
            
            // 弾を発射
            FireBullet(direction, targetPosition);
        }
    }
    
    // 最も近い敵を探す関数
    GameObject FindNearestEnemy()
    {
        // 全ての"Enemy"タグを持つオブジェクトを取得
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        
        if (enemies.Length == 0)
        {
            return null;
        }
        
        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity; // 無限大から始める
        
        foreach (GameObject enemy in enemies)
        {
            // 敵との距離を計算
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            
            // まず最も近い敵を見つける
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }
        
        // 最も近い敵が見つかったら、それが攻撃範囲内かどうか確認
        if (nearestEnemy != null && nearestDistance <= detectionRange)
        {
            return nearestEnemy;
        }
        else
        {
            return null; // 攻撃範囲内に敵がいない
        }
    }
    
    // 弾を発射する関数（ターゲット指定版）
    void FireBullet(Vector2 direction, Vector2 targetPosition)
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // 弾のプレハブを生成
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            
            // TargetSeekコンポーネントを追加（これにより弾が確実に敵を追跡）
            BulletTargetSeek targetSeek = bullet.AddComponent<BulletTargetSeek>();
            targetSeek.targetPosition = targetPosition;
            targetSeek.speed = bulletSpeed;
            
            // 攻撃力を設定
            targetSeek.damage = attackDamage;
            
            // デバッグ用にトレイルレンダラーを追加
            TrailRenderer trail = bullet.AddComponent<TrailRenderer>();
            trail.startWidth = 0.1f;
            trail.endWidth = 0.05f;
            trail.time = 0.5f;
            trail.startColor = Color.yellow;
            trail.endColor = new Color(1, 0.5f, 0, 0.5f);
            
            Debug.Log("弾を発射しました。目標位置: " + targetPosition + ", 攻撃力: " + attackDamage);
        }
    }
    
    // インスペクターでattackSpeedが変更された時の処理
    void OnValidate()
    {
        // attackSpeedが0以下にならないように
        if (attackSpeed <= 0)
        {
            attackSpeed = 0.1f;
        }
        
        // attackSpeedからfireRateを再計算
        fireRate = 1f / attackSpeed;
    }
}
