using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    public static Client Instance { get; private set; }

    // 两个handler
    public RoleChooseHandler roleChooseHandler;
    public JoystickHandler joystickHandler;
    public AdvanceControlHandler advanceControlHandler;
    //public NetConnectMaskController netConnectionController;
    public PanelController panelChanger;
    private ReConnectHandler reconnectHandler;

    // 获取到的gId和uId
    public int gId = -1;
    public int uId = -1;

    // 用于广播的ip地址
    public static string ipv4;
    
    private bool flagConnectServer = false;
    private int channelId;
    public static int portTCP = 5555;
    public static int portBroadCastUDP = 6666;

    public NetworkClient networkClient;

    private ClientNetworkDiscovery ClientDiscovery;


    //sessionid 用来标识一局游戏中的一个角色，每局比赛每个玩家sessionID都不同
    public int sessionId = -1;    // -1 作为空闲的sessionId
    public Stage stage = Stage.ChoosingRoleStage;
    
    
    // 玩家名字
    public string playerName = null;
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        flagConnectServer = false;
        // Todo reconnectHandler生命周期和server相同，并且为了尽量避免对editor做改动，暂时就作为component挂载上server了
        reconnectHandler = gameObject.AddComponent<ReConnectHandler>();
        GetServerIP();
    }

//    private void FixedUpdate()
//    {
//        if(flagConnectServer==false && ipv4 != null)
//        {
//            flagConnectServer = true;
//            StartClient();
//        }
//    }

    private void GetServerIP()  // 非阻塞
    {
        ClientDiscovery = gameObject.AddComponent<ClientNetworkDiscovery>();
        ClientDiscovery.Initialize();
        ClientDiscovery.broadcastPort = portBroadCastUDP;
        ClientDiscovery.showGUI = false;
        ClientDiscovery.StartAsClient();    // 非阻塞
    }

    public void StartClient()
    {
        Debug.Log("StartClient");
        if (networkClient == null)
        {
            networkClient = new NetworkClient();
            networkClient.Connect(ipv4, portTCP); // port server的端口，用于建立链接
//            networkClient.RegisterHandler(MsgType.Connect, OnConnect);
            networkClient.RegisterHandler(MsgType.Disconnect, reconnectHandler.OnDisconnect);
            networkClient.RegisterHandler(CustomMsgType.RoleState, roleChooseHandler.OnReceiveRoleState);
            networkClient.RegisterHandler(CustomMsgType.ClientChange, panelChanger.ChangePanel);
            networkClient.RegisterHandler(CustomMsgType.GroupState, joystickHandler.OnClientReciveMessage);
            networkClient.RegisterHandler(CustomMsgType.AdvanceControl, advanceControlHandler.OnReceiveAdvanceControl);
            networkClient.RegisterHandler(CustomMsgType.Session, reconnectHandler.OnReceiveSession);
            networkClient.RegisterHandler(CustomMsgType.Stage, panelChanger.ChangeStage);
        }
        else
        {
            TryConnectToGameServer();
        }

    }

    private void TryConnectToGameServer()
    {
        if (!networkClient.isConnected)
        {
           networkClient.Connect(ipv4, portTCP);
           Debug.Log("is connecting to game server");
        }
        else
        {
            // Todo 不能连接直接退回开始界面，后面加一个单独的显示错误的界面
            panelChanger.SwitchToStage(Stage.StartStage);
            Debug.LogError("not able to connect to game server");
        }
    }
}
