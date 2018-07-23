using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class JoystickHandler : MonoBehaviour
{

    public JoystickController joystickController;

    private Queue<JoystickControllMsg> jcmQueue;
    private System.Action continueSendFunc;

    // Use this for initialization
    void Start()
    {
        if (joystickController == null)
            joystickController = FindObjectOfType<JoystickController>();
        jcmQueue = new Queue<JoystickControllMsg>();
    }

    private void Update()
    {
        SendUpdateJoystickControllMsg();
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
        Joystick joystick = joystickController.joystick;
        Vector2 dir = new Vector2(joystick.Horizontal, joystick.Vertical);
        bool skillUsed = joystickController.skill;
        joystickController.skill = false;

        JoystickControllMsg jcm = new JoystickControllMsg();
        jcm.gId = Client.Instance.gId;
        jcm.uId = Client.Instance.uId;
        jcm.direction = dir;
        jcm.skill = skillUsed;

        jcmQueue.Enqueue(jcm);
    }

}
