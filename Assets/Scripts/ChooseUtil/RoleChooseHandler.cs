using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RoleChooseHandler : MonoBehaviour
{
    private RoleChoosingUIController roleChoosingUiController;
    private GamePanelUIController gamePanelUIController;
    private NetworkClient networkClient;

    private void Start()
    {
        if (roleChoosingUiController == null)
            roleChoosingUiController = FindObjectOfType<RoleChoosingUIController>();
        if (gamePanelUIController == null)
            gamePanelUIController = FindObjectOfType<GamePanelUIController>();
        Debug.Assert(roleChoosingUiController != null);
        InitAllButtons();
    }

    private void InitAllButtons()
    {
        roleChoosingUiController.InitBackSpritesUI();
        roleChoosingUiController.InitButtons();
        foreach (Button button in roleChoosingUiController.buttons)
        {
            button.onClick.AddListener(delegate { SetRole(button); });
        }

        roleChoosingUiController.confirmButton.onClick.AddListener(delegate
        {
            int gId = Client.Instance.gId;
            int uId = Client.Instance.uId;
            SendConfirmMessage(gId, uId);
            roleChoosingUiController.OnConfirm(gId, uId);
            gamePanelUIController.showIconeAndName(gId, uId, Client.Instance.playerName);
            Debug.Log("确定了");
        });
    }

    private void SendConfirmMessage(int gId, int uId)
    {
        ConfirmChooseMsg ccm = new ConfirmChooseMsg(gId, uId);
//        Debug.Log("chooser uid " + uId);
        networkClient.Send(CustomMsgType.Confirm, ccm);
    }

    private void SetRole(Button selectButton)
    {
        int i;
        for (i = 0; i < 8 && roleChoosingUiController.buttons[i] != selectButton; i++) ;

        if (i == 8)
        {
            Debug.LogError("button not found");
        }

        int newGid = i / 2;
        int newUid = i % 2;
        ChooseRequestMsg chooseRequest = new ChooseRequestMsg(newGid, newUid, Client.Instance.playerName);
        if (networkClient == null) networkClient = Client.Instance.networkClient;
        networkClient.Send(CustomMsgType.Choose, chooseRequest);
        Debug.Log("send " + chooseRequest);

    }

    /**
     * 1.现将所有button置为可用
     * 2.将玩家自己选中的角色置为selected
     * 3.将其他玩家选中的角色置为unvailable
     * 4.将已经被玩家确认的角色置为locked
     * 5.更新角色名字
     */
    public void SetButtonStates(Dictionary<int, string> session2names, Dictionary<int, int> session2roles,
        HashSet<int> session2confirm)
    {
        for (int i = 0; i < 8; i++)
        {
            roleChoosingUiController.SetButtonRoleAvailable(i / 2, i % 2);
        }

        foreach (KeyValuePair<int, int> session2role in session2roles)
        {
            int gid = session2role.Value / 2;
            int uid = session2role.Value % 2;
            if (session2role.Key == Client.Instance.sessionId)
            {
                Client.Instance.gId = gid;
                Client.Instance.uId = uid;
                roleChoosingUiController.SetRoleSelected(Client.Instance.gId, Client.Instance.uId);
                continue;
            }
            roleChoosingUiController.SetButtonRoleUnavailable(gid, uid);
        }

        foreach (int session in session2confirm)
        {
            int roleId = session2roles[session];
            if (session == Client.Instance.sessionId)
            {
                roleChoosingUiController.OnConfirm(roleId/2, roleId%2);
            }
            else
            {
                roleChoosingUiController.SetButtonRoleLocked(roleId / 2, roleId % 2);
            }
        }

        Dictionary<int, string> role2name = new Dictionary<int, string>();
        foreach (int sessionid in session2names.Keys)
        {
            string name = session2names[sessionid];
            int role = session2roles[sessionid];
            role2name.Add(role, name);
        }
        roleChoosingUiController.SetRoleNames(role2name);
    }
    
    
    public void OnReceiveRoleState(NetworkMessage netmsg)
    {
        RoleStateMsg roleStatesMsg = netmsg.ReadMessage<RoleStateMsg>();
        var session2names = roleStatesMsg.GetSessionToName();
        var session2roles = roleStatesMsg.GetSessionToRole();
        var session2confirm = roleStatesMsg.GetSesssion2Confirm();
        SetButtonStates(session2names, session2roles, session2confirm);
        
    }
}