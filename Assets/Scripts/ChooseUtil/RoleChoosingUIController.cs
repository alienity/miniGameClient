using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.EventSystems;
using UnityEngineInternal;

public class RoleChoosingUIController : MonoBehaviour
{
    //public Button P1Pengu;
    //public Button P1Pig;
    //public Button P2Pengu;
    //public Button P2Pig;
    //public Button P3Pengu;
    //public Button P3Pig;
    //public Button P4Pengu;
    //public Button P4Pig;

    public List<Sprite> BackgroundSprites;
    public Image BackgroundImage;

    public List<GameObject> AllGroupPanels;

    [System.Serializable]
    public struct PlayerIcone
    {
        public Image HeadSelect;
        public Image ConfirmMaskImage;
        public Text NameText;
        public Image ConfirmFrameImage;
        public Image ChooseFrameImage;
        public Image CrownImage;
        public Button button;
    }
    public List<PlayerIcone> playerIcone = new List<PlayerIcone>();
    [HideInInspector] public Button[] buttons;

    public List<Image> BarImages;
    public List<Button> ConfirmButtons;

    //public Sprite PenguHeadAltern;
    //public Sprite PenguHeadSelect;
    //public Sprite PenguHeadLock;
    //public Sprite PigHeadAltern;
    //public Sprite PigHeadSelect;
    //public Sprite PigHeadLock;
    //public Sprite ConfrimButtonNoPush;
    //public Sprite ConfrimButtonPushed;

    //[System.Serializable]
    //public struct SpritesUI
    //{
    //    public Image PangBackgroundImage;
    //    public Image PigBackgroundImage;
    //    public Image StarImage;
    //}
    //public List<SpritesUI> spritesUI;
    //public List<Text> playerNames;
    private List<string> defaultNames = new List<string>()
    {
        "企鹅-P1",
        "猪猪-P1",
        "企鹅-P2",
        "猪猪-P2",
        "企鹅-P3",
        "猪猪-P3",
        "企鹅-P4",
        "猪猪-P4"
    };

    //public Button confirmButton;

    public bool roleSelected { get; private set; } 

    public System.Action confirmFinalAction;

    public void InitBackSpritesUI()
    {
        //foreach(SpritesUI su in spritesUI)
        //{
        //    su.PangBackgroundImage.gameObject.SetActive(false);
        //    su.PigBackgroundImage.gameObject.SetActive(false);
        //    su.StarImage.gameObject.SetActive(false);
        //}

        BackgroundImage.sprite = null;
        int i = 0;
        foreach (PlayerIcone pIcone in playerIcone)
        {
            pIcone.HeadSelect.gameObject.SetActive(false);
            pIcone.ConfirmMaskImage.gameObject.SetActive(false);
            pIcone.NameText.text = defaultNames[i++];
            pIcone.ConfirmFrameImage.gameObject.SetActive(false);
            pIcone.ChooseFrameImage.gameObject.SetActive(false);
            pIcone.CrownImage.gameObject.SetActive(false);
        }

        foreach(Image im in BarImages)
        {
            im.gameObject.SetActive(false);
        }
    }

    public void InitButtons()
    {
        buttons = new Button[8];
        int i = 0;
        foreach(PlayerIcone pIcone in playerIcone)
        {
            buttons[i++] = pIcone.button;
        }

        /* 旧版本
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
            buttons[2 * i].GetComponent<Image>().sprite = PenguHeadAltern;
            buttons[2 * i + 1].GetComponent<Image>().sprite = PigHeadAltern;
        }
        */

    }

