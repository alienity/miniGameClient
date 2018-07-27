using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class JoystickHandler : MonoBehaviour
{

    public JoystickController joystickController;

    //private Queue<JoystickControllMsg> jcmQueue;
    private PanelController panelController;


    public bool enableControl { get; set; }
    // Use this for initialization
    private void Start()
    {
        if (joystickController == null)
            joystickController = FindObjectOfType<JoystickController>();
        if (panelController == null)
            panelController = FindObjectOfType<PanelController>();
        //jcmQueue = new Queue<JoystickControllMsg>();
        
    }

    private void FixedUpdate()
    {
        if (enableControl && Client.Instance.networkClient != null && Client.Instance.networkClient.isConnected)
        {
            SendUpdateJoystickControllMsg();
        }
    }

    public void OnClientReciveMessage(NetworkMessage netmsg)
    {
        PlayerStateMsg msg = netmsg.ReadMessage<PlayerStateMsg>();

        float coolingTime = msg.coolingTime;

        if (coolingTime <= 0)
            joystickController.ChangeSkillState(true);
        else
            joystickController.ChangeSkillState(false);
    }
    
    public void SendUpdateJoystickControllMsg()
    {
        if (Client.Instance.gId == -1 || Client.Instance.uId == -1)
            return;
        
        Joystick joystick = joystickController.joystick;
        Vector2 dir = new Vector2(joystick.Horizontal, joystick.Vertical);
        bool skillUsed = joystickController.skill;
        bool finish = joystickController.finish;
        joystickController.skill = false;

        //Debug.Log("gId = " + Client.Instance.gId + " , uId = " + Client.Instance.uId);

        JoystickControllMsg jcm = new JoystickControllMsg();
//        Debug.Log("client gid, uid " + Client.Instance.gId + " " + Client.Instance.gId);

        jcm.gId = Client.Instance.gId;
        jcm.uId = Client.Instance.uId;
        jcm.direction = dir;
        jcm.skill = skillUsed;
        jcm.finish = finish;

//        Debug.Log("jotstick send gid, uid " + jcm.gId + " " + jcm.uId);
//        Client.Instance.networkClient.Send(CustomMsgType.GroupControll, jcm);
        // todo 为了能通过编译把GroupControll 改为了 GroupJoystick，合并是请注意修改   --xinnjie
        Client.Instance.networkClient.Send(CustomMsgType.GroupJoystick, jcm);
    }

}
