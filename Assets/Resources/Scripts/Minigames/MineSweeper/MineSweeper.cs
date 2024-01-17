using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MineSweeperGameInfo;

class MineSweeper : MiniGame
{

    public Tilemap _TileMap = null;
    Manager manager;
    MineSweeperGameInfo gameInfo;
    MineSweeperGameInfo.Difficulty difficulty = MineSweeperGameInfo.Difficulty.Easy;
    int width, height, bombCount;
    float cameraDistance;

    void TableInit()
    {
        (width, height,cameraDistance, bombCount) = gameInfo.GetTableInfo(difficulty);
        GameObject go = GameObject.FindGameObjectsWithTag("@TileMap")[0];
        _TileMap = go.GetComponent<Tilemap>();
        
        //Camera Setting
        Vector3 midPoint  = _TileMap.CellToWorld(new Vector3Int(width / 2, height / 2));
        Camera.main.transform.position = midPoint + Vector3.back * cameraDistance;

        //CellTable initializing
        gameInfo.CellTable = new CellType[width, height];
        gameInfo.CellTableInitialize(difficulty);
        gameInfo.DataTable = new int[width, height];
        gameInfo.DataTableInitialize(difficulty);

        //Table Setting
        gameInfo.TileMapInitailze(_TileMap, difficulty);


    }
    
    override public void OnStart()
    {
        manager = Manager.manager;
        if (manager == null) Debug.Log("NULL!");
        InputManager input = manager.input;
        input.OnMouseAction -= OnClick;
        input.OnMouseAction += OnClick;

        gameInfo = new MineSweeperGameInfo();

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

    Vector3Int previousTarget = Vector3Int.zero;
    override public void OnClick(Define.MouseEvent _event)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (_event == Define.MouseEvent.Lrelease)
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red, 1f);
            Physics.Raycast(ray.origin, ray.direction, out hit, 100f);
            Vector3Int hitTarget = _TileMap.WorldToCell(hit.point);
            if (hitTarget.x >= 0 && hitTarget.x < width && hitTarget.y >= 0 && hitTarget.y < height)
            {
                gameInfo.CellOpen(_TileMap, difficulty, hitTarget);
            }
        }
        else if (_event == Define.MouseEvent.Lhold)
        {
            Physics.Raycast(ray.origin, ray.direction, out hit, 100f);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red);
            Vector3Int hitTarget = _TileMap.WorldToCell(hit.point);
            if(hitTarget.x >=0 && hitTarget.x < width &&hitTarget.y >=0 && hitTarget.y < height)
            {

                if (gameInfo.CellTable[hitTarget.x, hitTarget.y] == CellType.CellClose)
                {
                    _TileMap.SetTile(hitTarget, gameInfo.GetTileFromSprites(CellType.CellOpen));
                    gameInfo.CellTable[hitTarget.x, hitTarget.y] = CellType.CellOpen;

                    previousTarget = hitTarget;
                }
            }
            if (hitTarget != previousTarget)
            {
                if (gameInfo.CellTable[previousTarget.x, previousTarget.y] == CellType.CellOpen)
                {
                    _TileMap.SetTile(previousTarget, gameInfo.GetTileFromSprites(CellType.CellClose));
                    gameInfo.CellTable[previousTarget.x, previousTarget.y] = CellType.CellClose;

                }
                if (hitTarget.x >= 0 && hitTarget.x < width && hitTarget.y >= 0 && hitTarget.y < height)
                {
                    previousTarget = hitTarget;
                }
                else
                {
                    previousTarget = Vector3Int.zero;
                }
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
