using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Main : MiniGame
{
    Manager manager;

    public override void InGame()
    {
    }

    int Count = 0;
    public override void OnAwake()
    {
        if (Count < 5) Count++;
        else
        {
            manager = Manager.manager;
            if (!manager.scene.isClose)
            {
                status = GameStatus.Start; return;
            }

            if (manager.scene.OpenWindow())
            {
                status = GameStatus.Start;
            }
        }
    }

    public override void OnClick(Define.MouseEvent _event)
    {
    }

    public override void OnEnd()
    {
        if(manager.scene.CloseWindow())
        {
            Manager.manager.ChangeMiniGame(Define.MiniGameType.GameSelect);
        }
    }

    public override void OnFail()
    {

    }

    public override void OnStart()
    {
        manager = Manager.manager;

        manager.scene.initializer();
        ButtonInitializer();
        status = GameStatus.Rest;
    }
    void ButtonInitializer()
    {
        GameObject go = GameObject.Find("StartButton");
        Button startButton = go.GetComponent<Button>();
        startButton.onClick.AddListener(delegate { StartButtonClicked(); });


        go = GameObject.Find("QuitButton");
        Button exitButton = go.GetComponent<Button>();
        exitButton.onClick.AddListener(delegate { QuitButtonClicked(); });
    }
    
    void StartButtonClicked()
    {
        status = GameStatus.End;
    }
    void QuitButtonClicked()
    {
        Application.Quit();
    }
    public override void OnWin()
    {

    }
}

