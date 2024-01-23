using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

abstract class MiniGame
{
    public enum GameStatus
    {
        Awake, Start, InGame, Rest, Win, Fail, End
    }
    public GameStatus status = GameStatus.Start;


    abstract public void OnAwake();
    abstract public void OnStart();

    abstract public void InGame();

    abstract public void OnWin();
    abstract public void OnFail();
    abstract public void OnEnd();

    public static MiniGame GetMinigame(Define.MiniGameType _name)
    {
        MiniGame mg = null;
        switch (_name)
        {
            case Define.MiniGameType.GameSelect:
                mg = new GameSelect();
                break;
            case Define.MiniGameType.MineSweeper:
                mg = new MineSweeper();
                break;
            default:
                mg = null;
                break;

         }
        return mg;
    }

    public void OnUpdate()
    {
        switch(status)
        {
            case GameStatus.Awake: OnAwake(); break;
            case GameStatus.Start: OnStart(); break;
            case GameStatus.InGame: InGame(); break;
            case GameStatus.Rest: break;
            case GameStatus.Win: OnWin(); break;
            case GameStatus.Fail: OnFail(); break;
            case GameStatus.End: OnEnd(); break;
        }
    }


    abstract public void OnClick(Define.MouseEvent _event);
}
