using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.UI;


public class RoleChooseHandler : MonoBehaviour
{
    public RoleChoosingController roleChoosingController;

    private NetworkClient networkClient;

    private void Start()
    {
        if (roleChoosingController == null)
            roleChoosingController = FindObjectOfType<RoleChoosingController>();


        InitAllButtons();
    }

    private void InitAllButtons()
    {
        roleChoosingController.InitButtons();
        foreach (Button button in roleChoosingController.buttons)
        {
            button.onClick.AddListener(delegate { SetRole(button); });
        }
        roleChoosingController.confirmButton.onClick.AddListener(delegate {
            int gId = Client.Instance.gId;
            int uId = Client.Instance.uId;
            ConfirmChoose(gId, uId);

            foreach (Button button in roleChoosingController.buttons)
                button.interactable = false;

            Debug.Log("确定了");

            roleChoosingController.OnConfirm();
        });
    }
    
    private void ConfirmChoose(int gId, int uId)
    {
        ConfirmChooseMsg ccm = new ConfirmChooseMsg(gId, uId);

        networkClient.Send(CustomMsgType.Confirm, ccm);
    }

    private void SetRole(Button selectButton)
    {
        int i;
        for (i = 0; i < 8 && roleChoosingController.buttons[i] != selectButton; i++)
        {

        }

        if (i == 8)
        {
            Debug.LogError("button not found");
        }

        int newGid = i / 2;
        int newUid = i % 2;
        ChooseRequestMsg chooseRequest = new ChooseRequestMsg();
        chooseRequest.gid = newGid;
        chooseRequest.uid = newUid;
        if (roleChoosingController.roleSelected)
        {
            chooseRequest.hasOld = true;
            chooseRequest.oldGid = Client.Instance.gId;//roleChoosingController.selectedGid;
            chooseRequest.oldUid = Client.Instance.uId;//roleChoosingController.selectedUid;
        }

        if (networkClient == null) networkClient = Client.Instance.networkClient;
        networkClient.Send(CustomMsgType.Choose, chooseRequest);
    }

    public void OnReceiveRoleState(NetworkMessage netmsg)
    {
        RoleStateMsg roleState = netmsg.ReadMessage<RoleStateMsg>();
        int gid = roleState.gid;
        int uid = roleState.uid;

        Debug.Log("gId = " + gid + " , uId = " + uid);

        if (roleState.available)
        {
            roleChoosingController.SetButtonRoleAvailable(gid, uid);
        }
        else
        {
            roleChoosingController.SetButtonRoleUnavailable(gid, uid);
        }
    }

    public void OnReceiveChooseResult(NetworkMessage netmsg)
    {
        ChooseResultMsg result = netmsg.ReadMessage<ChooseResultMsg>();
        Debug.Log("recerived " + result);
        if (result.succeed)
        {
            roleChoosingController.SetRoleSelected(result.gid, result.uid);
            Client.Instance.gId = result.gid;
            Client.Instance.uId = result.uid;
            if (result.hasOld)
            {
                roleChoosingController.SetButtonRoleAvailable(result.oldGid, result.oldUid);
            }
        }
    }
    
}