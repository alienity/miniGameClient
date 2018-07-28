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


    public int sessionId = -1;    // -1 作为空闲的sessionId
    public Stage stage = Stage.ChoosingRoleStage;
    

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
        networkClient = new NetworkClient();

        networkClient.Connect(ipv4, portTCP); // port server的端口，用于建立链接
//        networkClient.RegisterHandler(CustomMsgType.Choose, roleChooseHandler.OnReceiveChooseResult);
        networkClient.RegisterHandler(MsgType.Connect, OnConnect);
        networkClient.RegisterHandler(MsgType.Disconnect, reconnectHandler.OnDisconnect);
        networkClient.RegisterHandler(CustomMsgType.RoleState, roleChooseHandler.OnReceiveRoleState);
        networkClient.RegisterHandler(CustomMsgType.ClientChange, panelChanger.ChangePanel);
        networkClient.RegisterHandler(CustomMsgType.GroupState, joystickHandler.OnClientReciveMessage);
        
        networkClient.RegisterHandler(CustomMsgType.AdvanceControl, advanceControlHandler.OnReceiveAdvanceControl);
        networkClient.RegisterHandler(CustomMsgType.Session, reconnectHandler.OnReceiveSession);
        networkClient.RegisterHandler(CustomMsgType.Stage, panelChanger.ChangeStage);
        
    }

    private void OnConnect(NetworkMessage netmsg)
    {
        if (stage == Stage.ConnectToNetStage)
        {
            panelChanger.SwitchToStage(Stage.ChoosingRoleStage);
        }
    }

    // 
    private void InitAllPanels()
    {

    }

}
