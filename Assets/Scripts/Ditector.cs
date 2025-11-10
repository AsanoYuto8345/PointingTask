using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;

public class Ditector : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject countCanvas;
    public GameObject experimentCanvas;
    public GameObject resultCanvas;
    public GameObject pointer;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI countDownText;
    public TextMeshProUGUI resultText;
    public GameObject target;
    public Toggle toggle;
    Vector3 prePosition;
    int count;
    int accidentalClick;
    float initTime, startTime, diff_time;
    bool isExperimentMode, isPracticeMode, isTenKeyMode;


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
        if (!isPracticeMode)
        {
            using (StreamWriter stream_writer = new StreamWriter("./PointingLog.txt", true))
            {
                stream_writer.WriteLine(txt);
                stream_writer.Close();
            }
        }
    }

    // 実験モード遷移
    public void changeExperimentMode()
    {
        isTenKeyMode = toggle.isOn;
        isPracticeMode = false;
        menuCanvas.SetActive(false);
        StartCoroutine(startExperiment(100));
    }

    public void changePracticeMode()
    {
        isTenKeyMode = toggle.isOn;
        isPracticeMode = true;
        menuCanvas.SetActive(false);
        StartCoroutine(startExperiment(10));
    }

    public void changeResultMode()
    {
        destroyPointer();
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

    public void generatePointer()
    {
        Camera cam = Camera.main;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                // 画面座標上での分割点（中央寄せ）
                float screenX = (i + 0.5f) * Screen.width / 3f;
                float screenY = (j + 0.5f) * Screen.height / 3f;

                // スクリーン座標 → ワールド座標に変換
                Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenX, screenY, cam.nearClipPlane + 10f));

                // ポインター生成
                GameObject p = Instantiate(pointer, worldPos, Quaternion.identity);
                Pointer pointerComponent = p.GetComponent<Pointer>();
                pointerComponent.key = i + j * 3 + 1;
                p.tag = "Pointer";
            }
        }
    }

    public void destroyPointer()
    {
        GameObject[] pointers = GameObject.FindGameObjectsWithTag("Pointer");
        foreach (GameObject p in pointers)
        {
            Destroy(p);
        }
    }

    IEnumerator startExperiment(int click)
    {
        generatePointer();
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
        count = click;
        accidentalClick = 0;
        countText.text = "残: " + count.ToString();
        startTime = Time.time;
        isExperimentMode = true;
        experimentCanvas.SetActive(true);
        writePointingData("計測開始: " + DateTime.Now.ToString());
    }
}
