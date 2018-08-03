using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{

    public GameObject preparePanel;
    public GameObject startPanel;
    public GameObject producerListPanel;
    public GameObject roomCanvas;
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    public GameObject connectingToNet;
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
        panels.Add(gamePanel);
        panels.Add(gameOverPanel);
        panels.Add(connectingToNet);
        panels.Add(reconnectPanel);
    }

    private void Start()
    {
        
        joystickHandler = FindObjectOfType<JoystickHandler>();
        roleChoosingUIController = FindObjectOfType<RoleChoosingUIController>();
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
        // Todo 还没有找到服务器，就不能进入游戏   Client.Instance.networkClient.isConnected的判断是游戏结束后重新x
        if (stage == Stage.Prepare && Client.ipv4 == null && Client.Instance.networkClient.isConnected)
        {
            return;
        }
        foreach (GameObject pgo in panels)
        {
            pgo.SetActive(false);
        }
        Debug.Log("panels: " + panels.Count);
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
            roleChoosingUIController.ResetUI();
        }

        switch (stage)
        {
            case Stage.StartStage:
                if (Client.Instance.networkClient != null && Client.Instance.networkClient.isConnected)
                {
                    Client.Instance.networkClient.Disconnect();
                }
                startPanel.SetActive(true);
                break;
            case Stage.Prepare:
                connectingToNet.SetActive(true);
                Client.Instance.StartClient();
                break;
            case Stage.ProducerListStage:
                startPanel.SetActive(false);
                producerListPanel.SetActive(true);
                break;
            case Stage.ChoosingRoleStage:
                FindObjectOfType<RoleChoosingUIController>().ResetUI();
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
                Debug.Log("deleted session");
                break;
            case Stage.ChangeNameStage:
                preparePanel.SetActive(true);
                break;
        }
        Client.Instance.stage = stage;
    }
    
}
