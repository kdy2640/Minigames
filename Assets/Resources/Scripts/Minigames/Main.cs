using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Main : MiniGame
{

    public override void InGame()
    {
    }

    int Count = 0;
    public override void OnAwake()
    {
        if (Count < 5) ++Count;
        else status = GameStatus.Start;
    }

    public override void OnClick(Define.MouseEvent _event)
    {
    }

    public override void OnEnd()
    {

    }

    public override void OnFail()
    {

    }

    public override void OnStart()
    {
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
        Manager.manager.ChangeMiniGame(Define.MiniGameType.GameSelect);
    }
    void QuitButtonClicked()
    {
        Application.Quit();
    }
    public override void OnWin()
    {

    }
}

