
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManagement : MonoBehaviour
{
    public LayerMask StageLayer;

    // 動作状態を定義する 
    private enum MOVE_TYPE
    {
        STOP,
        RIGHT,
        LEFT,
    }
    MOVE_TYPE move = MOVE_TYPE.LEFT; // 初期状態は左に移動 
    Rigidbody2D rbody2D;             // Rigidbody2Dを定義
    float speed;                     // 移動速度を格納する変数

    private void Start()
    {
        // Rigidbody2Dのコンポーネントを取得
        rbody2D = GetComponent<Rigidbody2D>();
    }

    // 物理演算(rigidbody)はFixedUpdateで処理する
    private void FixedUpdate()
    {
        // Playerの方向を決めるためにスケールの取り出し
        Vector3 scale = transform.localScale;
        if (move == MOVE_TYPE.STOP)
        {
            speed = 0;
        }
        else if (move == MOVE_TYPE.RIGHT)
        {
            scale.x = 1; // 右向き
            speed = 3;
        }
        else if (move == MOVE_TYPE.LEFT)
        {
            scale.x = -1; // 左向き
            speed = -3;
        }
        transform.localScale = scale; // scaleを代入
        // rigidbody2Dのvelocity(速度)へ取得したspeedを入れる。y方向は動かないのでそのままにする
        rbody2D.linearVelocity = new Vector2(speed, rbody2D.linearVelocity.y);
    }
}
