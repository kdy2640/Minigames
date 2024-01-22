using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class GameSelect : MiniGame
{
    Manager manager;
    InputManager input;

    int rightEnd = 20;

    public override void InGame()
    {
    }

    public override void OnAwake()
    {
    }

    bool isHold = false;
    Vector3 originPosition;
    Vector3 initialCameraPosition;
    public override void OnClick(Define.MouseEvent _event)
    {
        if (status != GameStatus.Rest) return;

        if (_event == Define.MouseEvent.Lrelease)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - originPosition);
            Vector3 move = new Vector3(pos.x * 2, 0, pos.y * 2);

            Camera.main.transform.Translate(move, Space.World);
        }
        if(_event == Define.MouseEvent.Lclick)
        {
            originPosition = Input.mousePosition;
        }
    }

    public override void OnEnd()
    {
    }

    public override void OnFail()
    {
    }

    public override void OnStart()
    {
        manager = Manager.manager;
        input = manager.input;
        input.OnMouseAction -= OnClick;
        input.OnMouseAction += OnClick;


        status = GameStatus.Rest;
    }

    public override void OnWin()
    {
    }
}