    public void SetButtonRoleLocked(int gid, int uid)
    {
        //buttons[gid * 2 + uid].GetComponent<Image>().sprite = (uid == 0) ? PenguHeadLock : PigHeadLock;
        
        playerIcone[gid * 2 + uid].HeadSelect.gameObject.SetActive(true);
        playerIcone[gid * 2 + uid].ConfirmFrameImage.gameObject.SetActive(true);
        playerIcone[gid * 2 + uid].ChooseFrameImage.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].CrownImage.gameObject.SetActive(true);
        //buttons[gid * 2 + uid].interactable = false;
    }

    public void OnConfirm(int gid, int uid, string platerName)
    {
        if (roleSelected)
        {
            playerIcone[gid * 2 + uid].NameText.text = platerName;
            // 在其他备选按钮添加遮罩
            for (int i = 0; i < 8; ++i)
            {
                if(i != gid * 2 + uid)
                {
                    playerIcone[i].ConfirmMaskImage.gameObject.SetActive(true);
                }
            }
            SetButtonRoleLocked(gid, uid);
            foreach (Button button in buttons)
            {
                button.interactable = false;
            }
            ConfirmButtons[gid].interactable = false;
            // 重置判断
            roleSelected = false;

            /* 旧版本
            // 在选择角色按钮上添加一层“蒙布”
            for (int i = 0; i < 4; ++i)
            {
                if(buttons[2 * i].GetComponent<Image>().sprite == PenguHeadAltern)
                    buttons[2 * i].GetComponent<Image>().sprite = PenguHeadSelect;
                if(buttons[2 * i + 1].GetComponent<Image>().sprite == PigHeadAltern)
                    buttons[2 * i + 1].GetComponent<Image>().sprite = PigHeadSelect;
            }
            SetButtonRoleLocked(gid, uid);
            spritesUI[gid].StarImage.gameObject.SetActive(true);
            confirmButton.GetComponent<Image>().sprite = ConfrimButtonPushed;
            foreach (Button button in buttons)
            {
                button.interactable = false;
            }

            confirmButton.interactable = false;
            // 重置判断
            roleSelected = false;
            */
        }
    }

    public void ResetChooseRoleUI()
    {
        Debug.Log("reset ChooseRole UI");
        for (int i = 0; i < 8; i++)
        {
            SetButtonRoleAvailable(i / 2, i % 2);
        }
        //confirmButton.interactable = true;
    }

    public void SetRoleNames(Dictionary<int, string> roleId2names)
    {
        for (int i = 0; i < 8; i++)
        {
            playerIcone[i].NameText.text = roleId2names.ContainsKey(i) ? roleId2names[i] : defaultNames[i];
        }
    }
    
    // 到时候在这里为 button 设置效果
    public void SetButtonRoleAvailable(int gid, int uid)
    {
        playerIcone[gid * 2 + uid].HeadSelect.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].NameText.text = defaultNames[gid * 2 + uid];
        playerIcone[gid * 2 + uid].ChooseFrameImage.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].CrownImage.gameObject.SetActive(false);
        BarImages[gid].gameObject.SetActive(false);
        buttons[gid * 2 + uid].interactable = true;
        /* 旧版本
        //        Debug.Log(gid+ " " + uid + " available");
        //buttons[gid*2 + uid].interactable = true;

        buttons[gid * 2 + uid].GetComponent<Image>().sprite = (uid == 0) ? PenguHeadAltern : PigHeadAltern;
        buttons[gid * 2 + uid].interactable = true;

        if (uid == 0)
        {
            spritesUI[gid].PangBackgroundImage.gameObject.SetActive(false);
        }
        else
        {
            spritesUI[gid].PigBackgroundImage.gameObject.SetActive(false);
        }
        */
    }

    // 到时候在这里为 button 设置效果
    public void SetButtonRoleUnavailable(int gid, int uid)
    {
        BarImages[gid].gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].HeadSelect.gameObject.SetActive(true);
        playerIcone[gid * 2 + uid].ConfirmMaskImage.gameObject.SetActive(true);
        playerIcone[gid * 2 + uid].ConfirmFrameImage.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].ChooseFrameImage.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].CrownImage.gameObject.SetActive(false);
        buttons[gid * 2 + uid].interactable = false;
        /* 旧版本
//        Debug.Log(gid + " " + uid + " selected");
        buttons[gid * 2 + uid].GetComponent<Image>().sprite = (uid == 0) ? PenguHeadSelect : PigHeadSelect;
        buttons[gid * 2 + uid].interactable = false;
        if (uid == 0)
        {
            spritesUI[gid].PangBackgroundImage.gameObject.SetActive(false);
        }
        else
        {
            spritesUI[gid].PigBackgroundImage.gameObject.SetActive(false);
        }
//        Debug.Log(gid + " " + uid + " unavailable");
        */
    }

    // todo 到时候在这里为 button 设置效果
    public void SetRoleSelected(int gid, int uid, string playerName)
    {
        roleSelected = true;
        buttons[gid * 2 + uid].interactable = false;
        BackgroundImage.sprite = BackgroundSprites[gid];
        BarImages[gid].gameObject.SetActive(true);
        //GameObject GroupPanel = BarImages[gid].gameObject.transform.parent.gameObject.transform.parent.gameObject;
        //GroupPanel.transform.SetAsLastSibling();
        AllGroupPanels[gid].transform.SetAsLastSibling();
        playerIcone[gid * 2 + uid].HeadSelect.gameObject.SetActive(true);
        playerIcone[gid * 2 + uid].ConfirmMaskImage.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].NameText.text = playerName;
        playerIcone[gid * 2 + uid].ConfirmFrameImage.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].ChooseFrameImage.gameObject.SetActive(true);
        playerIcone[gid * 2 + uid].CrownImage.gameObject.SetActive(false);
        /* 旧版本
//        Debug.Log(gid + " " + uid + " selected");
        roleSelected = true;
        buttons[gid * 2 + uid].interactable = false;
        if (uid == 0)
        {
            spritesUI[gid].PangBackgroundImage.gameObject.SetActive(true);
        }
        else
        {
            spritesUI[gid].PigBackgroundImage.gameObject.SetActive(true);
        }
        */
    }
}
