using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    public Action OnKeyAction = null;
    public Action<Define.MouseEvent> OnMouseAction = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void OnUpdate()
    {
        if (Input.anyKey && OnKeyAction != null)
        {
            OnKeyAction.Invoke();
        }

        if(OnMouseAction != null)
        {
            if (Input.GetMouseButton(0)) OnMouseAction.Invoke(Define.MouseEvent.Lclick);
            if (Input.GetMouseButtonDown(0)) OnMouseAction.Invoke(Define.MouseEvent.Lhold);
            if (Input.GetMouseButtonUp(0)) OnMouseAction.Invoke(Define.MouseEvent.Lrelease);

            if (Input.GetMouseButton(1)) OnMouseAction.Invoke(Define.MouseEvent.Rclick);
            if (Input.GetMouseButtonDown(1)) OnMouseAction.Invoke(Define.MouseEvent.Rhold);
            if (Input.GetMouseButtonUp(1)) OnMouseAction.Invoke(Define.MouseEvent.Rrelease);

            if (Input.GetMouseButton(2)) OnMouseAction.Invoke(Define.MouseEvent.Mclick);
            if (Input.GetMouseButtonDown(2)) OnMouseAction.Invoke(Define.MouseEvent.Mhold);
            if (Input.GetMouseButtonUp(2)) OnMouseAction.Invoke(Define.MouseEvent.Mrelease);
        }
    }
}
