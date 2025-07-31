using UnityEngine;
using TMPro; // TextMesh Proを使う場合
using System.Collections.Generic;

public class RankingDisplay : MonoBehaviour
{
    // Inspectorで設定するランキング表示用テキスト
    public TextMeshProUGUI rankingDisplayText;

    void Start()
    {
        // ランキングデータを読み込んで表示する
        DisplayRanking();
    }

    private void DisplayRanking()
    {
        if (Ranking.instance == null)
        {
            Debug.LogError("Rankingが見つかりません！");
            rankingDisplayText.text = "ランキングの読み込みに失敗しました。";
            return;
        }

        // Rankingrからスコアリストを取得
        List<ScoreEntry> scores = Ranking.instance.LoadScores();

        // 表示を初期化
        rankingDisplayText.text = "";

        // 上位から順にテキストを生成
        int rank = 1;
        foreach (ScoreEntry entry in scores)
        {
            // 例: "1. GUEST - 5000\n" のような形式で追加
            rankingDisplayText.text += $"{rank}. {entry.username} - {entry.score}\n";
            rank++;
        }
    }
}