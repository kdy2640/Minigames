using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

class MineSweeper : MiniGame
{
    enum CellType
    {
        NotOpen = -1,
        Zero, One, Two, Three, Four, Five, Six, Seven, Eight,
        Bomb = 10, BombOpen, BombCross,
        Flag = 13
    }
    static class GameInfo
    {
    public enum Difficulty
    {
        Easy , Normal, Hard
    }
    public static (int,int,int) GetTableInfo(Difficulty _difficulty)
    {
            switch (_difficulty)
            {
                case Difficulty.Easy:
                    return (9, 9,10);
                case Difficulty.Normal:
                    return (16, 16,40);
                case Difficulty.Hard:
                    return (30, 16, 99);
                default:
                    return (0, 0, 0);
            }

    }

    }


    public Tilemap _TileMap = null;
    Manager manager;
    GameInfo.Difficulty difficulty = GameInfo.Difficulty.Easy;
    void TableInit()
    {
        int width; int height; int bombCount;
        (width, height, bombCount) = GameInfo.GetTableInfo(difficulty);
        GameObject go = GameObject.FindGameObjectsWithTag("@TileMap")[0];

        _TileMap = go.GetComponent<Tilemap>();

        Tile temp = Resources.Load<Tile>("Art/Sprites/MineSweeperSprites_1");
        _TileMap.SetTile(Vector3Int.zero, temp);


    }
    
    override public void OnStart()
    {
        manager = Manager.manager;
        if (manager == null) Debug.Log("NULL!");
        InputManager input = manager.input;
        input.OnMouseAction -= OnClick;
        input.OnMouseAction += OnClick;


        TableInit();


        status = GameStatus.Ingame;
    }
    override public void InProgress()
    {
        status = GameStatus.Rest;
    }
    override public void OnEnd()
    {

    }
    override public void OnClick(Define.MouseEvent _event)
    {
        if(_event == Define.MouseEvent.Lrelease)
        {   
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawLine(ray.origin,ray.origin + ray.direction * 100f ,Color.red,1f);
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
