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

    // 获取到的gId和uId
    public int gId = -1;
    public int uId = -1;

    // 用于广播的ip地址
    public static string ipv4;
    
    private bool flagConnectServer = false;
    private int channelId;
    private int portTCP = 5555;
    private int portBroadCastUDP = 6666;

    public NetworkClient networkClient;

    private ClientNetworkDiscovery ClientDiscovery;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        flagConnectServer = false;
        GetServerIP();
    }

    private void FixedUpdate()
    {
        if(flagConnectServer==false && ipv4 != null)
        {
            flagConnectServer = true;
            StartClient();
        }
    }

    private void GetServerIP()  // 非阻塞
    {
        ClientDiscovery = gameObject.AddComponent<ClientNetworkDiscovery>();
        ClientDiscovery.Initialize();
        ClientDiscovery.broadcastPort = portBroadCastUDP;
        ClientDiscovery.showGUI = false;
        ClientDiscovery.StartAsClient();    // 非阻塞
    }

    private void StartClient()
    {
        Debug.Log("StartClient");
        networkClient = new NetworkClient();

        networkClient.Connect(ipv4, portTCP); // port server的端口，用于建立链接
        networkClient.RegisterHandler(CustomMsgType.Choose, roleChooseHandler.OnReceiveChooseResult);
        networkClient.RegisterHandler(CustomMsgType.RoleState, roleChooseHandler.OnReceiveRoleState);
        networkClient.RegisterHandler(CustomMsgType.ClientChange, panelChanger.ChangePanel);
        networkClient.RegisterHandler(CustomMsgType.GroupState, joystickHandler.OnClientReciveMessage);
        networkClient.RegisterHandler(CustomMsgType.AdvanceControl, advanceControlHandler.OnReceiveAdvanceControl);
        
        panelChanger.EnableNetConnectionMaskPanel(false);
        //netConnectionController.EnableNetConnectionMaskPanel(false);
    }

    // 
    private void InitAllPanels()
    {

    }

}
