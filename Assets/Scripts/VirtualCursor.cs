using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCursor : MonoBehaviour
{
    public float sensivity = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Mouse X");
        float moveY = Input.GetAxis("Mouse Y");


        // マウス移動で計算した目的地
        // 差分を計算（他のコードが transform.position をワープで変更しても
        // この差分を足し合わせることでワープを上書きしないようにする）
        Vector3 originalPos = transform.position;
        Vector3 intendedPos = originalPos + new Vector3(moveX / 2 * sensivity, moveY / 2 * sensivity, 0);

        // カメラからはみ出さないようにビューポート座標で丸め込む（ワールド座標の絶対値ではなく差分を算出）
        Camera cam = Camera.main;
        Vector3 deltaToApply;
        if (cam != null)
        {
            // original と intended をそれぞれ Viewport にして差分を取得
            Vector3 vpOriginal = cam.WorldToViewportPoint(originalPos);
            Vector3 vpIntended = cam.WorldToViewportPoint(intendedPos);

            // intended の Viewport を clamp する
            vpIntended.x = Mathf.Clamp01(vpIntended.x);
            vpIntended.y = Mathf.Clamp01(vpIntended.y);

            // clamp 後のワールド座標を得て、original に対する差分を計算
            Vector3 worldClamped = cam.ViewportToWorldPoint(vpIntended);
            deltaToApply = worldClamped - originalPos;
        }
        else
        {
            // カメラがない場合は単純にワールド差分をそのまま適用
            deltaToApply = intendedPos - originalPos;
        }

        // 差分を足し込む（外部でワープが入っていても上書きしない）
        transform.position += deltaToApply;
    }
}
