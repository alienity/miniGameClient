using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RoleChooseHandler : MonoBehaviour
{
    private RoleChoosingUIController roleChoosingUiController;

    private NetworkClient networkClient;

    private void Start()
    {
        if (roleChoosingUiController == null)
            roleChoosingUiController = FindObjectOfType<RoleChoosingUIController>();
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
        ChooseRequestMsg chooseRequest = new ChooseRequestMsg();
        chooseRequest.gid = newGid;
        chooseRequest.uid = newUid;
        if (networkClient == null) networkClient = Client.Instance.networkClient;
        networkClient.Send(CustomMsgType.Choose, chooseRequest);
        Debug.Log("send " + chooseRequest);

    }

    public void OnReceiveRoleState(NetworkMessage netmsg)
    {
        RoleStateMsg roleStatesMsg = netmsg.ReadMessage<RoleStateMsg>();
        Debug.Log("received role info " + roleStatesMsg);
        for (int i = 0; i < 8; i++)
        {
            roleChoosingUiController.SetButtonRoleAvailable(i / 2, i % 2);
        }

        foreach (KeyValuePair<int, int> session2role in roleStatesMsg.GetSessionToRole())
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

    }
}