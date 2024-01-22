using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Tilemaps;
using static MineSweeperGameInfo;
using TMPro;
using UnityEngine.UIElements;

class MineSweeper : MiniGame
{

    public Tilemap _TileMap = null;
    Manager manager;
    MineSweeperGameInfo gameInfo;
    MineSweeperGameInfo.Difficulty difficulty = MineSweeperGameInfo.Difficulty.Easy;
    TableInfo tableInfo; CameraInfo cameraInfo;
    GameObject backGround;
    UnityEngine.UI.Button faceButton;
    MineSweeperSound audio;


    int LeftCellCount;
    void TableInit()
    {
        tableInfo = gameInfo.GetTableInfo(difficulty);
        cameraInfo = gameInfo.GetCameraInfo(difficulty);

        GameObject go = GameObject.FindGameObjectsWithTag("@TileMap")[0];
        _TileMap = go.GetComponent<Tilemap>();
        //Camera Setting
        Vector3 midPoint  = _TileMap.CellToWorld(new Vector3Int(tableInfo.width / 2, tableInfo.height / 2));
        Camera.main.transform.position = cameraInfo.cameraPos;

        //CellTable initializing
        gameInfo.CellTable = new CellType[tableInfo.width, tableInfo.height];
        gameInfo.CellTableInitialize(difficulty);
        gameInfo.DataTable = new module[tableInfo.width, tableInfo.height];
        gameInfo.DataTableInitialize(difficulty);

        //Table Setting
        gameInfo.TileMapInitailze(_TileMap, difficulty);

        //BackGround setting
        backGround = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Desk"));
        BackGroundInfo backGroundInfo = gameInfo.BackGroundInitialize(difficulty);
        backGround.transform.position = backGroundInfo.pos; backGround.transform.localScale = backGroundInfo.scale;

    }
    
    void InterfaceInit()
    {
        GameObject go = GameObject.Find("DifficultySelect");
        TMP_Dropdown difficultSelector = go.GetComponent<TMP_Dropdown>();
        difficultSelector.onValueChanged.AddListener(delegate { ChangeDifficulty(difficultSelector); });

        go = GameObject.Find("FaceButton");
        faceButton = go.GetComponent<UnityEngine.UI.Button>();
        faceButton.onClick.AddListener(delegate { ChangeDifficulty(difficultSelector); });
        faceButton.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Tile>("Art/Sprites/MineSweeperSprites_12").sprite;

    }
    void InterfaceUpdate()
    {
        switch(status)
        {
            case GameStatus.Rest:
                faceButton.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Tile>("Art/Sprites/MineSweeperSprites_12").sprite;
                break;
            case GameStatus.Win:
                faceButton.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Tile>("Art/Sprites/MineSweeperSprites_15").sprite;
                break;
            case GameStatus.Fail:
                faceButton.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Tile>("Art/Sprites/MineSweeperSprites_16").sprite;
                break;
            default:
                break;
        }
    }
    void ChangeDifficulty(TMP_Dropdown _difficultSelector)
    {
        switch(_difficultSelector.value)
        {
            case 0:
                difficulty = Difficulty.Easy;
                break;
            case 1:
                difficulty = Difficulty.Normal;
                break;
            case 2:
                difficulty = Difficulty.Hard;
                break;

        }
        _TileMap.ClearAllTiles();
        GameObject.Destroy(backGround);
        status = GameStatus.Start;

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
        InterfaceInit();

        LeftCellCount = tableInfo.width * tableInfo.height - tableInfo.bombCount;
        audio = new MineSweeperSound();

        status = GameStatus.InGame;
    }
    override public void InGame()
    {
        status = GameStatus.Rest;
    }
    override public void OnWin()
    {
        // ���� ����
        // �޽��� 
    }
    override public void OnFail()
    {
        // ���� ����
        // �޽��� 
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
            // �����Ŷ� ���� ������
            if (hitTarget != previousTarget)
            {
                // �����Ű� �����ְ� ������� �ʾҴٸ�
                if (gameInfo.CellTable[previousTarget.x, previousTarget.y] == CellType.CellOpen && gameInfo.DataTable[previousTarget.x, previousTarget.y].isOpen == false)
                {
                    _TileMap.SetTile(previousTarget, gameInfo.GetTileFromSprites(CellType.CellClose));
                    gameInfo.CellTable[previousTarget.x, previousTarget.y] = CellType.CellClose;
                }

                if (hitTarget.x >= 0 && hitTarget.x < tableInfo.width && hitTarget.y >= 0 && hitTarget.y < tableInfo.height)
                {
                    previousTarget = hitTarget;
                }
                else
                {
                    previousTarget = Vector3Int.zero;
                }
            }

            if (hitTarget.x >= 0 && hitTarget.x < tableInfo.width && hitTarget.y >= 0 && hitTarget.y < tableInfo.height)
            {
                if (gameInfo.DataTable[hitTarget.x, hitTarget.y].isOpen == false && gameInfo.CellTable[hitTarget.x, hitTarget.y] != CellType.Flag)
                {
                    int check = gameInfo.CellOpen(_TileMap, difficulty, hitTarget);
                    if (check == 0)
                    { status = GameStatus.Fail;
                        audio.Play(MineSweeperSound.SoundType.Fail);
                    }
                    else
                    {
                        audio.Play(MineSweeperSound.SoundType.Click);
                        LeftCellCount -= check;
                    } 
                }
            }
        }
        else if (_event == Define.MouseEvent.Lhold)
        {
            Physics.Raycast(ray.origin, ray.direction, out hit, 100f);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red);
            Vector3Int hitTarget = _TileMap.WorldToCell(hit.point);

