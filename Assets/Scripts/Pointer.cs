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
            Debug.Log("Key pressed for Pointer: " + key);
            MoveCursorToPointer();
        }
    }

    void MoveCursorToPointer()
    {
        Debug.Log("MoveCursorToPointer: " + key);
        GameObject cursor = GameObject.FindGameObjectWithTag("Cursor");
        cursor.transform.position = new Vector3(transform.position.x, transform.position.y, 0.3f);
    }
}
