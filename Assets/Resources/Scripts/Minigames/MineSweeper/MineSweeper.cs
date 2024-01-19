using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEditor.Experimental.GraphView;
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
        gameInfo.DataTable = new module[width, height];
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


        status = GameStatus.InGame;
    }
    override public void InGame()
    {
        status = GameStatus.Rest;
    }
    override public void OnEnd()
    {

    }

    Vector3Int previousTarget = Vector3Int.zero;
    override public void OnClick(Define.MouseEvent _event)
    {
        if (status != GameStatus.Rest) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int[] checkArrayX = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] checkArrayY = { 1, 1, 1, 0, 0, -1, -1, -1 };

        if (_event == Define.MouseEvent.Lrelease)
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red, 1f);
            Physics.Raycast(ray.origin, ray.direction, out hit, 100f);
            Vector3Int hitTarget = _TileMap.WorldToCell(hit.point);
            if (hitTarget.x >= 0 && hitTarget.x < width && hitTarget.y >= 0 && hitTarget.y < height)
            {
                if (gameInfo.DataTable[hitTarget.x, hitTarget.y].isOpen == false && gameInfo.CellTable[hitTarget.x, hitTarget.y] != CellType.Flag)
                {
                    gameInfo.CellOpen(_TileMap, difficulty, hitTarget);
                }
            }
        }
        else if (_event == Define.MouseEvent.Lhold)
        {
            Physics.Raycast(ray.origin, ray.direction, out hit, 100f);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red);
            Vector3Int hitTarget = _TileMap.WorldToCell(hit.point);

            //누르고 있으면 바뀌는거
            if (hitTarget.x >= 0 && hitTarget.x < width && hitTarget.y >= 0 && hitTarget.y < height)
            {
                if (gameInfo.CellTable[hitTarget.x, hitTarget.y] == CellType.CellClose)
                {
                    _TileMap.SetTile(hitTarget, gameInfo.GetTileFromSprites(CellType.CellOpen));
                    gameInfo.CellTable[hitTarget.x, hitTarget.y] = CellType.CellOpen;

                }
            }
            // 이전거랑 같지 않으면
            if (hitTarget != previousTarget)
            {
                // 이전거가 눌려있고 개방되지 않았다면
                if (gameInfo.CellTable[previousTarget.x, previousTarget.y] == CellType.CellOpen && gameInfo.DataTable[previousTarget.x, previousTarget.y].isOpen == false)
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
        else if (_event == Define.MouseEvent.Rrelease)
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red, 1f);
            Physics.Raycast(ray.origin, ray.direction, out hit, 100f);
            Vector3Int hitTarget = _TileMap.WorldToCell(hit.point);
            if (hitTarget.x >= 0 && hitTarget.x < width && hitTarget.y >= 0 && hitTarget.y < height)
            {
                if (gameInfo.CellTable[hitTarget.x, hitTarget.y] == CellType.Flag)
                {
                    gameInfo.CellTable[hitTarget.x, hitTarget.y] = CellType.CellClose;
                    _TileMap.SetTile(hitTarget, gameInfo.GetTileFromSprites(CellType.CellClose));

                }
                else if (gameInfo.CellTable[hitTarget.x, hitTarget.y] == CellType.CellOpen && gameInfo.DataTable[hitTarget.x, hitTarget.y].isOpen == false)
                {
                    gameInfo.CellTable[hitTarget.x, hitTarget.y] = CellType.Flag;
                    _TileMap.SetTile(hitTarget, gameInfo.GetTileFromSprites(CellType.Flag));

                }
            }
        }
        else if (_event == Define.MouseEvent.Rhold)
        {
            Physics.Raycast(ray.origin, ray.direction, out hit, 100f);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red);
            Vector3Int hitTarget = _TileMap.WorldToCell(hit.point);

            //누르고 있으면 바뀌는거
            if (hitTarget.x >= 0 && hitTarget.x < width && hitTarget.y >= 0 && hitTarget.y < height)
            {
                if (gameInfo.CellTable[hitTarget.x, hitTarget.y] == CellType.CellClose)
                {
                    _TileMap.SetTile(hitTarget, gameInfo.GetTileFromSprites(CellType.CellOpen));
                    gameInfo.CellTable[hitTarget.x, hitTarget.y] = CellType.CellOpen;

                }
            }
            // 이전거랑 같지 않으면
            if (hitTarget != previousTarget)
            {
                // 이전거가 눌려있고 개방되지 않았다면
                if (gameInfo.CellTable[previousTarget.x, previousTarget.y] == CellType.CellOpen && gameInfo.DataTable[previousTarget.x, previousTarget.y].isOpen == false)
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
        else if (_event == Define.MouseEvent.Mrelease)
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red, 1f);
            Physics.Raycast(ray.origin, ray.direction, out hit, 100f);
            Vector3Int hitTarget = _TileMap.WorldToCell(hit.point);

            // 선택한 셀이 개방되지 않은 셀이거나, 개방된 숫자셀임에도 주변에 숫자만큼의 깃발이 없다면 
            // 본인과 주변의 셀변형을 없앰
            int x = 0; int y = 0;
            // 범위 확인
            if (hitTarget.x >= 0 && hitTarget.x < width && hitTarget.y >= 0 && hitTarget.y < height)
            {
                CellType hitCell = gameInfo.CellTable[hitTarget.x, hitTarget.y];
                if (hitCell == CellType.CellOpen && gameInfo.DataTable[hitTarget.x, hitTarget.y].isOpen == false)
                {

                    _TileMap.SetTile(hitTarget, gameInfo.GetTileFromSprites(CellType.CellClose));
                    gameInfo.CellTable[hitTarget.x, hitTarget.y] = CellType.CellClose;

                    for (int i = 0; i < 8; i++)
                    {
                        x = hitTarget.x + checkArrayX[i]; y = hitTarget.y + checkArrayY[i];
                        // 인덱스 조건
                        if (x < 0 || x >= width) continue;
                        if (y < 0 || y >= width) continue;

                        //개방되지 않았는데 변형된 셀이면 바꿔줌
                        if (gameInfo.CellTable[x, y] == CellType.CellOpen && gameInfo.DataTable[x, y].isOpen == false)
                        {
                            gameInfo.CellTable[x, y] = CellType.CellClose;
                            _TileMap.SetTile(new Vector3Int(x, y, 0), gameInfo.GetTileFromSprites(CellType.CellClose));
                        }
                    }
                }
                else if ((int)hitCell > 24 && (int)hitCell < 33)
                {
                    int flagCount = 0;
                    bool misMatch = false;
                    //깃발 개수 확인
                    for (int i = 0; i < 8; i++)
                    {
                        x = hitTarget.x + checkArrayX[i]; y = hitTarget.y + checkArrayY[i];
                        // 인덱스 조건
                        if (x < 0 || x >= width) continue;
                        if (y < 0 || y >= width) continue;

                        // 깃발 개수 확인
                        if (gameInfo.CellTable[x, y] == CellType.Flag) ++flagCount;
                    }

                    // 깃발을 숫자에 맞게 안 꽂았으면 아무 것도 안 함
                    if(flagCount != gameInfo.DataTable[hitTarget.x,hitTarget.y].data)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            x = hitTarget.x + checkArrayX[i]; y = hitTarget.y + checkArrayY[i];
                            // 인덱스 조건
                            if (x < 0 || x >= width) continue;
                            if (y < 0 || y >= width) continue;

                            //개방되지 않았는데 변형된 셀이면 바꿔줌
                            if (gameInfo.CellTable[x, y] == CellType.CellOpen && gameInfo.DataTable[x, y].isOpen == false)
                            {
                                gameInfo.CellTable[x, y] = CellType.CellClose;
                                _TileMap.SetTile(new Vector3Int(x, y, 0), gameInfo.GetTileFromSprites(CellType.CellClose));
                            }
                        }
                    }
                    else // 깃발을 숫자에 맞게 꽂았다면 개방
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            x = hitTarget.x + checkArrayX[i]; y = hitTarget.y + checkArrayY[i];
                            // 인덱스 조건
                            if (x < 0 || x >= width) continue;
                            if (y < 0 || y >= width) continue;

                            // 깃발을 제외하고 개방
                            if (gameInfo.CellTable[x, y] != CellType.Flag)
                            {
                                gameInfo.CellOpen(_TileMap, difficulty, new Vector3Int(x, y, 0));
                            }
                        }
                        
                    }
                }
            }
            // 주변 셀 적용
            if (hitTarget.x >= 0 && hitTarget.x < width && hitTarget.y >= 0 && hitTarget.y < height)
            {
                for (int i = 0; i < 8; i++)
                {
                    x = hitTarget.x + checkArrayX[i]; y = hitTarget.y + checkArrayY[i];

                }
            }
        }
        else if (_event == Define.MouseEvent.Mhold)
        {
            Physics.Raycast(ray.origin, ray.direction, out hit, 100f);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red);
            Vector3Int hitTarget = _TileMap.WorldToCell(hit.point);


            // 이전셀과 같지 않으면 상호작용
            if (hitTarget != previousTarget)
            {
                // 이전셀이 개방되있지 않은데 변형된 셀이면 다시 닫음
                if (gameInfo.CellTable[previousTarget.x, previousTarget.y] == CellType.CellOpen && gameInfo.DataTable[previousTarget.x, previousTarget.y].isOpen == false)
                {
                    _TileMap.SetTile(previousTarget, gameInfo.GetTileFromSprites(CellType.CellClose));
                    gameInfo.CellTable[previousTarget.x, previousTarget.y] = CellType.CellClose;
                }

                //주변 셀 적용
                int x = 0; int y = 0;
                for (int i = 0; i < 8; i++)
                {
                    x = previousTarget.x + checkArrayX[i]; y = previousTarget.y + checkArrayY[i];

                    // 인덱스 조건
                    if (x < 0 || x >= width) continue;
                    if (y < 0 || y >= width) continue;

                    //개방되지 않았는데 열린 셀이면 닫힌 걸로 바꿔줌
                    if (gameInfo.CellTable[x, y] == CellType.CellOpen && gameInfo.DataTable[x,y].isOpen == false)
                    {
                        gameInfo.CellTable[x, y] = CellType.CellClose;
                        _TileMap.SetTile(new Vector3Int(x, y, 0), gameInfo.GetTileFromSprites(CellType.CellClose));
                    }
                }

                // 현재셀을 이전셀로 변경
                if (hitTarget.x >= 0 && hitTarget.x < width && hitTarget.y >= 0 && hitTarget.y < height)
                {
                    previousTarget = hitTarget;
                }
                else // 바깥으로 나가면
                {
                    previousTarget = Vector3Int.zero;
                }
            }
            //누르고 있으면 바뀌는거
            if (hitTarget.x >= 0 && hitTarget.x < width && hitTarget.y >= 0 && hitTarget.y < height)
            {
                // 닫힌 셀과 숫자에만 적용
                if (gameInfo.CellTable[hitTarget.x, hitTarget.y] == CellType.CellClose || ((int)gameInfo.CellTable[hitTarget.x, hitTarget.y] > 24 && (int)gameInfo.CellTable[hitTarget.x, hitTarget.y] < 33) )
                {
                    CellType hitCell = gameInfo.CellTable[hitTarget.x, hitTarget.y];
                    //클릭한 셀이 닫힌셀이고 개방되지 않았다면 클릭한 셀 변경
                    if(hitCell == CellType.CellClose && gameInfo.DataTable[hitTarget.x,hitTarget.y].isOpen == false)
                    {
                        _TileMap.SetTile(hitTarget, gameInfo.GetTileFromSprites(CellType.CellOpen));
                        gameInfo.CellTable[hitTarget.x, hitTarget.y] = CellType.CellOpen;
                    }

                    //주변 셀 적용
                    int x = 0; int y = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        x = hitTarget.x + checkArrayX[i]; y = hitTarget.y + checkArrayY[i];
                        
                        // 인덱스 조건
                        if (x < 0 || x >= width) continue;
                        if (y < 0 || y >= width) continue;

                        //닫힌 셀이면 여는 걸로 바꿔줌
                        if (gameInfo.CellTable[x,y] == CellType.CellClose)
                        {
                            gameInfo.CellTable[x, y] = CellType.CellOpen;
                            _TileMap.SetTile(new Vector3Int(x,y,0), gameInfo.GetTileFromSprites(CellType.CellOpen));
                        }
                    }
                    

                }
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
