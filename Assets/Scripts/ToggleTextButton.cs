using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 名前衝突を避けるためクラス名は ToggleTextButton にしています。
// このコンポーネントを UI Button (GameObject) にアタッチし、
// Button の OnClick イベントに OnButtonPressed を割り当ててください。
public class ToggleTextButton : MonoBehaviour
{
    [Header("Text Targets (assign one)")]
    [SerializeField] private TextMeshProUGUI tmpText;
    [SerializeField] private Text uiText;

    [Header("Displayed Strings")]
    [SerializeField] private string option1 = "Option 1";
    [SerializeField] private string option2 = "Option 2";

    [Header("State (1 or 2)")]
    [SerializeField] private int state = 1;

    // Start is called before the first frame update
    void Start()
    {
        // state を正規化して表示を更新
        state = (state == 1) ? 1 : 2;
        UpdateDisplayedText();
    }

    // ボタンの OnClick に割り当てるメソッド
    public void OnButtonPressed()
    {
        ToggleState();
    }

    // 状態を切り替える（1 <-> 2）
    public void ToggleState()
    {
        state = (state == 1) ? 2 : 1;
        UpdateDisplayedText();
    }

    // 外部から状態を設定（1 または 2）。範囲外はクリップされる
    public void SetState(int s)
    {
        state = (s == 1) ? 1 : 2;
        UpdateDisplayedText();
    }

    // 現在の状態を取得
    public int GetState()
    {
        return state;
    }

    // 表示を更新
    private void UpdateDisplayedText()
    {
        string textToShow = (state == 1) ? option1 : option2;

        if (tmpText != null)
        {
            tmpText.text = textToShow;
        }

        if (uiText != null)
        {
            uiText.text = textToShow;
        }
    }
}
