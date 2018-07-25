using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RoleChooseHandler : MonoBehaviour
{
    public RoleChoosingUIController roleChoosingUiController;

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
        roleChoosingUiController.InitButtons();
        foreach (Button button in roleChoosingUiController.buttons)
        {
            button.onClick.AddListener(delegate { SetRole(button); });
        }
        roleChoosingUiController.confirmButton.onClick.AddListener(delegate {
            int gId = Client.Instance.gId;
            int uId = Client.Instance.uId;
            SendConfirmMessage(gId, uId);
            roleChoosingUiController.OnConfirm();
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
        for (i = 0; i < 8 && roleChoosingUiController.buttons[i] != selectButton; i++);

        if (i == 8)
        {
            Debug.LogError("button not found");
        }

        int newGid = i / 2;
        int newUid = i % 2;
        ChooseRequestMsg chooseRequest = new ChooseRequestMsg();
        chooseRequest.gid = newGid;
        chooseRequest.uid = newUid;
        if (roleChoosingUiController.roleSelected)
        {
            chooseRequest.hasOld = true;
            chooseRequest.oldGid = roleChoosingUiController.selectedGid;
            chooseRequest.oldUid = roleChoosingUiController.selectedUid;
        }

        if (networkClient == null) networkClient = Client.Instance.networkClient;
        networkClient.Send(CustomMsgType.Choose, chooseRequest);
    }

    public void OnReceiveRoleState(NetworkMessage netmsg)
    {
        RoleStateMsg roleState = netmsg.ReadMessage<RoleStateMsg>();
        int gid = roleState.gid;
        int uid = roleState.uid;
        if (roleState.available)
        {
            roleChoosingUiController.SetButtonRoleAvailable(gid, uid);
        }
        else
        {
            roleChoosingUiController.SetButtonRoleUnavailable(gid, uid);
        }
    }

    public void OnReceiveChooseResult(NetworkMessage netmsg)
    {
        ChooseResultMsg result = netmsg.ReadMessage<ChooseResultMsg>();
        if (result.succeed)
        {
            roleChoosingUiController.SetRoleSelected(result.gid, result.uid);
            Client.Instance.gId = result.gid;
            Client.Instance.uId = result.uid; 
            if (result.hasOld)
            {
                roleChoosingUiController.SetButtonRoleAvailable(result.oldGid, result.oldUid);
            }
        }
    }
    
}