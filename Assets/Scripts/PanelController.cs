using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{

    public GameObject preparePanel;
    public GameObject startPanel;
    public GameObject roomCanvas;
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    public GameObject connectingToNet;
    public GameObject reconnectPanel;
    public Text reconnectErrorText;

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
        panels.Add(reconnectPanel);
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

    public void ChangeStage(NetworkMessage netmsg)
    {
        StageTransferMsg stageTransferMsg = netmsg.ReadMessage<StageTransferMsg>();
        SwitchToStage((Stage)stageTransferMsg.stage);
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
                case Stage.StartStage:
                    Client.Instance.networkClient.Disconnect();
                    startPanel.SetActive(true);
                    break;
                case Stage.ConnectToNetStage:
                    connectingToNet.SetActive(true);
                    Client.Instance.StartClient();
                    break;
                case Stage.ChoosingRoleStage:
                    roomCanvas.SetActive(true);
                    break;
                case Stage.GammingStage:
                    joystickHandler.enableControl = true;
                    gamePanel.SetActive(true);
                    break;
                case Stage.OfflineStage:
                    reconnectPanel.SetActive(true);
                    break;
                case Stage.GameOverStage:
                    // 游戏结束时断开时断开与服务器的连接, 删除sessionid
                    PlayerPrefs.DeleteKey(ReConnectHandler.SESSION_NAME);
                    Client.Instance.sessionId = -1;
                    gameOverPanel.SetActive(true);
                    Client.Instance.networkClient.Disconnect();
                    break;
        }
    }
}
