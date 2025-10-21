using System;
using UnityEngine;
using TMPro;
using System.IO;
using System.Collections;

public class Ditector : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject countCanvas;
    public GameObject experimentCanvas;
    public GameObject resultCanvas;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI countDownText;
    public TextMeshProUGUI resultText;
    public GameObject target;
    Vector3 prePosition;
    int count;
    int accidentalClick;
    float initTime, startTime, diff_time;
    bool isExperimentMode;


    // Start is called before the first frame update
    void Start()
    {
        isExperimentMode = false;
    }
    // Update is called once per frame

    void Update()
    {
        if (isExperimentMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 clickPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D collision2D = Physics2D.OverlapPoint(clickPoint);

                if (collision2D)
                {
                    Debug.Log(collision2D.gameObject.name);

                    // クリックされたGameObject clickedObjectを取得
                    GameObject clickedObject = collision2D.transform.gameObject;

                    // ランダム移動
                    Vector3 nowPosition = clickedObject.transform.position;
                    clickedObject.transform.position = new Vector2(UnityEngine.Random.Range(-7.0f, 7.0f), UnityEngine.Random.Range(-4.0f, 4.0f));

                    // カウント処理
                    count--;
                    countText.text = "残: " + count.ToString();

                    // 距離、時間計算
                    float Distance = Vector3.Distance(prePosition, nowPosition);
                    prePosition = nowPosition;
                    diff_time = Time.time - startTime;
                    startTime = Time.time;

                    // ログファイルに書きこみ
                    writePointingData("移動距離: " + Distance + ", 時間: " + diff_time + ", 誤クリック: " + accidentalClick);

                    // 誤クリックカウントリセット
                    accidentalClick = 0;
                }
                else
                {
                    // 誤クリック
                    accidentalClick++;
                }
            }

            if (isExperimentMode && count == 0)
            {
                changeResultMode();
            }
        }

        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     count = 1;
        // }
    }

    // ファイル書き込み
    void writePointingData(string txt)
    {
        using (StreamWriter stream_writer = new StreamWriter("./PointingLog.txt", true))
        {
            stream_writer.WriteLine(txt);
            stream_writer.Close();
        }
    }

    // 実験モード遷移
    public void changeExperimentMode()
    {
        menuCanvas.SetActive(false);
        StartCoroutine(startExperiment());
    }

    public void changeResultMode()
    {
        isExperimentMode = false;
        experimentCanvas.SetActive(false);
        target.SetActive(false);

        string resultTime = (Time.time - initTime).ToString();
        resultText.text = "計測時間: " + resultTime;
        writePointingData("計測時間: " + resultTime + "\n実験終了");

        resultCanvas.SetActive(true);
    }

    // メニューモード遷移
    public void changeMenuMode()
    {
        resultCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }

    IEnumerator startExperiment()
    {
        // カウントダウン表示
        countCanvas.SetActive(true);
        for (int i = 3; i >= 1; i--)
        {
            countDownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        countCanvas.SetActive(false);

        // ターゲット表示
        initTime = Time.time;
        target.SetActive(true);
        count = 100;
        accidentalClick = 0;
        countText.text = "残: " + count.ToString();
        startTime = Time.time;
        isExperimentMode = true;
        experimentCanvas.SetActive(true);
        writePointingData("計測開始: " + DateTime.Now.ToString());
    }
}
