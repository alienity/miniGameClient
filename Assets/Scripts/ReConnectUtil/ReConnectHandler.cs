﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ReConnectHandler: MonoBehaviour
{
    private PanelController panelController;
    private RoleChoosingUIController roleChoosingUiController;
    private RoleChooseHandler roleChooseHandler;

    public int maxReconnectTimes = 10; // 最大的尝试重连次数
    public float waitTimePerTry = 10; // 每次重连尝试间的间隔时间

    // 在PlayerPref中存的sessionID，每个sessionID表示特定游戏场次中的特定玩家
    static public string SESSION_NAME = "cool_sessionId";

    public bool tryingReConnect;
    private void Start()
    {    
        if (panelController == null)
            panelController = FindObjectOfType<PanelController>();
        if (roleChoosingUiController == null)
            roleChoosingUiController = FindObjectOfType<RoleChoosingUIController>();
        if (roleChooseHandler == null)
            roleChooseHandler = FindObjectOfType<RoleChooseHandler>();
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
               // 试图读取sessionId
                Client.Instance.sessionId = PlayerPrefs.GetInt(SESSION_NAME, -1);
                if (Client.Instance.sessionId == -1)
                {
                    // Todo 询问session时如果没有属于自己的sessionID，说明玩家误入游戏房间, 要怎么告诉玩家当前不能游戏，maybe跳到一个提示界面，告玩家当前游戏不可用
                    Debug.Log("该玩家没有sessionID，不能进行断线重连");
                    panelController.SwitchToStage(Stage.StartStage);
                    return;
                }
            }
            SessionMsg responSessionMsg = new SessionMsg(false, true, Client.Instance.sessionId, 0,Client.Instance.stage, false, 0, 0, null, null, null);
            Client.Instance.networkClient.Send(CustomMsgType.Session, responSessionMsg);
            Debug.Log("reply with sessionId " + responSessionMsg);

        }
        else 
        {
            /*
             * 
             * 服务器分配sessionId时的逻辑
             * askSession为假，不是断线重连，服务器会分配一个sessionId给客户端, 客户端需要记录下这个session
             */
            if (sessionMsg.provideSessionInfo)
            {
                /*
                 * 断线重连会发生这样一种状况：客户端在游戏时断线，但是并不连接，在下一次服务器游戏开始后，客户端进行了重连。这是客户端实际需要退回到开始界面，不进入房间
                 */
                if (Client.Instance.InRoom)
                {
                    Debug.Log("进入不该进的房间curRoomId: " + Client.Instance.curRoomId + "sessionInfo :" + sessionMsg);
                    Client.Instance.InRoom = false;
                    panelController.SwitchToStage(Stage.StartStage);
                }
                else
                {
                    Debug.Log("received session " + sessionMsg);
                    Client.Instance.sessionId = sessionMsg.sessionId;
                    Client.Instance.InRoom = true;
                    Client.Instance.curRoomId = sessionMsg.roomId;
                    PlayerPrefs.SetInt(SESSION_NAME, sessionMsg.sessionId);
                    panelController.SwitchToStage(Stage.ConnectedToChooseRoomStage);
                }

            }
            /*
            * 接收来自服务器的恢复信息，这里才是真正断线重连的逻辑
            */
            else
            {
                switch ((Stage)sessionMsg.stage)
                {
                        case Stage.Prepare:
                            // Todo 需要一个单独的界面显示错误信息
                            Debug.LogError("不应该在准备时掉线");
                            panelController.SwitchToStageUI(Stage.Prepare);
                            return;
                        case Stage.ChoosingRoleStage:
                            panelController.SwitchToStageUI(Stage.ChoosingRoleStage);
                            var cur_session2role = sessionMsg.GetSession2Role();
                            var cur_confirmed = sessionMsg.GetSession2Confirm();
                            var cur_session2name = sessionMsg.GetSessionToName();
                            roleChooseHandler.SetButtonStates(cur_session2name, cur_session2role, cur_confirmed);
                            Debug.Log("received roll to choosing " + sessionMsg);
                            break;
                        case Stage.GammingStage:
                            Client.Instance.gId = sessionMsg.gid;
                            Client.Instance.uId = sessionMsg.uid;
                            panelController.SwitchToStageUI(Stage.GammingStage);
                            Debug.Log("received roll to gamming " + sessionMsg);
                            break;
                }
            }
        }
    }
    
    public void OnDisconnect(NetworkMessage netmsg)
    {
        Debug.Log("disconnected");
        if (Client.Instance.stage == Stage.GammingStage || Client.Instance.stage == Stage.ChoosingRoleStage || Client.Instance.stage == Stage.Prepare || Client.Instance.stage == Stage.ConnectedToChooseRoomStage)
        {
            panelController.SwitchToStage(Stage.OfflineStage);
            StartCoroutine(TryToReConnect());
        }
    }

    public void OnConnect(NetworkMessage netmsg)
    {
        if (tryingReConnect)
        {
            Debug.Log("reconnected！");
        }
        tryingReConnect = false;
    }

    private IEnumerator TryToReConnect()
    {
        int curReconnectTimes = 0;
        tryingReConnect = true;
        Text errorText = panelController.reconnectErrorText;
        while (tryingReConnect && !Client.Instance.networkClient.isConnected && (curReconnectTimes++ < maxReconnectTimes))
        {
            Client.Instance.networkClient.Connect(Client.ipv4, Client.portTCP); 
            string errorMsg = "重连第" + curReconnectTimes  +"次";
            Debug.Log(errorMsg);
            errorText.text = errorMsg;
            yield return new WaitForSeconds(waitTimePerTry);
        }
        tryingReConnect = false;
    }

  
}