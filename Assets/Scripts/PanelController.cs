﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PanelController : MonoBehaviour
{

    public GameObject preparePanel;
    public GameObject startPanel;
    public GameObject roomCanvas;
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    public GameObject connectingToNet;

    public NetworkClient netClient;

    private List<GameObject> panels = new List<GameObject>();
    private JoystickHandler joystickHandler;

    private void Start()
    {
        panels.Add(preparePanel);
        panels.Add(startPanel);
        panels.Add(roomCanvas);
        panels.Add(gamePanel);
        panels.Add(gameOverPanel);
        panels.Add(connectingToNet);
        joystickHandler = FindObjectOfType<JoystickHandler>();
        Debug.Assert(joystickHandler != null);
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
        if(nextSceneName == "GameScene")
        {
            // 切换到游戏场景后**才**开始发送摇杆信息
            SwitchToStage(Stage.GammingStage);
            gamePanel.SetActive(true);

        }
    }

    public void SwitchToStage(Stage stage)
    {
        Client.Instance.stage = stage;
        foreach (GameObject pgo in panels)
        {
            pgo.SetActive(false);
        }

        joystickHandler.enableControl = false;
        switch (stage)
        {
                case Stage.Prepare:
                    preparePanel.SetActive(true);
                    break;
                case Stage.ChoosingRoleStage:
                    roomCanvas.SetActive(true);
                    break;
                case Stage.GammingStage:
                    joystickHandler.enableControl = true;
                    gamePanel.SetActive(true);
                    break;
        }
    }

    public void EnableNetConnectionMaskPanel(bool enable)
    {
        connectingToNet.SetActive(enable);
    }

}
