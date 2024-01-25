using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public MiniGame game { get { return _game; } }

    private static Define.MiniGameType _gameName = Define.MiniGameType.Count;

    public SceneController scene = new SceneController();
    public void ChangeMiniGame(Define.MiniGameType _type)
    {
        if(_type != Define.MiniGameType.Count)
        {
            SceneManager.LoadScene(_type.ToString());
        }
        else
        {
            SceneManager.LoadScene("Main");
        }
        _gameName = _type;
        _game = MiniGame.GetMinigame(_gameName);
        _game.status = MiniGame.GameStatus.Awake;
    }
    private static void init()
    {
        if (_manager == null)
        {
            Manager mg = GameObject.Find("@Manager").GetComponent<Manager>();
            if (mg == null)
            {
                GameObject go = new GameObject("@Manager");
                go.AddComponent<Manager>();
                DontDestroyOnLoad(go);
                mg = go.GetComponent<Manager>();
            }
            DontDestroyOnLoad(mg);
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

    void LateUpdate()
    {
        
    }
}
