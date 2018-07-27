using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReConnectHandler: MonoBehaviour
{
    private PanelController panelController;
    private RoleChoosingUIController roleChoosingUiController;
    private int curReconnectTimes;
    public int maxReconnectTimes = 10; // 最大的尝试重连次数
    public float waitTimePerTry = 10; // 每次重连尝试间的间隔时间



    private void Start()
    {
        if (panelController == null)
            panelController = FindObjectOfType<PanelController>();
        if (roleChoosingUiController == null)
            roleChoosingUiController = FindObjectOfType<RoleChoosingUIController>();
    }
    
    public void OnReceiveSession(NetworkMessage netmsg)
    {
        SessionMsg sessionMsg = netmsg.ReadMessage<SessionMsg>();
        /*
         * 判断是否为断线重连，断线重连需要回应 sessionid,这样服务器会知道该用户是已经参与游戏的玩家
         * 当askSesssion为真时，说明这是断线重连，需要回应sessionId
         */
        if (sessionMsg.askSession)
        {
            if (Client.Instance.sessionId == -1)
            {
               // Todo 询问session时如果没有属于自己的sessionID，说明玩家误入游戏房间 
                return;
            }
            
        }
        else 
        {
            /* 记录服务器分配的sessionId
             * askSession为假，不是断线重连，服务器会分配一个sessionId给客户端, 客户端需要记录下这个session
             */
            if (sessionMsg.provideSessionId)
            {
                Debug.Assert(sessionMsg.provideSessionId);
                Client.Instance.sessionId = sessionMsg.sessionId;
                PlayerPrefs.SetInt("cool_sessionId", sessionMsg.sessionId);
            }
            /*
            * 接收来自服务器的恢复信息
            */
            else
            {
                switch ((Stage)sessionMsg.stage)
                {
                        case Stage.ConnectToNetStage:
                            Debug.LogError("准备阶段掉线是不可能的");
                            return;
                        case Stage.ChoosingRoleStage:
                            panelController.SwitchToStage(Stage.ChoosingRoleStage);
                            bool confirmed = sessionMsg.confirmed;
                            Dictionary<int, int> cur_session2role = sessionMsg.GetSession2Role();
                            UpdateUI(cur_session2role, confirmed);
                            break;
                        case Stage.GammingStage:
                            Client.Instance.gId = sessionMsg.gid;
                            Client.Instance.uId = sessionMsg.uid;
                            panelController.SwitchToStage(Stage.GammingStage);
                            break;
                }
            }
        }
    }
    
    public void OnDisconnect(NetworkMessage netmsg)
    {
        Debug.Log("disconnected");
        if (Client.Instance.stage == Stage.GammingStage || Client.Instance.stage == Stage.ChoosingRoleStage)
        {
            // todo 跳转到断线界面，并进行重连
            panelController.SwitchToStage(Stage.OfflineStage);
            StartCoroutine(TryToReConnect());
        }
    }

    private IEnumerator TryToReConnect()
    {
        while (!Client.Instance.networkClient.isConnected && Client.Instance.networkClient.ReconnectToNewHost(Client.ipv4, Client.portTCP) && (curReconnectTimes++ < maxReconnectTimes))
        {
            Debug.Log("reconnecting to game, time: " + curReconnectTimes);
            yield return new WaitForSeconds(waitTimePerTry);      
        }

        if (Client.Instance.networkClient.isConnected)
        {
            Debug.Log("reconnected！");
        }
        else
        {
            Debug.Log("can not reconnected");
        }
    }

    private void UpdateUI(Dictionary<int, int> session2roles, bool confirmed)
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

        if (confirmed)
        {
            roleChoosingUiController.OnConfirm();
        }
    }

    
}