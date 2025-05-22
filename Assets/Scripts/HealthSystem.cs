using UnityEngine;

// 体力システムの共通インターフェース
// タワーと敵の両方に使用される基本的なヘルスシステム
public interface IHealth
{
    // 現在のヘルスポイント
    float CurrentHealth { get; }
    
    // 最大ヘルスポイント
    float MaxHealth { get; }
    
    // ダメージを受ける処理
    void TakeDamage(float damageAmount);
    
    // 死亡時の処理
    void Die();
    
    // ヘルス状態の確認
    bool IsAlive();
}