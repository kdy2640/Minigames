using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Manager : MonoBehaviour
{
    ///  Managers
    private static Manager _manager;
    public static Manager manager
    {
        get
        {
            init();
            return _manager;
        }
    }
    private InputManager _input = new InputManager();
    public InputManager input { get { return _input; } }

    // variables
    private MiniGame _game = MiniGame.GetMinigame(_gameName);
    private MiniGame game
    {
        get { return _game;}
    }

    private static Define.MiniGameStatus _gameName = Define.MiniGameStatus.MineSweeper;
    public Define.MiniGameStatus gameName { get { return _gameName;} set { _game = MiniGame.GetMinigame(value);  _gameName = value;  } }

    private static void init()
    {
        if (_manager == null)
        {
            Manager mg = FindObjectOfType<Manager>();
            if (mg == null)
            {
                GameObject go = new GameObject("@Manager");
                go.AddComponent<Manager>();
                DontDestroyOnLoad(go);
                mg = go.GetComponent<Manager>();
            }
            _manager = mg;
        }
    }
    void Awake()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        input.OnUpdate();
        game.OnUpdate();
    }
}
