using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 変数で指定した要素（例: 4つ）をランダムに、かつ一度出たものは再出力しないように抽選するコンポーネント。
// 抽選は public メソッド Draw() を呼ぶことで行う（UI Button の OnClick に割り当てる）。
// リセットは public メソッド ResetPool() を呼ぶことで行う（別ボタンに割り当てる）。
public class RandomText : MonoBehaviour
{
    [Header("Elements to draw (unique)")]
    public List<string> elements = new List<string> { "A", "B", "C", "D" };

    [Header("Output Text (assign one)")]
    public TextMeshProUGUI tmpOutput;
    public Text uiOutput;

    [Header("Optional Buttons (assign in Inspector)")]
    public Button drawButton;
    public Button resetButton;

    // 内部で使用する残りプール
    private List<string> remaining = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        // ボタンが割り当てられていればイベント登録（Inspector にも設定可）
        if (drawButton != null)
            drawButton.onClick.AddListener(Draw);
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetPool);

        ResetPool();
    }

    // 抽選：残りプールからランダムに一つ取り出して表示、二度と出ないように除外
    public void Draw()
    {
        if (remaining == null || remaining.Count == 0)
        {
            // 何も残っていない場合は表示クリアまたは通知
            SetOutputText("(なし)");
            // 必要なら drawButton を無効化
            if (drawButton != null) drawButton.interactable = false;
            // プールが空のときだけリセットを許可
            if (resetButton != null) resetButton.interactable = true;
            return;
        }

        int idx = Random.Range(0, remaining.Count);
        string picked = remaining[idx];
        // 取り出し
        remaining.RemoveAt(idx);

        SetOutputText(picked);

        // 残りが無くなったら drawButton を無効化し、リセットを有効化
        if (remaining.Count == 0)
        {
            if (drawButton != null) drawButton.interactable = false;
            if (resetButton != null) resetButton.interactable = true;
        }
    }

    // プールをリセットして最初から抽選可能にする
    public void ResetPool()
    {
        remaining = new List<string>(elements);
        // 出力をリセット表示
        SetOutputText("(リセット)");
        // プール再生成後は抽選可能（要素がある場合のみ）。リセットは無効化しておく。
        if (drawButton != null) drawButton.interactable = (remaining.Count > 0);
        if (resetButton != null) resetButton.interactable = (remaining.Count == 0);
    }

    private void SetOutputText(string s)
    {
        if (tmpOutput != null)
            tmpOutput.text = s;
        if (uiOutput != null)
            uiOutput.text = s;
    }
}
