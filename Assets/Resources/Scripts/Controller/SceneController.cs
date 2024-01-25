using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneController
{
    GameObject Window;
    GameObject Canvas;
    public bool isClose = false;
    public SceneController()
    {
    }


    GameObject UpperWindow;
    GameObject LowerWindow;
    public void initializer()
    {
        Window = Resources.Load<GameObject>("Prefabs/SceneController/Window");
        Canvas = GameObject.Find("Canvas");
        if (Canvas == null) Debug.Log("There is no canvas!");



        UpperWindow = GameObject.Instantiate(Window, Canvas.transform);
        UpperWindow.transform.Translate(Vector3.up * 360);
        LowerWindow = GameObject.Instantiate(Window, Canvas.transform);
        LowerWindow.transform.Translate(Vector3.down * 720);
        LowerWindow.transform.rotation = Quaternion.Euler(Vector3.forward * 180);

        GameObject.DontDestroyOnLoad(Canvas);
        Canvas.GetComponent<Canvas>().sortingOrder = 1;

    }

    const float CLOSESPEED = 0.3f;
    const float OPENSPEED = 0.5f;
    public bool CloseWindow()
    {
        UpperWindow.transform.position = Vector3.Lerp(UpperWindow.transform.position, Canvas.transform.position + 180 * Vector3.up, CLOSESPEED);
        LowerWindow.transform.position = Vector3.Lerp(LowerWindow.transform.position, Canvas.transform.position + 180 * Vector3.down, CLOSESPEED);

        if(UpperWindow.transform.position.y - LowerWindow.transform.position.y < 360.5f)
        {
            UpperWindow.transform.position = Canvas.transform.position + 180 * Vector3.up;
            LowerWindow.transform.position = Canvas.transform.position + 180 * Vector3.down;
            isClose = true;
            destroyAllUi();
            return true;
        }
        return false;
    }

    public bool OpenWindow()
    {
        UpperWindow.transform.position = Vector3.Lerp(UpperWindow.transform.position, Canvas.transform.position + 555 * Vector3.up, OPENSPEED);
        LowerWindow.transform.position = Vector3.Lerp(LowerWindow.transform.position, Canvas.transform.position + 555 * Vector3.down, OPENSPEED);


        if (UpperWindow.transform.position.y - LowerWindow.transform.position.y > 1080)
        {
            UpperWindow.transform.position = Canvas.transform.position + 1080 * Vector3.up;
            LowerWindow.transform.position = Canvas.transform.position + 1080 * Vector3.down;

            isClose = false;
            GameObject.Destroy(Canvas);
            return true;
        }
        return false;

    }

    public void destroyAllUi()
    {
        int Count = Canvas.transform.childCount;
        for (int i = 0; i < Count; i++)
        {
            if (Canvas.transform.GetChild(i).tag != "@SceneChanger")
            {
                Canvas.transform.GetChild(i).gameObject.SetActive(false);
            }
        }


    }
}
