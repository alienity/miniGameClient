﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public GameObject preparePanel;
    public GameObject startPanel;
    public GameObject connectingToRoomPanel;
    public GameObject producerListPanel;
    public GameObject roomCanvas;
    public GameObject pigJoyStickPanel;
    public GameObject penguTouchPadPanel;
    public GameObject penguJoystickPanel;
    public GameObject gameOverPanel;
    public GameObject waitingOtherPlayerPanel;
    public GameObject reconnectPanel;
    public Text reconnectErrorText;

    public NetworkClient netClient;

    private List<GameObject> panels = new List<GameObject>();
    private JoystickHandler joystickHandler;
    private RoleChoosingUIController roleChoosingUIController;


    private void Awake()
    {
        panels.Add(preparePanel);
        panels.Add(startPanel);
        panels.Add(producerListPanel);
        panels.Add(roomCanvas);
        panels.Add(pigJoyStickPanel);
        panels.Add(penguTouchPadPanel);
        panels.Add(penguJoystickPanel);
        panels.Add(gameOverPanel);
        panels.Add(waitingOtherPlayerPanel);
        panels.Add(reconnectPanel);
        // **注意**connectingToRoomPanel 在 startPanel 中
        panels.Add(connectingToRoomPanel);
    }

    private void Start()
    {
        joystickHandler = FindObjectOfType<JoystickHandler>();
        roleChoosingUIController = FindObjectOfType<RoleChoosingUIController>();
        Debug.Assert(joystickHandler != null);
        SwitchToStage(Stage.StartStage);
    }

    // 手动切换界面
    public void ChangePanelByHand()
    {
    }

    public void ChangePanel(NetworkMessage netmsg)
    {
        Debug.Log("改变界面");
        SceneTransferMsg stm = netmsg.ReadMessage<SceneTransferMsg>();
        string nextSceneName = stm.nextSceneName;
        Debug.Log(nextSceneName);
        // 暂时先设置成只从要转换的下一个场景读
        if (nextSceneName == "GameScene")
        {
            // 切换到游戏场景后**才**开始发送摇杆信息
            SwitchToStageUI(Stage.GammingStage);
        }
    }

    public void ChangeStage(NetworkMessage netmsg)
    {
        StageTransferMsg stageTransferMsg = netmsg.ReadMessage<StageTransferMsg>();
        SwitchToStage((Stage) stageTransferMsg.stage);
    }

    public void SwitchToStage(Stage stage)
    {
        Debug.Log("switch stage: " + stage);
        switch (stage)
        {
            case Stage.StartStage:
                if (Client.Instance.networkClient != null && Client.Instance.networkClient.isConnected)
                {
                    Client.Instance.networkClient.Disconnect();
                }

                break;
            case Stage.Prepare:
                Client.Instance.StartClient();
                break;
            case Stage.ConnectedToChooseRoomStage:
                break;
            case Stage.ChoosingRoleStage:
                break;
            case Stage.GammingStage:
                break;
            case Stage.OfflineStage:
                break;
            case Stage.GameOverStage:
                // 游戏结束时断开时断开与服务器的连接, 删除sessionid
                PlayerPrefs.DeleteKey(ReConnectHandler.SESSION_NAME);
                Client.Instance.sessionId = -1;
                Client.Instance.networkClient.Disconnect();
                Debug.Log("deleted session");
                break;
            case Stage.ChangeNameStage:
                preparePanel.SetActive(true);
                break;
        }

        SwitchToStageUI(stage);
    }

    /*
     * SwitchToStageUI 只负责UI的变化
     */
    public void SwitchToStageUI(Stage stage)
    {
//        // Todo 还没有找到服务器，就不能进入游戏   Client.Instance.networkClient.isConnected的判断是游戏结束后重新
//        if (stage == Stage.Prepare && Client.ipv4 == null && Client.Instance.networkClient.isConnected)
//        {
//            return;
//        }

        foreach (GameObject pgo in panels)
        {
            pgo.SetActive(false);
        }

        if (joystickHandler != null)
        {
            joystickHandler.enableControl = false;
        }
        else
        {
            Debug.Log("joystickHandler null");
        }

        if (roleChoosingUIController != null)
        {
            roleChoosingUIController.ResetChooseRoleUI();
        }

        switch (stage)
        {
            case Stage.StartStage:
                startPanel.SetActive(true);
                connectingToRoomPanel.SetActive(false);
                break;
            case Stage.Prepare:
                startPanel.SetActive(true);
                connectingToRoomPanel.SetActive(true);
                break;
            case Stage.ConnectedToChooseRoomStage:
                waitingOtherPlayerPanel.SetActive(true);
                break;
            case Stage.ChoosingRoleStage:
                FindObjectOfType<RoleChoosingUIController>().ResetChooseRoleUI();
                roomCanvas.SetActive(true);
                break;
            case Stage.GammingStage:
                joystickHandler.enableControl = true;
                if (Client.Instance.uId == 0)
                {
                    penguTouchPadPanel.SetActive(true);
                    Debug.Log("switch to penguin panel");
                }
                else
                {
                    pigJoyStickPanel.SetActive(true);
                    Debug.Log("switch to pig panel"    );
                }
                break;
            case Stage.ProducerListStage:
                producerListPanel.SetActive(true);
                break;
            case Stage.OfflineStage:
                reconnectPanel.SetActive(true);
                break;
            case Stage.GameOverStage:
                // 游戏结束时断开时断开与服务器的连接, 删除sessionid
                gameOverPanel.SetActive(true);
                break;
            case Stage.ChangeNameStage:
                preparePanel.SetActive(true);
                break;
        }

        Client.Instance.stage = stage;
    }
}