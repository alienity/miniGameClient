using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    public GameObject choosePanel;
    public GameObject joystickPanel;
    [Header("Joystick")]
    public FixedJoystick joystick;
    [Header("SkillButton")]
    public Button skillButton;
    [Header("PlayerBotton")]
    public List<Button> buttons;
    private bool flagButtonPressed = false;

    [Header("Movement direction")]
    public float LeftJoystickHorizontal;
    public float LeftJoystickVertical;

    [Header("User message")]
    public int commMode;
    public int panelID;
    public int groupID;
    public int userID;
    public int skill;
    public int joystickAvailable = 1;
    public int coolingTime = 0;
    public static string ipv4;
    [Header("test NetworkServer")]
    public RegisterHostMessage msg = new RegisterHostMessage();

    NetworkClient myClient;
    private bool flagConnectServer = false;
    private int channelId;
    private int portTCP = 5555;
    private int portBroadCastUDP = 6666;
    const short MsgTypeMessageRecive = 101;
    const short MsgTypeMessageSend = 100;
    const short MsgTypeChooseRoleSend = 102;

    public ClientNetworkDiscovery ClientDiscovery;
    
    private void Start()
    {
        panelID = 0;
        groupID = 0;
        userID = 0;
        flagConnectServer = false;
        GetServerIP();
        InitButtons();
    }

    private void FixedUpdate()
    {
        if(flagConnectServer==false && ipv4 != null)
        {
            flagConnectServer = true;
            StartClient();
        }
        if (myClient == null) return;
        if(myClient.isConnected == true)
        {
            switch (panelID)
            {
                case 0:             //选人场景 几个场景的显示和隐藏不会写
                    choosePanel.SetActive(true);
                    joystickPanel.SetActive(false);
                    break;
                case 1:             //游戏场景
                    choosePanel.SetActive(false);
                    joystickPanel.SetActive(true);
                    CheckAvai();
                    ClientSendMessage();
                    break;
                case 2:             //结束场景
                    break;
                default:
                    break;
            }
        }
    }

    /************************************************
         * 网络初始化
    ************************************************/
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
        myClient = new NetworkClient();
        myClient.RegisterHandler(MsgType.Connect, OnClientConnected);
        myClient.RegisterHandler(MsgTypeMessageRecive, ClientReciveMessage);
        myClient.RegisterHandler(MsgTypeMessageRecive, ClientReciveMessage);
        //myClient.Connect("10.29.89.84", 5555);
        myClient.Connect(ipv4, portTCP);        // port server的端口，用于建立链接
    }

    /************************************************
         * 收包
    ************************************************/
    private void OnClientConnected(NetworkMessage netmsg)
    {
        Debug.Log("Client connected to server");
        channelId = netmsg.channelId;
    }
    
    private void ClientReciveMessage(NetworkMessage netmsg)
    {
        //Debug.Log("ClientReciveMessage");
        //JObject json = new JObject
        //    {
        //        { "commMode", 1},                               // 新增通讯模式字段，0表示单播，1表示组播
        //        { "stageId", panelID},                           // 正在哪个panel
        //        { "gId", groupID},                              // 组ID： 0~3表示四个组
        //        { "uId", userID},                               // 用户ID：0或1， 0表示大将，1表示 马
        //        { "joystickAvailable", joystickAvailable},      // 当前摇杆是否可以操控，0表示不行，1表示可以
        //        { "coolingTime", coolingTime}                   //剩余冷却时间
        //    };
        msg = netmsg.ReadMessage<RegisterHostMessage>();
        Debug.Log(msg.m_Comment);
        JObject ServerMessage = JObject.Parse(msg.m_Comment);
        commMode = (int)ServerMessage["commMode"];
        panelID = (int)ServerMessage["stageId"];        // 场景ID 场景状态机由此切换 
        joystickAvailable = (int)ServerMessage["joystickAvailable"];
        coolingTime = (int)ServerMessage["coolingTime"];

        int temp_gId = (int)ServerMessage["gId"];
        int temp_uId = (int)ServerMessage["uId"];
        if (commMode == 1 && panelID == 0)               // 场景0时的组播为选人确认组播
        {
            // 此处将按键对应失能
            //buttons[temp_gId * 2 + temp_uId].interactable = false;
            if(temp_gId==groupID && temp_uId==userID && flagButtonPressed==true)
            {
                foreach (Button button in buttons)
                {
                    button.interactable = false;
                }
            }
            else
            {
                buttons[temp_gId * 2 + temp_uId].interactable = false;
            }
            
        }
    }

    /************************************************
         * 发包
    ************************************************/
    private void ClientSendMessage()
    {
        JObject json = new JObject
        {
            {"stageId", panelID},   //遥控第二个界面，操纵杆界面
            { "gId", groupID},      //组ID： 0~3
            { "uId", userID},       //用户ID：0~1 
            {
                "direction", new JObject  // 操纵杆坐标
                {
                    { "x", joystick.Horizontal },
                    { "y", joystick.Vertical }
                }
            },
            { "skill", skill}        //技能释放，0表示不使用技能
        };
        string output = json.ToString();//JsonConvert.SerializeObject(json);
        //Debug.Log(output);

        NetworkWriter writer = new NetworkWriter();
        writer.StartMessage(MsgTypeMessageSend);
        writer.Write(output);
        writer.FinishMessage();
        if (myClient.isConnected)
            myClient.SendWriter(writer, channelId);
    }
    private void ClientSendSetRoleMessage()
    {
        JObject json = new JObject
        {
            {"stageId", panelID},   //遥控第二个界面，操纵杆界面
            { "gId", groupID},      //组ID： 0~3
            { "uId", userID},       //用户ID：0~1 
            {
                "direction", new JObject  // 操纵杆坐标
                {
                    { "x", joystick.Horizontal },
                    { "y", joystick.Vertical }
                }
            },
            { "skill", skill}        //技能释放，0表示不使用技能
        };
        string output = json.ToString();//JsonConvert.SerializeObject(json);
        //Debug.Log(output);

        NetworkWriter writer = new NetworkWriter();
        writer.StartMessage(MsgTypeChooseRoleSend);
        writer.Write(output);
        writer.FinishMessage();
        if (myClient.isConnected)
            myClient.SendWriter(writer, channelId);
    }
    /************************************************
         * 按键
    ************************************************/
    private void InitButtons()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(delegate { SetRole(button); });
        }
    }
    void SetRole(Button selectButton)
    {
        int i;
        for (i = 0; i < 8 && buttons[i] != selectButton; i++) { }

        if (i == 8)
        {
            Debug.LogError("button not found");
            return;
        }
        Debug.Log("Button" + i + "trigger");
        flagButtonPressed = true;
        groupID = i / 2;
        userID = i % 2;

        //ClientSendMessage();
        ClientSendSetRoleMessage();
    }

    public void SpawnSkill()
    {
        Debug.Log("发动技能");
        skill = 1;
        skillButton.interactable = false;
    }

    public void CheckAvai()
    {
        coolingTime = 10;
        if (coolingTime <= 0)
        {
            skillButton.interactable = true;
        }
    }

}
