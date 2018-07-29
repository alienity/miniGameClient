﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class JoystickHandler : MonoBehaviour
{

    // 现在不需要JoystickController，使用EasyTouch插件，完成摇杆和技能的操控

    // 摇杆对象
    public ETCJoystick eTCJoystick;
    // 蓄力对象
    public ETCButton eTCChargeButton;

    // 开始蓄力的时间点
    private float chargeStartTime = -1;

    // 网络连接
    private NetworkClient networkClient;

    // 记录指令，再发送到服务器
    private Queue<JoystickMsg> jcmQueue = new Queue<JoystickMsg>();
    private Queue<ChargeSkillMsg> csmQueue = new Queue<ChargeSkillMsg>();
    private Queue<RushSkillMag> rsmQueue = new Queue<RushSkillMag>();

    public bool enableControl { get; set; }

    private void Start()
    {
        Debug.Log("JoystickHandler start");
    }

    /// <summary>
    /// 为摇杆移动注册事件
    /// </summary>
    /// <param name="move"></param>
    public void OnJoystickMove(Vector2 move)
    {
        int gId = Client.Instance.gId;
        int uId = Client.Instance.uId;
        Vector2 dir = move.normalized;
        JoystickMsg jcm = new JoystickMsg(gId, uId, dir);
        jcmQueue.Enqueue(jcm);
    }

    public void OnJoystickEnd()
    {
        int gId = Client.Instance.gId;
        int uId = Client.Instance.uId;
        JoystickMsg jcm = new JoystickMsg(gId, uId, Vector2.zero);
        jcmQueue.Enqueue(jcm);
    }

    /// <summary>
    /// 蓄力是发送开始时间和当前时间到服务器，当服务器首次接收到 chargeStartTime != -1 的数据时开始蓄力，直到接收到
    /// chargeStartTime == -1 的情况，停止计时，发动蓄力的技能，并返回冷却时间到手机端。或者服务器计时到终点结束，
    /// 手机端强行结束蓄力
    /// </summary>
    // 为蓄力按钮注册蓄力事件
    public void OnChargeStart()
    {
        int gId = Client.Instance.gId;
        int uId = Client.Instance.uId;
        if(uId == 0)
        {
            Debug.Log("开始蓄力");
            chargeStartTime = Time.time;
        }
        else
        {
            Debug.Log("猪发动技能");
            RushSkillMag rsm = new RushSkillMag(gId, uId, true);
            rsmQueue.Enqueue(rsm);
        }
    }

    // 按着蓄力
    public void OnChargeNow()
    {
        int gId = Client.Instance.gId;
        int uId = Client.Instance.uId;

        if (uId == 1) return;

        ChargeSkillMsg csm = new ChargeSkillMsg(gId, uId, chargeStartTime, Time.time, false);
        csmQueue.Enqueue(csm);
    }

    // 结束蓄力
    public void OnChargeOver()
    {
        int gId = Client.Instance.gId;
        int uId = Client.Instance.uId;

        if (uId == 1) return;

        Debug.Log("结束蓄力");
        ChargeSkillMsg csm = new ChargeSkillMsg(gId, uId, chargeStartTime, Time.time, true);
        csmQueue.Enqueue(csm);
    }

    private void FixedUpdate()
    {
        if (enableControl && Client.Instance.networkClient != null && Client.Instance.networkClient.isConnected)
        {
            if (Client.Instance.networkClient != null && Client.Instance.networkClient.isConnected)
                networkClient = Client.Instance.networkClient;
        }

        if (networkClient != null && networkClient.isConnected)
        {
            SendUpdateJoystickAndSkillMsg();
        }
        
    }

    // 到冷却时间，蓄力就结束
    public void OnClientReciveMessage(NetworkMessage netmsg)
    {
        PlayerStateMsg msg = netmsg.ReadMessage<PlayerStateMsg>();

        bool gamePanelAvi = msg.joystickAvailable;
        float coolingTime = msg.coolingTime;

        if (gamePanelAvi)
            eTCJoystick.activated = true;
        else
            eTCJoystick.activated = false;

        if (coolingTime <= 0)
            eTCChargeButton.activated = true;
        else
            eTCChargeButton.activated = false;
    }
    
    // 发送摇杆数据
    public void SendUpdateJoystickAndSkillMsg()
    {
        if (Client.Instance.gId == -1 || Client.Instance.uId == -1)
            return;

        foreach (JoystickMsg jcm in jcmQueue)
            networkClient.Send(CustomMsgType.GroupJoystick, jcm);
        foreach (ChargeSkillMsg csm in csmQueue)
            networkClient.Send(CustomMsgType.GroupChargeSkill, csm);
        foreach (RushSkillMag rsm in rsmQueue)
            networkClient.Send(CustomMsgType.GroupRushSkill, rsm);

        jcmQueue.Clear();
        csmQueue.Clear();
        rsmQueue.Clear();
    }

}
