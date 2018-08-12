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
    public AudioClip SwithCharacterAudio;

    public List<Image> BarImages;
    public List<Button> ConfirmButtons;
    public AudioClip SelectCharacterAudio;

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
            pIcone.button.interactable = true;
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
        foreach (PlayerIcone pIcone in playerIcone)
        {
            buttons[i++] = pIcone.button;
        }
        foreach (Button button in ConfirmButtons)
        {
            button.interactable = true;
        }

    }

    // 按下确认键后的操作函数的被调函数
    public void SetButtonRoleLocked(int gid, int uid)
    {
        playerIcone[gid * 2 + uid].ChooseFrameImage.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].ConfirmFrameImage.gameObject.SetActive(true);
        playerIcone[gid * 2 + uid].CrownImage.gameObject.SetActive(true);
        //buttons[gid * 2 + uid].interactable = false;
    }

    // 按下确认键后的操作
    public void OnConfirm(int gid, int uid)
    {
        //if (roleSelected)
        //{
            // 在其他备选按钮添加遮罩
            for (int i = 0; i < 8; ++i)
            {
                if(i != gid * 2 + uid)
                {
                    playerIcone[i].ConfirmMaskImage.gameObject.SetActive(true);
                }
            }
            SetButtonRoleLocked(gid, uid);
            foreach (PlayerIcone pIcone in playerIcone)
            {
                pIcone.button.interactable = false;
            }
            ConfirmButtons[gid].interactable = false;
            // 标志位玩家已确认选择
            //roleSelected = false;
        //}
    }

    // 重新进入新的一局，重置全部状态
    public void ResetChooseRoleUI()
    {
        Debug.Log("reset ChooseRole UI");

        for (int i = 0; i < 8; i++)
        {
            SetButtonRoleAvailable(i / 2, i % 2);
        }
    }

    public void SetRoleNames(Dictionary<int, string> roleId2names)
    {
        for (int i = 0; i < 8; i++)
        {
            playerIcone[i].NameText.text = roleId2names.ContainsKey(i) ? roleId2names[i] : defaultNames[i];
        }
    }

    // 此函数直接替换为初始化全部UI，因为每次调用此函是都是循环8次将全部用户重置
    public void SetButtonRoleAvailable(int gid, int uid)
    {
        playerIcone[gid * 2 + uid].HeadSelect.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].ConfirmMaskImage.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].NameText.text = defaultNames[gid * 2 + uid];
        playerIcone[gid * 2 + uid].ConfirmFrameImage.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].ChooseFrameImage.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].CrownImage.gameObject.SetActive(false);
        playerIcone[gid * 2 + uid].button.interactable = true;

        BarImages[gid].gameObject.SetActive(false);
        ConfirmButtons[gid].interactable = true;
        //buttons[gid * 2 + uid].interactable = true;
    }

    // 其他玩家选择此角色中(当前其他玩家选择中和已确认均为次状态)
    public void SetButtonRoleUnavailable(int gid, int uid)
    {
//        Debug.Log(gid + " " + uid + " selected");
        playerIcone[gid * 2 + uid].HeadSelect.gameObject.SetActive(true);
        playerIcone[gid * 2 + uid].ConfirmMaskImage.gameObject.SetActive(true);
        playerIcone[gid * 2 + uid].button.interactable = false;
//        Debug.Log(gid + " " + uid + " unavailable");
    }

    // 本机角色选中(未确认状态)
    public void SetRoleSelected(int gid, int uid)
    {
//        Debug.Log(gid + " " + uid + " selected");
        // 本机选择
        //roleSelected = true;
        playerIcone[gid * 2 + uid].button.interactable = false;
        AllGroupPanels[gid].transform.SetAsLastSibling();
        BackgroundImage.sprite = BackgroundSprites[gid];
        BarImages[gid].gameObject.SetActive(true);
        playerIcone[gid * 2 + uid].HeadSelect.gameObject.SetActive(true);
        playerIcone[gid * 2 + uid].ChooseFrameImage.gameObject.SetActive(true);
    }

    public void SelectCharactorAudioPlay()
    {
        this.gameObject.GetComponent<AudioSource>().clip = SelectCharacterAudio;
        //this.gameObject.GetComponent<AudioSource>().pitch = 2;
        this.gameObject.GetComponent<AudioSource>().Play();
    }

    public void SwitchCharactorAudioPlay()
    {
        this.gameObject.GetComponent<AudioSource>().clip = SwithCharacterAudio;
        //this.gameObject.GetComponent<AudioSource>().pitch = 2;
        this.gameObject.GetComponent<AudioSource>().Play();
    }
}
