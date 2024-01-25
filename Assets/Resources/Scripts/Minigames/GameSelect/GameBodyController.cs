using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBodyController : MonoBehaviour
{

    Vector3 origin;
    bool isUp;
    bool isEanble = true;
    string name;
    private void Start()
    {
        origin = transform.position;
        name = transform.Find("GameName").GetComponent<TMP_Text>().text;
        if (name == "")
        {
            isEanble = false;
        }else
        {
            cover = transform.Find("GameCover").gameObject;
        }
        
    }
    public void OnPointerEnter()
    {
        if(isEanble)
        {
            isUp = true;
            StartCoroutine("PopUpGameBody");
        }
    }
    IEnumerator PopUpGameBody()
    {
        while(isUp)
        {
            transform.position = Vector3.Lerp(transform.position, origin + Vector3.back, 0.2f);
            if((transform.position - origin).magnitude < 0.2f)
            {
                transform.position = origin + Vector3.back;
                isUp = false;
                yield break;
            }
            yield return null;
        }
    }
    IEnumerator PopDownGameBody()
    {
        while(!isUp)
        {
            transform.position = Vector3.Lerp(transform.position, origin, 0.6f);
            if ((transform.position - origin).magnitude < 0.2f)
            {
                transform.position = origin; 
                yield break;
            }
            yield return null;
        }
    }
    public void OnPointerExit()
    {
        if (isEanble)
        {
            isUp = false;
            StartCoroutine("PopDownGameBody");
        }
    }


    GameObject cover;
    public void OnPointerDown()
    {
        if (isEanble)
        {
            cover.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Art/Materials/GameBody/CoverClickMaterial");
            cover.SetActive(true);
        }
    }
    public void OnPointerUp()
    {
        if (isEanble)
        {
            cover.SetActive(false);
            cover.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Art/Materials/GameBody/CoverMaterial");
        }
    }
    public void OnPointerClick()
    {
        if (isEanble)
        {
            //¾À ¹Ù²Ù±â
            GameSelect.GameType = (Define.MiniGameType)Enum.Parse(typeof(Define.MiniGameType), name);
            Manager.manager.game.status = MiniGame.GameStatus.End;

        }
    }
}
