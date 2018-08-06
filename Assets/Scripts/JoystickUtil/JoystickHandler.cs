﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

public class JoystickHandler : MonoBehaviour
{
    private GamePanelUIController gamePanelUIController;
    // 现在不需要JoystickController，使用EasyTouch插件，完成摇杆和技能的操控

    // 摇杆对象
    public ETCJoystick pigJoystick;
    public ETCJoystick penguJoystick;

    public ETCTouchPad eTCTouchPad;
    public Image touchStartImg;
    public Image touchEndImg;

    private Vector2 curOffset = Vector2.zero;
    // 蓄力对象
    public ETCButton pigButton;
    public ETCButton penguButton;

    // 开始蓄力的时间点
    private float chargeStartTime = 0;

    // 网络连接
    private NetworkClient networkClient;

    // 记录指令，再发送到服务器
    private Queue<JoystickMsg> jcmQueue = new Queue<JoystickMsg>();
    private Queue<ChargeSkillMsg> csmQueue = new Queue<ChargeSkillMsg>();
    private Queue<RushSkillMag> rsmQueue = new Queue<RushSkillMag>();

    private float coolingTime;

    public bool enableControl;

    private void Start()
    {
        if (gamePanelUIController == null)
            gamePanelUIController = FindObjectOfType<GamePanelUIController>();
        Debug.Log("JoystickHandler start");
        
        InitPigJoystick();
        InitPenguTouchpad();
        InitPenguJoystick();

    }

    private void InitPenguTouchpad()
    {
        eTCTouchPad.onTouchStart.AddListener(OnPenguChargeStartTouchPad);
        eTCTouchPad.onMoveSpeed.AddListener(OnPenguChargeNowTouchPad);
        eTCTouchPad.onTouchUp.AddListener(OnPenguChargeOverTouchPad);
        touchStartImg.raycastTarget = false;
        touchEndImg.raycastTarget = false;
        touchStartImg.enabled = false;
        touchEndImg.enabled = false;
    }

    private void InitPenguJoystick()
    {
        penguJoystick.onMove.AddListener(OnJoystickMove);
        penguJoystick.onMoveEnd.AddListener(OnJoystickEnd);
        penguButton.onPressed.AddListener(OnPenguChargeNowJoystick);
        penguButton.onUp.AddListener(OnPenguChargeOverJoystick);
    }

