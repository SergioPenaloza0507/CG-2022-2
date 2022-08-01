using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobaInputHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SendMessage("Listen", new Vector2Action{contextId = MOBA.ActionContextHolder.SCREEN_POSITION_CONTEXT_ID, value = Input.mousePosition}, SendMessageOptions.DontRequireReceiver);
        }
    }
}
