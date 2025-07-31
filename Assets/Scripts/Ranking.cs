using UnityEngine;
using System.Collections.Generic;
using System.IO; // ファイルの読み書きに必要
using System.Linq; // LINQを使ってソートするために必要

// スコア一つのデータを保持するためのクラス
public class ScoreEntry
{
    public string username;
    public int score;
}

public class Ranking : MonoBehaviour
{
    public static Ranking instance;
    private string csvPath;

    void Awake()
    {
        // シングルトンパターンの実装
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄しない
        }
        else
        {
            Destroy(gameObject);
            return; // 既にインスタンスがあれば何もしない
        }
        
        csvPath = Path.Combine(Application.streamingAssetsPath, "ranking.csv");
    }

    // ランキングを読み込む関数
    public List<ScoreEntry> LoadScores()
    {
        List<ScoreEntry> scores = new List<ScoreEntry>();

        // CSVファイルが存在するか確認
        if (!File.Exists(csvPath))
        {
            Debug.LogError("ranking.csvが見つかりません！");
            return scores;
        }

        // CSVファイルを1行ずつ読み込む
        string[] lines = File.ReadAllLines(csvPath);
        foreach (string line in lines)
        {
            // 空行は無視
            if (string.IsNullOrEmpty(line)) continue;

            // カンマで分割して、ユーザー名とスコアを取得
            string[] values = line.Split(',');
            if (values.Length == 2)
            {
                ScoreEntry entry = new ScoreEntry();
                entry.username = values[0];
                int.TryParse(values[1], out entry.score); // 文字列を数値に変換
                scores.Add(entry);
            }
        }
        return scores;
    }

    // ランキングを保存する関数
    public void SaveScores(List<ScoreEntry> scores)
    {
        // 新しいCSVファイルの内容を作成
        string[] lines = scores.Select(entry => $"{entry.username},{entry.score}").ToArray();

        // ファイルに書き込む
        File.WriteAllLines(csvPath, lines);
    }

    // 新しいスコアを追加する関数
    public void AddScore(string newUsername, int newScore)
    {
        // 1. 現在のランキングをロードする
        List<ScoreEntry> scores = LoadScores();

        // 2. 新しいスコアを追加する
        scores.Add(new ScoreEntry { username = newUsername, score = newScore });

        // 3. スコアの高い順に並び替える (ソート)
        List<ScoreEntry> sortedScores = scores.OrderByDescending(entry => entry.score).ToList();

        // 4. (任意) 上位10件など、保存する件数を制限する
        List<ScoreEntry> topScores = sortedScores.Take(5).ToList();

        // 5. 新しいランキングをファイルに保存する
        SaveScores(topScores);

        Debug.Log("新しいスコアを追加し、ランキングを更新しました。");
    }
}