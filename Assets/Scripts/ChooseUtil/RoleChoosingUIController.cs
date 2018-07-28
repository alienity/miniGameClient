using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleChoosingUIController : MonoBehaviour {
    public Button P1Pengu;
    public Button P1Pig;
    public Button P2Pengu;
    public Button P2Pig;
    public Button P3Pengu;
    public Button P3Pig;
    public Button P4Pengu;
    public Button P4Pig;
    public Sprite PenguHeadAltern;
    public Sprite PenguHeadSelect;
    public Sprite PenguHeadLock;
    public Sprite PigHeadAltern;
    public Sprite PigHeadSelect;
    public Sprite PigHeadLock;
    public Sprite ConfrimButtonNoPush;
    public Sprite ConfrimButtonPushed;

    [System.Serializable]
    public struct SpritesUI
    {
        public Image PangBackgroundImage;
        public Image PigBackgroundImage;
        public Image StarImage;
    }
    public List<SpritesUI> spritesUI;

    public Button confirmButton;

    [HideInInspector] public Button[] buttons;

    public bool roleSelected { get; private set; } 

    public System.Action confirmFinalAction;

    public void InitBackSpritesUI()
    {
        foreach(SpritesUI su in spritesUI)
        {
            su.PangBackgroundImage.gameObject.SetActive(false);
            su.PigBackgroundImage.gameObject.SetActive(false);
            su.StarImage.gameObject.SetActive(false);
        }
    }
    public void InitButtons()
    {
        buttons = new Button[8];
        buttons[0] = P1Pengu;
        buttons[1] = P1Pig;
        buttons[2] = P2Pengu;
        buttons[3] = P2Pig;
        buttons[4] = P3Pengu;
        buttons[5] = P3Pig;
        buttons[6] = P4Pengu;
        buttons[7] = P4Pig;
        confirmButton.GetComponent<Image>().sprite = ConfrimButtonNoPush;
        for (int i = 0; i < 4; ++i)
        {
            //button.image.color = Color.white;D:\github_MiniGame\miniGameServer\Assets\Resource\Textures\ChooseRoleSceneICon
            buttons[2 * i].GetComponent<Image>().sprite = PenguHeadAltern;
            buttons[2 * i + 1].GetComponent<Image>().sprite = PigHeadAltern;
        }
    }

    public void OnConfirm(int gid, int uid)
    {
        if (roleSelected)
        {
            // 在选择角色按钮上添加一层“蒙布”
            for (int i = 0; i < 4; ++i)
            {
                if(buttons[2 * i].GetComponent<Image>().sprite == PenguHeadAltern)
                    buttons[2 * i].GetComponent<Image>().sprite = PenguHeadSelect;
                if(buttons[2 * i + 1].GetComponent<Image>().sprite == PigHeadAltern)
                    buttons[2 * i + 1].GetComponent<Image>().sprite = PigHeadSelect;
            }
            buttons[gid * 2 + uid].GetComponent<Image>().sprite = (uid == 0) ? PenguHeadLock : PigHeadLock;
            spritesUI[gid].StarImage.gameObject.SetActive(true);
            confirmButton.GetComponent<Image>().sprite = ConfrimButtonPushed;
            foreach (Button button in buttons)
            {
                button.interactable = false;
            }

            confirmButton.interactable = true;
        }
    }

    public void ResetUI()
    {
        for (int i = 0; i < 8; i++)
        {
            SetButtonRoleAvailable(i / 2, i % 2);
        }

        confirmButton.interactable = true;
    }
    
    // 到时候在这里为 button 设置效果
    public void SetButtonRoleAvailable(int gid, int uid)
    {
        Debug.Log(gid+ " " + uid + " available");
        //buttons[gid*2 + uid].interactable = true;
        buttons[gid * 2 + uid].GetComponent<Image>().sprite = (uid == 0) ? PenguHeadAltern : PigHeadAltern;
        if (uid == 0)
        {
            spritesUI[gid].PangBackgroundImage.gameObject.SetActive(false);
        }
        else
        {
            spritesUI[gid].PigBackgroundImage.gameObject.SetActive(false);
        }

    }

    // 到时候在这里为 button 设置效果
    public void SetButtonRoleUnavailable(int gid, int uid)
    {
        Debug.Log(gid + " " + uid + " selected");
        buttons[gid * 2 + uid].GetComponent<Image>().sprite = (uid == 0) ? PenguHeadSelect : PigHeadSelect;
        if (uid == 0)
        {
            spritesUI[gid].PangBackgroundImage.gameObject.SetActive(false);
        }
        else
        {
            spritesUI[gid].PigBackgroundImage.gameObject.SetActive(false);
        }
        Debug.Log(gid + " " + uid + " unavailable");
    }

    // todo 到时候在这里为 button 设置效果
    public void SetRoleSelected(int gid, int uid)
    {
        Debug.Log(gid + " " + uid + " selected");
        roleSelected = true;
        if (uid == 0)
        {
            spritesUI[gid].PangBackgroundImage.gameObject.SetActive(true);
        }
        else
        {
            spritesUI[gid].PigBackgroundImage.gameObject.SetActive(true);
        }
    }
}
