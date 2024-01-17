using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MineSweeperGameInfo;
using static UnityEngine.UI.Image;


public class MineSweeperGameInfo
{
    //Difficulty
    public enum Difficulty
    {
        Easy, Normal, Hard
    }
    public (int, int, float,int) GetTableInfo(Difficulty _difficulty)
    {
        switch (_difficulty)
        {
            case Difficulty.Easy:
                return (9, 9, 1.5f, 10);
            case Difficulty.Normal:
                return (16, 16, 4, 40);
            case Difficulty.Hard:
                return (30, 16, 5, 99);
            default:
                return (0, 0, 0, 0);
        }

    }


    //Cell Resource
    public enum CellType
    {
        cOne, cTwo, cThree, cFour, cFive, cSix, cSeven, cEight, cNine, cZero, cBar, cNone,
        SmileClose, SmileOpen, ShockFace, WinFace, DeadFace, CellClose , CellOpen, Flag, QuestionClose, QeustionOpen,
        Bomb, BombOpen, BombCross,
        One, Two, Three, Four, Five, Six, Seven, Eight, NotOpen
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
        int width, height, bombCount;
        float notUsing;
        (width, height, notUsing, bombCount) = GetTableInfo(_difficulty);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                CellTable[i, j] = CellType.CellClose;
            }
        }

    }
    // DataTable
    public int[,] DataTable;

    public void DataTableInitialize(Difficulty _difficulty)
    {
        int width, height, bombCount;
        float notUsing;
        (width, height, notUsing, bombCount) = GetTableInfo(_difficulty);

        //inputBomb
        for (int i = 0; i < bombCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0,height);
        while (DataTable[x,y] == -1)
        {
            x = Random.Range(0, width);
            y = Random.Range(0, height);
        }

        DataTable[x, y] = -1;

        }
        int[] checkArrayX = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] checkArrayY = { 1, 1, 1, 0, 0, -1, -1, -1 };
        //inputMarker
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (DataTable[i, j] == -1) continue;
                for (int k = 0; k < 8; k++)
                {
                    if (DataTable[Mathf.Clamp(i + checkArrayX[k], 0, width - 1), Mathf.Clamp(j + checkArrayY[k], 0, height - 1)] == -1)
                    {
                        DataTable[i, j] += 1;
                    }
                }
            }
        }
    }


    //TileMap
    public void TileMapInitailze(in Tilemap _map, Difficulty _difficulty)
    {
        int width, height, bombCount;
        float notUsing;
        (width, height, notUsing, bombCount) = GetTableInfo(_difficulty);
        Vector3Int origin = Vector3Int.zero;
        Tile CellClose = GetTileFromSprites(MineSweeperGameInfo.CellType.CellClose);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                _map.SetTile(origin + i * Vector3Int.right + j * Vector3Int.up, CellClose);

            }
        }
    }

    public void CellOpen(in Tilemap _map, Difficulty _difficulty, Vector3Int _location)
    {
        int width, height, bombCount;
        float notUsing;
        (width, height, notUsing, bombCount) = GetTableInfo(_difficulty);

        if (DataTable[_location.x,_location.y] > 0)
        {
            _map.SetTile(_location, GetTileFromSprites((CellType)(24 + DataTable[_location.x, _location.y])));
            CellTable[_location.x, _location.y] = (CellType)(24 + DataTable[_location.x, _location.y]);
        }


        if (DataTable[_location.x, _location.y] == 0)
        {
            _map.SetTile(_location, GetTileFromSprites(CellType.CellOpen));
            CellTable[_location.x, _location.y] = CellType.CellOpen;
        }
    }

}
