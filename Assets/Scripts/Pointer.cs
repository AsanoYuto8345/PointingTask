using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    public int key;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1 + (key - 1)))
        {
            MoveCursorToPointer();
        }
    }

    void MoveCursorToPointer()
    {
        GameObject cursor = GameObject.FindGameObjectWithTag("Cursor");
        cursor.transform.position = transform.position;
    }
}
