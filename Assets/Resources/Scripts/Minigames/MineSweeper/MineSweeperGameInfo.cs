using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MineSweeperGameInfo;
using static UnityEngine.UI.Image;


public class MineSweeperGameInfo
{
    // TableInfo

    public struct TableInfo
    {
        public int width;
        public int height;
        public int bombCount;
        public TableInfo(int _width, int _height, int _bombCount)
        {
            width = _width; height = _height; bombCount = _bombCount;
        }
    }

    public struct CameraInfo
    {
        public Vector3 cameraPos;
        public CameraInfo(Vector3 _cameraPos)
        {
            cameraPos = _cameraPos;
        }
    }

    public struct BackGroundInfo
    {
        public Vector3 pos;
        public Vector3 scale;

        public BackGroundInfo(Vector3 _pos , Vector3 _scale)
        {
            pos = _pos; scale = _scale;
        }
    }

    //Difficulty
    public enum Difficulty
    {
        Easy, Normal, Hard
    }
    public TableInfo GetTableInfo(Difficulty _difficulty)
    {
        switch (_difficulty)
        {
            case Difficulty.Easy:
                return new TableInfo(9, 9, 10);
            case Difficulty.Normal:
                return new TableInfo(16, 16, 40);
            case Difficulty.Hard:
                return new TableInfo(24, 20, 99);
            default:
                return new TableInfo(0, 0, 0);
        }

    }
    public CameraInfo GetCameraInfo(Difficulty _difficulty)
    {
        switch (_difficulty)
        {
            case Difficulty.Easy:
                return new CameraInfo(new Vector3(0.85f, 0.65f,-1.3f));
            case Difficulty.Normal:
                return new CameraInfo(new Vector3(1.45f, 1.2f,-2.2f));
            case Difficulty.Hard:
                return new CameraInfo(new Vector3(1.8f, 1.5f,-2.7f));
            default:
                return new CameraInfo(new Vector3(0, 0,0));
        }

    }

    public BackGroundInfo BackGroundInitialize(Difficulty _difficulty)
    {
        switch (_difficulty)
        {
            case Difficulty.Easy:
                return new BackGroundInfo(new Vector3(1.79f, 0.45f, 2f), new Vector3(0.65f, 1f, 0.5f));
            case Difficulty.Normal:
                return new BackGroundInfo(new Vector3(2.24f, 1.2f, 2f), new Vector3(0.74f, 1.63f, 0.55f));
            case Difficulty.Hard:
                return new BackGroundInfo(new Vector3(2.75f, 1.55f, 2f), new Vector3(0.85f, 1f, 0.56f));
            default:
                return new BackGroundInfo(Vector3.zero, Vector3.zero);
        }

    }

    //Cell Resource
    public enum CellType
    {
        cOne, cTwo, cThree, cFour, cFive, cSix, cSeven, cEight, cNine, cZero, cBar, cNone,
        SmileClose, SmileOpen, ShockFace, WinFace, DeadFace, CellClose , CellOpen, Flag, QuestionClose, QeustionOpen,
        Bomb, BombOpen, BombCross,
        One, Two, Three, Four, Five, Six, Seven, Eight
    }

    private const string TILE_DIRECTORY = "Art/Sprites/MineSweeperSprites_";
    private static Tile[] TileArray = new Tile[33];
    public Tile GetTileFromSprites(CellType _type)
    {
        Tile output;
        int type = (int)_type;
        if (TileArray[type] != null) return TileArray[type];
        string directory = TILE_DIRECTORY + type.ToString();
        output = Resources.Load<Tile>(directory);
        
        if (output == null) Debug.Log("Error: There is no Tile");
        TileArray[type] = output;

        return output;
    }

    // CellTable
    public CellType[,] CellTable;
    public void CellTableInitialize(Difficulty _difficulty)
    {
        TableInfo tableInfo = GetTableInfo(_difficulty);

        for (int i = 0; i < tableInfo.width; i++)
        {
            for (int j = 0; j < tableInfo.height; j++)
            {
                CellTable[i, j] = CellType.CellClose;
            }
        }

    }
    // DataTable
    public struct module
    {
        public int data;
        public bool isOpen;
    }
    public module[,] DataTable;

