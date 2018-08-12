using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;

public class PanelController : MonoBehaviour
{
    public Image blackImage;
    public Button StartPanelButton;
    public Button ToPosterButton;
    public AudioClip NormalButtonAudio;
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
    private ReConnectHandler reConnectHandler;


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
        reConnectHandler = FindObjectOfType<ReConnectHandler>();
        Debug.Assert(joystickHandler != null);
        // SwitchToStage(Stage.StartStage);
        // 修改为按键按下后切换到游戏StartStage
        StartPanelButton.gameObject.SetActive(true);
        StartPanelButton.onClick.AddListener(delegate
        {
            Debug.Log("进入游戏准备界面按键按下");
            ButtonOnChickAudioPlay();
            Sequence seq = DOTween.Sequence();
            seq.Append(blackImage.DOFade(0.8f, 1f));
            StartPanelButton.gameObject.SetActive(false);
            SwitchToStageUI(Stage.StartStage);
        });
        ToPosterButton.onClick.AddListener(delegate
        {
            SwitchToStageUI(Stage.PosterStage);
        });
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
                reConnectHandler.tryingReConnect = false;

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
                Client.Instance.InRoom = false;
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
                ButtonOnChickAudioPlay();
                startPanel.SetActive(true);
                connectingToRoomPanel.SetActive(false);
                break;
            case Stage.Prepare:
                ButtonOnChickAudioPlay();
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
                ButtonOnChickAudioPlay();
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
                ButtonOnChickAudioPlay();
                preparePanel.SetActive(true);
                break;

            case Stage.PosterStage:
                Debug.Log("切换回海报页面");
                ButtonOnChickAudioPlay();
                //startPanel.SetActive(false);
                StartPanelButton.gameObject.SetActive(true);
                Sequence seq = DOTween.Sequence();
                seq.Append(blackImage.DOFade(0f, 1f));
                break;
        }

        Client.Instance.stage = stage;
    }

    private void ButtonOnChickAudioPlay()
    {
        this.gameObject.GetComponent<AudioSource>().clip = NormalButtonAudio;
        this.gameObject.GetComponent<AudioSource>().pitch = 2;
        this.gameObject.GetComponent<AudioSource>().Play();
    }
}