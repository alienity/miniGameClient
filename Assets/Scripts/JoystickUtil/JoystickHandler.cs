﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class JoystickHandler : MonoBehaviour
{

    public JoystickController joystickController;

    //private Queue<JoystickControllMsg> jcmQueue;
    private PanelController panelController;


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
        if (Client.Instance.networkClient.isConnected)
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
        joystickController.skill = false;

        JoystickControllMsg jcm = new JoystickControllMsg();
        jcm.gId = Client.Instance.gId;
        jcm.uId = Client.Instance.uId;
        jcm.direction = dir;
        jcm.skill = skillUsed;

        Client.Instance.networkClient.Send(CustomMsgType.GroupControll, jcm);
    }

}
