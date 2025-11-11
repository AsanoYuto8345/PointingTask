using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    public int key;
    // Update is called once per frame
    void Update()
    {
        // key は 1..9 を想定。これを Q W E / A S D / Z X C に対応させる。
        // マップ配列を用いて該当 KeyCode が押されたかを判定する。
        KeyCode[] keyMap = new KeyCode[] {
            KeyCode.None, // index 0 unused
            KeyCode.Q, // 1 -> Q (top-left)
            KeyCode.W, // 2 -> W (top-center)
            KeyCode.E, // 3 -> E (top-right)
            KeyCode.A, // 4 -> A (middle-left)
            KeyCode.S, // 5 -> S (middle-center)
            KeyCode.D, // 6 -> D (middle-right)
            KeyCode.Z, // 7 -> Z (bottom-left)
            KeyCode.X, // 8 -> X (bottom-center)
            KeyCode.C  // 9 -> C (bottom-right)
        };

        if (key >= 1 && key <= 9)
        {
            KeyCode mapped = keyMap[key];
            if (mapped != KeyCode.None && Input.GetKeyDown(mapped))
            {
                Debug.Log("Key pressed for Pointer: " + key + " mapped to " + mapped);
                MoveCursorToPointer();
            }
        }
    }

    void MoveCursorToPointer()
    {
        Debug.Log("MoveCursorToPointer: " + key);
        GameObject cursor = GameObject.FindGameObjectWithTag("Cursor");
        if (cursor != null)
        {
            cursor.transform.position = new Vector3(transform.position.x, transform.position.y, 0.3f);
        }
        else
        {
            Debug.LogWarning("Cursor object with tag 'Cursor' not found.");
        }
    }
}
