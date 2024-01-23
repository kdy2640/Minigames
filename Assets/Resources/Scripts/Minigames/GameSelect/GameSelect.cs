using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class GameSelect : MiniGame
{
    Manager manager;
    InputManager input;



    int CameraIndex = 0;
    bool isAdapting = false;
    Vector3 Destination;
    public override void InGame()
    {
        if (isAdapting)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, Destination, 0.5f);
            if((Camera.main.transform.position - Destination).magnitude < 0.2f)
            {
                status = GameStatus.Rest;
                isAdapting = false;
            }
        }
        else
        {
            float relativeIndex = (Camera.main.transform.position.x + 4.5f) / 5.75f;
            CameraIndex = Mathf.Clamp(Mathf.RoundToInt(relativeIndex),0,gameArray.Length  - 1);
            Destination = new Vector3(- 4.5f + 5.75f * CameraIndex,0,-10);
            isAdapting = true;
        }
    }

    public override void OnAwake()
    {
    }

    bool isHold = false;
    Vector3 originMousePosition;
    Vector3 originCameraPosition;
    float DragSpeed = 0.05f;
    const float RIGHTEND = 15;
    const float LEFTEND = -7f;
    public override void OnClick(Define.MouseEvent _event)
    {
        if (status != GameStatus.Rest) return;

        if (_event == Define.MouseEvent.Lhold)
        {
            if(isHold)
            {
                float x = originCameraPosition.x + DragSpeed * (originMousePosition.x - Input.mousePosition.x);
                if (x > LEFTEND && x < RIGHTEND )
                {
                    Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, originCameraPosition + DragSpeed * new Vector3(originMousePosition.x - Input.mousePosition.x, 0, 0),0.9f);
                }else if ( x<= LEFTEND)
                {
                    Camera.main.transform.position = new Vector3(LEFTEND, 0, -10);
                }
                else
                {
                    Camera.main.transform.position = new Vector3(RIGHTEND, 0, -10);
                }

            }
            else
            {
                originMousePosition = Input.mousePosition;
                originCameraPosition = Camera.main.transform.position;
                isHold = true;
            }
        }
        if(_event == Define.MouseEvent.Lrelease)
        {
            isHold = false;
            status = GameStatus.InGame;
        }

    }

    public override void OnEnd()
    {
    }

    public override void OnFail()
    {
    }

    public override void OnStart()
    {
        manager = Manager.manager;
        input = manager.input;
        input.OnMouseAction -= OnClick;
        input.OnMouseAction += OnClick;

        GameBodyInitializer();
        ButtonInitializer();
        
        status = GameStatus.Rest;
    }


    //GameBody Interaction

    struct GameBody
    {
        public string name;
        public Define.MiniGameType Type;
        public Vector3 location;
        public int index;
        public GameObject gameBody;
        public bool isOpen;

        public GameBody(string name, Define.MiniGameType type, int index, GameObject gameBody, bool isOpen)
        {
            this.name = name;
            Type = type;
            this.index =  index;
            this.gameBody = gameBody;
            this.isOpen = isOpen;
            location = new Vector3(-4.5f + 5.75f * index, -0.5f, -1);
        }
    }

    GameBody[] gameArray;
    public void GameBodyInitializer()
    {
        gameArray = new GameBody[(int)Define.MiniGameType.Count];
        for (int i = 0; i < (int)Define.MiniGameType.Count; i++)
        {

            gameArray[i] = new GameBody(((Define.MiniGameType)i).ToString(), (Define.MiniGameType)i , i, null, true);

            //ÇÁ¸®ÆÕ »ý¼º
            GameObject go = Resources.Load<GameObject>("Prefabs/GameBody");
            GameObject instance = GameObject.Instantiate(go);
            gameArray[i].gameBody = instance;


            if (i == (int)Define.MiniGameType.Count - 1)
            { 
                gameArray[i].isOpen = false; gameArray[i].name = "";
                GameObject cover = Resources.Load<GameObject>("Prefabs/GameCover");
                GameObject.Instantiate(cover, instance.transform);
            }
            Debug.Log("Art/Materials/GameBody/" + gameArray[i].name + "GameImage");

            instance.transform.Find("GameImage").GetComponent<MeshRenderer>().material = Resources.Load<Material>("Art/Materials/GameBody/" + gameArray[i].name + "GameImage");
            instance.transform.Find("GameName").GetComponent<TMP_Text>().text = gameArray[i].name;
            instance.transform.position = gameArray[i].location;
            
        }




    }
        //Button Interaction
     public void ButtonInitializer()
    {

        GameObject go = GameObject.Find("LeftButton");
        Button LeftButton = go.GetComponent<Button>();
        LeftButton.onClick.AddListener(delegate { LeftButtonOnClicked(); });


        go = GameObject.Find("RightButton");
        Button RighttButton = go.GetComponent<Button>();
        RighttButton.onClick.AddListener(delegate { RightButtonOnClicked(); });

    }
    public void LeftButtonOnClicked()
    {
        if (CameraIndex == 0)
        {
            Destination = new Vector3(-4.5f + 5.75f * (gameArray.Length - 1), 0, -10);
        }
        else
        {
            Destination = new Vector3(-4.5f + 5.75f * (CameraIndex - 1), 0, -10);
        }
        while (true)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, Destination, 0.1f);
            if ((Camera.main.transform.position - Destination).magnitude < 0.2f)
            {
                status = GameStatus.Rest;
                break;
            }
        }
    }
    public void RightButtonOnClicked()
    {
        if (CameraIndex == gameArray.Length - 1)
        {
            Destination = new Vector3(-4.5f, 0, -10);
        }else
        {
            Destination = new Vector3(-4.5f + 5.75f * (CameraIndex + 1 ), 0, -10);
        }
        while (true)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, Destination, 0.1f);
            if ((Camera.main.transform.position - Destination).magnitude < 0.2f)
            {
                status = GameStatus.Rest;
                break;
            }
        }
    }


    public override void OnWin()
    {
    }
}
