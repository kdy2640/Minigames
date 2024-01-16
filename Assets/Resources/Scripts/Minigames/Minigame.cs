using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

abstract class MiniGame
{
    public enum GameStatus
    {
        Start, Ingame,Rest, End
    }
    public GameStatus status = GameStatus.Start;


    abstract public void OnStart();

    abstract public void InProgress();

    abstract public void OnEnd();

    public static MiniGame GetMinigame(Define.MiniGameStatus _name)
    {
        MiniGame mg = null;
        switch (_name) 
        {
            case Define.MiniGameStatus.MineSweeper:
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
            case GameStatus.Start: OnStart(); break;
            case GameStatus.Ingame: InProgress(); break;
            case GameStatus.Rest: break;
            case GameStatus.End: OnEnd(); break;
        }
    }


    abstract public void OnClick(Define.MouseEvent _event);
}