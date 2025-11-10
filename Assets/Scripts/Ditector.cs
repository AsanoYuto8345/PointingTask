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
    public GameObject target;
    public GameObject virtualCursor;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI countDownText;
    public TextMeshProUGUI resultText;
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
        Cursor.lockState = CursorLockMode.Confined;
    }
    // Update is called once per frame

    void Update()
    {
        if (isExperimentMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // virtualCursor の Collider2D があればその bounds を使って重なりを調べる。
                // なければ virtualCursor.transform.position を基準に OverlapPoint を使う。
                Collider2D collision2D = null;

                if (virtualCursor != null)
                {
                    Collider2D vCol = virtualCursor.GetComponent<Collider2D>();
                    if (vCol != null)
                    {
                        // Collider の bounds を使って OverlapBoxAll で重なりを検出
                        Bounds b = vCol.bounds;
                        Collider2D[] overlaps = Physics2D.OverlapBoxAll(b.center, b.size, 0f);
                        if (overlaps != null && overlaps.Length > 0)
                        {
                            // 最初に見つかったものをクリック対象とする（必要ならソートしてz順/優先度を変える）
                            collision2D = overlaps[0];
                        }
                    }
                    else
                    {
                        // Collider が無ければ transform.position を基準に判定
                        collision2D = Physics2D.OverlapPoint(virtualCursor.transform.position);
                    }
                }
                else
                {
                    // virtualCursor が割り当てられていない場合は従来のマウス位置ベースの判定にフォールバック
                    if (Camera.main != null)
                    {
                        Vector2 clickPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        collision2D = Physics2D.OverlapPoint(clickPoint);
                    }
                }

                if (collision2D)
                {
                    GameObject clickedObject = collision2D.transform.gameObject;

                    // クリック対象が target の場合に target をランダム移動させる
                    if (clickedObject == target)
                    {
                        Debug.Log("Target clicked: " + clickedObject.name);

                        // 現在の target の位置を取得してからランダム移動
                        Vector3 nowPosition = target.transform.position;
                        target.transform.position = new Vector2(UnityEngine.Random.Range(-7.0f, 7.0f), UnityEngine.Random.Range(-4.0f, 4.0f));

                        // カウント処理
                        count--;
                        countText.text = "残: " + count.ToString();

                        // 距離、時間計算（prePosition は target の前回位置を保持している想定）
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
                        // virtualCursor が何か他のオブジェクトに当たっている場合は誤クリック扱い
                        accidentalClick++;
                    }
                }
                else
                {
                    // 衝突なしは誤クリック
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
        Cursor.visible = true;
        virtualCursor.SetActive(false);
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
        Cursor.visible = false;
        virtualCursor.SetActive(true);
        if(isTenKeyMode)generatePointer();
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
