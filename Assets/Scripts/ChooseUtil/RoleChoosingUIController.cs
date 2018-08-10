using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.EventSystems;
using UnityEngineInternal;

public class RoleChoosingUIController : MonoBehaviour
{
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

    public bool roleSelected { get; private set; } 

    public System.Action confirmFinalAction;

    public void InitBackSpritesUI()
    {

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
    }

    // 到时候在这里为 button 设置效果
    public void SetButtonRoleUnavailable(int gid, int uid)
    {
//        Debug.Log(gid + " " + uid + " selected");
        BarImages[gid].gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].HeadSelect.gameObject.SetActive(true);
        playerIcone[gid * 2 + uid].ConfirmMaskImage.gameObject.SetActive(true);
        playerIcone[gid * 2 + uid].ConfirmFrameImage.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].ChooseFrameImage.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].CrownImage.gameObject.SetActive(false);
        buttons[gid * 2 + uid].interactable = false;
        //        Debug.Log(gid + " " + uid + " unavailable");
    }

    // todo 到时候在这里为 button 设置效果
    public void SetRoleSelected(int gid, int uid, string playerName)
    {
//        Debug.Log(gid + " " + uid + " selected");
        roleSelected = true;
        buttons[gid * 2 + uid].interactable = false;
        BackgroundImage.sprite = BackgroundSprites[gid];
        BarImages[gid].gameObject.SetActive(true);
        AllGroupPanels[gid].transform.SetAsLastSibling();
        playerIcone[gid * 2 + uid].HeadSelect.gameObject.SetActive(true);
        playerIcone[gid * 2 + uid].ConfirmMaskImage.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].NameText.text = playerName;
        playerIcone[gid * 2 + uid].ConfirmFrameImage.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].ChooseFrameImage.gameObject.SetActive(true);
        playerIcone[gid * 2 + uid].CrownImage.gameObject.SetActive(false);
    }
}
