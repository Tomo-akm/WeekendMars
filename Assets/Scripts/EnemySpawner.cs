using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //敵プレハブ
    public GameObject enemyPrefab;
    //時間間隔の最小値
    public float minTime = 3f;
    //時間間隔の最大値
    public float maxTime = 5f;
    //敵生成時間間隔
    private float interval;
    //経過時間
    private float time = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //時間間隔を決定する
        interval = GetRandomTime();
    }

    // Update is called once per frame
    void Update()
    {
        //時間計測
        time += Time.deltaTime;

        //経過時間が生成時間になったとき(生成時間より大きくなったとき)
        if(time > interval)
        {
            //enemyをインスタンス化する(生成する)
            GameObject enemy = Instantiate(enemyPrefab);
            //生成した敵の座標を決定する
            enemy.transform.position = new Vector3(9.56f,-6.13f,0f);
            //経過時間を初期化して再度時間計測を始める
            time = 0f;
            //次に発生する時間間隔を決定する
            interval = GetRandomTime();
        }
    }

    //ランダムな時間を生成する関数
    private float GetRandomTime()
    {
        return Random.Range(minTime, maxTime);
    }
}