            //������ ������ �ٲ�°�
            if (hitTarget.x >= 0 && hitTarget.x < tableInfo.width && hitTarget.y >= 0 && hitTarget.y < tableInfo.height)
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

                if (hitTarget.x >= 0 && hitTarget.x < tableInfo.width && hitTarget.y >= 0 && hitTarget.y < tableInfo.height)
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
            // �����Ŷ� ���� ������
            if (hitTarget != previousTarget)
            {
                // �����Ű� �����ְ� ������� �ʾҴٸ�
                if (gameInfo.CellTable[previousTarget.x, previousTarget.y] == CellType.CellOpen && gameInfo.DataTable[previousTarget.x, previousTarget.y].isOpen == false)
                {
                    _TileMap.SetTile(previousTarget, gameInfo.GetTileFromSprites(CellType.CellClose));
                    gameInfo.CellTable[previousTarget.x, previousTarget.y] = CellType.CellClose;
                }

                if (hitTarget.x >= 0 && hitTarget.x < tableInfo.width && hitTarget.y >= 0 && hitTarget.y < tableInfo.height)
                {
                    previousTarget = hitTarget;
                }
                else
                {
                    previousTarget = Vector3Int.zero;
                }
            }
            if (hitTarget.x >= 0 && hitTarget.x < tableInfo.width && hitTarget.y >= 0 && hitTarget.y < tableInfo.height)
            {
                if (gameInfo.CellTable[hitTarget.x, hitTarget.y] == CellType.Flag)
                {
                    gameInfo.CellTable[hitTarget.x, hitTarget.y] = CellType.CellClose;
                    _TileMap.SetTile(hitTarget, gameInfo.GetTileFromSprites(CellType.CellClose));
                    audio.Play(MineSweeperSound.SoundType.DeFlag);
                }
                else if (gameInfo.CellTable[hitTarget.x, hitTarget.y] == CellType.CellOpen && gameInfo.DataTable[hitTarget.x, hitTarget.y].isOpen == false)
                {
                    gameInfo.CellTable[hitTarget.x, hitTarget.y] = CellType.Flag;
                    _TileMap.SetTile(hitTarget, gameInfo.GetTileFromSprites(CellType.Flag));
                    audio.Play(MineSweeperSound.SoundType.Flag);
                }

            }
        }
        else if (_event == Define.MouseEvent.Rhold)
        {
            Physics.Raycast(ray.origin, ray.direction, out hit, 100f);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red);
            Vector3Int hitTarget = _TileMap.WorldToCell(hit.point);

            //������ ������ �ٲ�°�
            if (hitTarget.x >= 0 && hitTarget.x < tableInfo.width && hitTarget.y >= 0 && hitTarget.y < tableInfo.height)
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

                if (hitTarget.x >= 0 && hitTarget.x < tableInfo.width && hitTarget.y >= 0 && hitTarget.y < tableInfo.height)
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


            int x = 0; int y = 0;
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
                x = 0; y = 0;
                for (int i = 0; i < 8; i++)
                {
                    x = previousTarget.x + checkArrayX[i]; y = previousTarget.y + checkArrayY[i];

                    // �ε��� ����
                    if (x < 0 || x >= tableInfo.width) continue;
                    if (y < 0 || y >= tableInfo.height) continue;

                    //������� �ʾҴµ� ���� ���̸� ���� �ɷ� �ٲ���
                    if (gameInfo.CellTable[x, y] == CellType.CellOpen && gameInfo.DataTable[x, y].isOpen == false)
                    {
                        gameInfo.CellTable[x, y] = CellType.CellClose;
                        _TileMap.SetTile(new Vector3Int(x, y, 0), gameInfo.GetTileFromSprites(CellType.CellClose));
                    }
                }

                // ���缿�� �������� ����
                if (hitTarget.x >= 0 && hitTarget.x < tableInfo.width && hitTarget.y >= 0 && hitTarget.y < tableInfo.height)
                {
                    previousTarget = hitTarget;
                }
                else // �ٱ����� ������
                {
                    previousTarget = Vector3Int.zero;
                }
            }

            // ������ ���� ������� ���� ���̰ų�, ����� ���ڼ��ӿ��� �ֺ��� ���ڸ�ŭ�� ����� ���ٸ� 
            // ���ΰ� �ֺ��� �������� ����
            x = 0; y = 0;
            // ���� Ȯ��
            if (hitTarget.x >= 0 && hitTarget.x < tableInfo.width && hitTarget.y >= 0 && hitTarget.y < tableInfo.height)
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
                        if (x < 0 || x >= tableInfo.width) continue;
                        if (y < 0 || y >= tableInfo.height) continue;

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
                    //��� ���� Ȯ��
                    for (int i = 0; i < 8; i++)
                    {
                        x = hitTarget.x + checkArrayX[i]; y = hitTarget.y + checkArrayY[i];
                        // �ε��� ����
                        if (x < 0 || x >= tableInfo.width) continue;
                        if (y < 0 || y >= tableInfo.height) continue;

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
                            if (x < 0 || x >= tableInfo.width) continue;
                            if (y < 0 || y >= tableInfo.height) continue;

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
                            if (x < 0 || x >= tableInfo.width) continue;
                            if (y < 0 || y >= tableInfo.height) continue;

                            // ����� �����ϰ� ����
                            if (gameInfo.CellTable[x, y] != CellType.Flag && gameInfo.DataTable[x,y].isOpen == false)
                            {
                                int check = gameInfo.CellOpen(_TileMap, difficulty, new Vector3Int(x,y,0));
                                if (check == 0)
                                {
                                    status = GameStatus.Fail;
                                    audio.Play(MineSweeperSound.SoundType.Fail);
                                }
                                else
                                {
                                    audio.Play(MineSweeperSound.SoundType.Click);
                                    LeftCellCount -= check;
                                }
                            }
                        }
                        
                    }
                }
            }
            // �ֺ� �� ����
            if (hitTarget.x >= 0 && hitTarget.x < tableInfo.width && hitTarget.y >= 0 && hitTarget.y < tableInfo.height)
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
                    if (x < 0 || x >= tableInfo.width) continue;
                    if (y < 0 || y >= tableInfo.height) continue;

                    //������� �ʾҴµ� ���� ���̸� ���� �ɷ� �ٲ���
                    if (gameInfo.CellTable[x, y] == CellType.CellOpen && gameInfo.DataTable[x,y].isOpen == false)
                    {
                        gameInfo.CellTable[x, y] = CellType.CellClose;
                        _TileMap.SetTile(new Vector3Int(x, y, 0), gameInfo.GetTileFromSprites(CellType.CellClose));
                    }
                }

                // ���缿�� �������� ����
                if (hitTarget.x >= 0 && hitTarget.x < tableInfo.width && hitTarget.y >= 0 && hitTarget.y < tableInfo.height)
                {
                    previousTarget = hitTarget;
                }
                else // �ٱ����� ������
                {
                    previousTarget = Vector3Int.zero;
                }
            }
            //������ ������ �ٲ�°�
            if (hitTarget.x >= 0 && hitTarget.x < tableInfo.width && hitTarget.y >= 0 && hitTarget.y < tableInfo.height)
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
                        if (x < 0 || x >= tableInfo.width) continue;
                        if (y < 0 || y >= tableInfo.height) continue;

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

        if (status != GameStatus.Fail && LeftCellCount == 0)
        {
            status = GameStatus.Win;
            audio.Play(MineSweeperSound.SoundType.Win);
        }

        Debug.Log($"LeftCellCount is {LeftCellCount}! ");

        InterfaceUpdate();
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnAwake()
    {
        throw new System.NotImplementedException();
    }

    public override void OnEnd()
    { 
        throw new System.NotImplementedException();
    }
}