    private void InitPigJoystick()
    {
        pigJoystick.onMove.AddListener(OnJoystickMove);
        pigJoystick.onMoveEnd.AddListener(OnJoystickEnd);
        pigButton.onDown.AddListener(OnPigDown);

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

    /**
     * *******************猪的按钮***************************
     */
    /// <summary>
    /// 蓄力是发送开始时间和当前时间到服务器，当服务器首次接收到 chargeStartTime != -1 的数据时开始蓄力，直到接收到
    /// chargeStartTime == -1 的情况，停止计时，发动蓄力的技能，并返回冷却时间到手机端。或者服务器计时到终点结束，
    /// 手机端强行结束蓄力
    /// </summary>
    // 为蓄力按钮注册蓄力事件
    public void OnPigDown()
    {

        int gId = Client.Instance.gId;
        int uId = Client.Instance.uId;
        Debug.Log("猪发动技能");
        RushSkillMag rsm = new RushSkillMag(gId, uId, true);
        rsmQueue.Enqueue(rsm);
    }
    
    /**
     * *******************企鹅的摇杆版本***************************
     */

//     按着蓄力
    public void OnPenguChargeNowJoystick()
    {

        int gId = Client.Instance.gId;
        int uId = Client.Instance.uId;

        if (chargeStartTime == 0)
            chargeStartTime = Time.time;

        ChargeSkillMsg csm = new ChargeSkillMsg(gId, uId, chargeStartTime.ToString(), Time.time, 0);
        csmQueue.Enqueue(csm);
    }
    // 结束蓄力
    public void OnPenguChargeOverJoystick()
    {
        int gId = Client.Instance.gId;
        int uId = Client.Instance.uId;

        Debug.Log("结束蓄力");
        ChargeSkillMsg csm = new ChargeSkillMsg(gId, uId, chargeStartTime.ToString(), Time.time, 1);
        csmQueue.Enqueue(csm);
        chargeStartTime = 0;
    }
    
    
    /**
    * *******************企鹅的触控版本***************************
    */
    
    public void OnPenguChargeStartTouchPad()
    {
        curOffset = Vector2.zero;
        chargeStartTime = Time.time;
        
        // 显示起始按钮
        if (Input.touchSupported)
        {
            Touch touch = Input.GetTouch(0);
            Debug.Log("torch postion: " + touch.position);
            touchStartImg.GetComponent<Transform>().position = touch.position;
            touchEndImg.GetComponent<Transform>().position = touch.position;
        }
        else
        {
            touchStartImg.GetComponent<Transform>().position = Input.mousePosition;
            touchEndImg.GetComponent<Transform>().position = Input.mousePosition;
        }

        touchStartImg.enabled = true;
        touchEndImg.enabled = true;

    }

    
    public void OnPenguChargeNowTouchPad(Vector2 speed)
    {
        curOffset.x += speed.x * Time.fixedDeltaTime;
        curOffset.y += speed.y * Time.fixedDeltaTime;
        JoystickMsg jsm = new JoystickMsg(Client.Instance.gId, Client.Instance.uId, curOffset);
        jcmQueue.Enqueue(jsm);
        
        
        // 显示手指当前位置
        if (Input.touchSupported)
        {
            Touch touch = Input.GetTouch(0);
            Debug.Log("torch postion: " + touch.position);
            touchEndImg.GetComponent<Transform>().position = touch.position;
        }
        else
        {
            touchEndImg.GetComponent<Transform>().position = Input.mousePosition;

        }

    }

    public void OnPenguChargeOverTouchPad()
    {
        ChargeSkillMsg csm = new ChargeSkillMsg(Client.Instance.gId, Client.Instance.uId, chargeStartTime.ToString(), Time.time, 1);
        csmQueue.Enqueue(csm);
        touchStartImg.enabled = false;
        touchEndImg.enabled = false;
    }

    private void PenguTouchPadCharge()
    {
        if (touchStartImg.isActiveAndEnabled)
        {
            ChargeSkillMsg csm = new ChargeSkillMsg(Client.Instance.gId, Client.Instance.uId, chargeStartTime.ToString(), Time.time, 0);
//                Debug.Log("OnCharge " + csm);
            csmQueue.Enqueue(csm);
        }
    }


    private void FixedUpdate()
    {
        if (enableControl && Client.Instance.networkClient != null && Client.Instance.networkClient.isConnected)
        {
            if (Client.Instance.networkClient != null && Client.Instance.networkClient.isConnected)
                networkClient = Client.Instance.networkClient;
        }

        if (enableControl && networkClient != null && networkClient.isConnected)
        {
            PenguTouchPadCharge();
            SendUpdateJoystickAndSkillMsg();
        }
        
    }

    // 到冷却时间，蓄力就结束
    public void OnClientReciveMessage(NetworkMessage netmsg)
    {
        PlayerStateMsg msg = netmsg.ReadMessage<PlayerStateMsg>();

        bool gamePanelAvi = msg.joystickAvailable;
        float coolingTime = msg.coolingTime;
        float totalCoolingTime = msg.totalCoolingTime;

//        if (gamePanelAvi)
//            pigJoystick.activated = true;
//        else
//            pigJoystick.activated = false;

        //if (coolingTime <= 0)
        //    pigButton.activated = true;
        //else
        //    pigButton.activated = false;
        gamePanelUIController.showCoolingTimeAndICone(coolingTime, totalCoolingTime);
        this.coolingTime = coolingTime;

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

    // 通过冷却时间检查能否使用技能
    public bool IsSkillAvi()
    {
        return coolingTime > 0 ? false : true;
    }

}