    public void DataTableInitialize(Difficulty _difficulty)
    {
        TableInfo tableInfo = GetTableInfo(_difficulty);

        //inputBomb
        for (int i = 0; i < tableInfo.bombCount; i++)
        {
            int x = Random.Range(0, tableInfo.width);
            int y = Random.Range(0,tableInfo.height);
        while (DataTable[x,y].data == -1)
        {
            x = Random.Range(0, tableInfo.width);
            y = Random.Range(0, tableInfo.height);
        }

        DataTable[x, y].data = -1;

        }
        int[] checkArrayX = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] checkArrayY = { 1, 1, 1, 0, 0, -1, -1, -1 };
        //inputMarker
        for (int i = 0; i < tableInfo.width; i++)
        {
            for (int j = 0; j < tableInfo.height; j++)
            {
                DataTable[i,j].isOpen = false;
                if (DataTable[i, j].data == -1) continue;
                for (int k = 0; k < 8; k++)
                {
                    if (i + checkArrayX[k] < 0 || i + checkArrayX[k] >= tableInfo.width) continue;
                    if (j + checkArrayY[k] < 0 || j + checkArrayY[k] >= tableInfo.height) continue;
                    if (DataTable[i + checkArrayX[k], j + checkArrayY[k]].data == -1)
                    {
                        DataTable[i, j].data += 1;
                    }
                }
            }
        }
    }


    //TileMap
    public void TileMapInitailze(in Tilemap _map, Difficulty _difficulty)
    {
        TableInfo tableInfo = GetTableInfo(_difficulty);
        Vector3Int origin = Vector3Int.zero;
        Tile CellClose = GetTileFromSprites(MineSweeperGameInfo.CellType.CellClose);
        for (int i = 0; i < tableInfo.width; i++)
        {
            for (int j = 0; j < tableInfo.height; j++)
            {
                _map.SetTile(origin + i * Vector3Int.right + j * Vector3Int.up, CellClose);

            }
        }
    }

    public int CellOpen(in Tilemap _map, Difficulty _difficulty, Vector3Int _location)
    {

        TableInfo tableInfo = GetTableInfo(_difficulty);
        DataTable[_location.x, _location.y].isOpen = true;
        if (DataTable[_location.x,_location.y].data > 0)
        {
            _map.SetTile(_location, GetTileFromSprites((CellType)(24 + DataTable[_location.x, _location.y].data)));
            CellTable[_location.x, _location.y] = (CellType)(24 + DataTable[_location.x, _location.y].data);
            return 1;
        }
        else if (DataTable[_location.x, _location.y].data == 0)
        {
            return BlankOpen(_map, _difficulty, _location);

        }
        else if (DataTable[_location.x, _location.y].data == -1)
        {
            _map.SetTile(_location, GetTileFromSprites(CellType.BombOpen));
            CellTable[_location.x, _location.y] = CellType.BombOpen;

            return 0;
            
        }
        return 0;
    }

    public int BlankOpen(in Tilemap _map, Difficulty _difficulty, Vector3Int _location)
    {
        int output = 1;
        _map.SetTile(_location, GetTileFromSprites(CellType.CellOpen));
        CellTable[_location.x, _location.y] = CellType.CellOpen;

        TableInfo tableInfo = GetTableInfo(_difficulty);

        int[] checkArrayX = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] checkArrayY = { 1, 1, 1, 0, 0, -1, -1, -1 };


        int i = _location.x; int j = _location.y;
        DataTable[i, j].isOpen = true;
        for (int k = 0; k < 8; k++)
        {
            if (i + checkArrayX[k] < 0 || i + checkArrayX[k] >= tableInfo.width) continue;
            if (j + checkArrayY[k] < 0 || j + checkArrayY[k] >= tableInfo.height) continue;
            if (DataTable[i + checkArrayX[k], j + checkArrayY[k]].isOpen == false)
            {
                DataTable[i + checkArrayX[k], j + checkArrayY[k]].isOpen = true;
                if (DataTable[i + checkArrayX[k], j + checkArrayY[k]].data > 0)
                {
                    output++;
                    _map.SetTile(new Vector3Int(i + checkArrayX[k], j + checkArrayY[k], -0), GetTileFromSprites((CellType)(24 + DataTable[i + checkArrayX[k], j + checkArrayY[k]].data)));
                    CellTable[i + checkArrayX[k], j + checkArrayY[k]] = (CellType)(24 + DataTable[i + checkArrayX[k], j + checkArrayY[k]].data);
                }
                else if (DataTable[i + checkArrayX[k], j + checkArrayY[k]].data == 0)
                {
                    output += BlankOpen(_map, _difficulty, new Vector3Int(i + checkArrayX[k], j + checkArrayY[k],-0));
                }
                else if (DataTable[i + checkArrayX[k], j + checkArrayY[k]].data == -1)
                {
                    continue;
                }
            }
        }

        return output;


    }


}
