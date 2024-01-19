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

            //������ ������ �ٲ�°�
            if (hitTarget.x >= 0 && hitTarget.x < width && hitTarget.y >= 0 && hitTarget.y < height)
            {
                if (gameInfo.CellTable[hitTarget.x, hitTarget.y] == CellType.CellClose)
                {
                    _TileMap.SetTile(hitTarget, gameInfo.GetTileFromSprites(CellType.CellOpen));
                    gameInfo.CellTable[hitTarget.x, hitTarget.y] = CellType.CellOpen;

                }
            }
            // �����Ŷ� ���� ������
            if (hitTarget != previousTarget)
            {
                // �����Ű� �����ְ� ������� �ʾҴٸ�
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

            //������ ������ �ٲ�°�
            if (hitTarget.x >= 0 && hitTarget.x < width && hitTarget.y >= 0 && hitTarget.y < height)
            {
                if (gameInfo.CellTable[hitTarget.x, hitTarget.y] == CellType.CellClose)
                {
                    _TileMap.SetTile(hitTarget, gameInfo.GetTileFromSprites(CellType.CellOpen));
                    gameInfo.CellTable[hitTarget.x, hitTarget.y] = CellType.CellOpen;

                }
            }
            // �����Ŷ� ���� ������
            if (hitTarget != previousTarget)
            {
                // �����Ű� �����ְ� ������� �ʾҴٸ�
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

            // ������ ���� ������� ���� ���̰ų�, ����� ���ڼ��ӿ��� �ֺ��� ���ڸ�ŭ�� ����� ���ٸ� 
            // ���ΰ� �ֺ��� �������� ����
            int x = 0; int y = 0;
            // ���� Ȯ��
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
                        // �ε��� ����
                        if (x < 0 || x >= width) continue;
                        if (y < 0 || y >= width) continue;

                        //������� �ʾҴµ� ������ ���̸� �ٲ���
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
                    //��� ���� Ȯ��
                    for (int i = 0; i < 8; i++)
                    {
                        x = hitTarget.x + checkArrayX[i]; y = hitTarget.y + checkArrayY[i];
                        // �ε��� ����
                        if (x < 0 || x >= width) continue;
                        if (y < 0 || y >= width) continue;

                        // ��� ���� Ȯ��
                        if (gameInfo.CellTable[x, y] == CellType.Flag) ++flagCount;
                    }

                    // ����� ���ڿ� �°� �� �Ⱦ����� �ƹ� �͵� �� ��
                    if(flagCount != gameInfo.DataTable[hitTarget.x,hitTarget.y].data)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            x = hitTarget.x + checkArrayX[i]; y = hitTarget.y + checkArrayY[i];
                            // �ε��� ����
                            if (x < 0 || x >= width) continue;
                            if (y < 0 || y >= width) continue;

                            //������� �ʾҴµ� ������ ���̸� �ٲ���
                            if (gameInfo.CellTable[x, y] == CellType.CellOpen && gameInfo.DataTable[x, y].isOpen == false)
                            {
                                gameInfo.CellTable[x, y] = CellType.CellClose;
                                _TileMap.SetTile(new Vector3Int(x, y, 0), gameInfo.GetTileFromSprites(CellType.CellClose));
                            }
                        }
                    }
                    else // ����� ���ڿ� �°� �ȾҴٸ� ����
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            x = hitTarget.x + checkArrayX[i]; y = hitTarget.y + checkArrayY[i];
                            // �ε��� ����
                            if (x < 0 || x >= width) continue;
                            if (y < 0 || y >= width) continue;

                            // ����� �����ϰ� ����
                            if (gameInfo.CellTable[x, y] != CellType.Flag)
                            {
                                gameInfo.CellOpen(_TileMap, difficulty, new Vector3Int(x, y, 0));
                            }
                        }
                        
                    }
                }
            }
            // �ֺ� �� ����
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


            // �������� ���� ������ ��ȣ�ۿ�
            if (hitTarget != previousTarget)
            {
                // �������� ��������� ������ ������ ���̸� �ٽ� ����
                if (gameInfo.CellTable[previousTarget.x, previousTarget.y] == CellType.CellOpen && gameInfo.DataTable[previousTarget.x, previousTarget.y].isOpen == false)
                {
                    _TileMap.SetTile(previousTarget, gameInfo.GetTileFromSprites(CellType.CellClose));
                    gameInfo.CellTable[previousTarget.x, previousTarget.y] = CellType.CellClose;
                }

                //�ֺ� �� ����
                int x = 0; int y = 0;
                for (int i = 0; i < 8; i++)
                {
                    x = previousTarget.x + checkArrayX[i]; y = previousTarget.y + checkArrayY[i];

                    // �ε��� ����
                    if (x < 0 || x >= width) continue;
                    if (y < 0 || y >= width) continue;

                    //������� �ʾҴµ� ���� ���̸� ���� �ɷ� �ٲ���
                    if (gameInfo.CellTable[x, y] == CellType.CellOpen && gameInfo.DataTable[x,y].isOpen == false)
                    {
                        gameInfo.CellTable[x, y] = CellType.CellClose;
                        _TileMap.SetTile(new Vector3Int(x, y, 0), gameInfo.GetTileFromSprites(CellType.CellClose));
                    }
                }

                // ���缿�� �������� ����
                if (hitTarget.x >= 0 && hitTarget.x < width && hitTarget.y >= 0 && hitTarget.y < height)
                {
                    previousTarget = hitTarget;
                }
                else // �ٱ����� ������
                {
                    previousTarget = Vector3Int.zero;
                }
            }
            //������ ������ �ٲ�°�
            if (hitTarget.x >= 0 && hitTarget.x < width && hitTarget.y >= 0 && hitTarget.y < height)
            {
                // ���� ���� ���ڿ��� ����
                if (gameInfo.CellTable[hitTarget.x, hitTarget.y] == CellType.CellClose || ((int)gameInfo.CellTable[hitTarget.x, hitTarget.y] > 24 && (int)gameInfo.CellTable[hitTarget.x, hitTarget.y] < 33) )
                {
                    CellType hitCell = gameInfo.CellTable[hitTarget.x, hitTarget.y];
                    //Ŭ���� ���� �������̰� ������� �ʾҴٸ� Ŭ���� �� ����
                    if(hitCell == CellType.CellClose && gameInfo.DataTable[hitTarget.x,hitTarget.y].isOpen == false)
                    {
                        _TileMap.SetTile(hitTarget, gameInfo.GetTileFromSprites(CellType.CellOpen));
                        gameInfo.CellTable[hitTarget.x, hitTarget.y] = CellType.CellOpen;
                    }

                    //�ֺ� �� ����
                    int x = 0; int y = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        x = hitTarget.x + checkArrayX[i]; y = hitTarget.y + checkArrayY[i];
                        
                        // �ε��� ����
                        if (x < 0 || x >= width) continue;
                        if (y < 0 || y >= width) continue;

                        //���� ���̸� ���� �ɷ� �ٲ���
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
