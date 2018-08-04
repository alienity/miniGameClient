using UnityEngine;
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
    public ETCJoystick eTCJoystick;

    public ETCTouchPad eTCTouchPad;
    public Image touchStartImg;
    public Image touchEndImg;

    private Vector2 curOffset = Vector2.zero;
    // 蓄力对象
    public ETCButton eTCChargeButton;

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
        
        eTCChargeButton.onDown.AddListener(OnDown);
        
        eTCTouchPad.onTouchStart.AddListener(OnChargeStartTouchPad);
        eTCTouchPad.onMoveSpeed.AddListener(OnChargeNowTouchPad);
        eTCTouchPad.onTouchUp.AddListener(OnChargeOverTouchPad);
        touchStartImg.raycastTarget = false;
        touchEndImg.raycastTarget = false;
        touchStartImg.enabled = false;
        touchEndImg.enabled = false;

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
    public void OnDown()
    {

        int gId = Client.Instance.gId;
        int uId = Client.Instance.uId;
        Debug.Log("猪发动技能");
        RushSkillMag rsm = new RushSkillMag(gId, uId, true);
        rsmQueue.Enqueue(rsm);
    }

    // 按着蓄力
//    public void OnChargeNow()
//    {
//        //if (!IsSkillAvi()) return;
//
//        int gId = Client.Instance.gId;
//        int uId = Client.Instance.uId;
//
//        if (uId == 1) return;
//
//        if (chargeStartTime == 0)
//            chargeStartTime = Time.time;
//
//        ChargeSkillMsg csm = new ChargeSkillMsg(gId, uId, chargeStartTime.ToString(), Time.time, 0);
//        csmQueue.Enqueue(csm);
//    }

    public void OnChargeStartTouchPad()
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

    
    public void OnChargeNowTouchPad(Vector2 speed)
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

    public void OnChargeOverTouchPad()
    {
        ChargeSkillMsg csm = new ChargeSkillMsg(Client.Instance.gId, Client.Instance.uId, chargeStartTime.ToString(), Time.time, 1);
        //new ChargeSkillMsg(gId, uId, chargeStartTime, 0, true);
        Debug.Log("OnChargeOverTouchPad 结束蓄力: " + csm);

        csmQueue.Enqueue(csm);
        touchStartImg.enabled = false;
        touchEndImg.enabled = false;
    }

//    // 结束蓄力
//    public void OnChargeOver()
//    {
//        //if (!IsSkillAvi()) return;
//
//        int gId = Client.Instance.gId;
//        int uId = Client.Instance.uId;
//
//        if (uId == 1) return;
//        
//        Debug.Log("结束蓄力");
//        ChargeSkillMsg csm = new ChargeSkillMsg(gId, uId, chargeStartTime.ToString(), Time.time, 1);
//        //new ChargeSkillMsg(gId, uId, chargeStartTime, 0, true);
//        csmQueue.Enqueue(csm);
//
//        chargeStartTime = 0;
//    }

    private void FixedUpdate()
    {
        if (enableControl && Client.Instance.networkClient != null && Client.Instance.networkClient.isConnected)
        {
            if (Client.Instance.networkClient != null && Client.Instance.networkClient.isConnected)
                networkClient = Client.Instance.networkClient;
        }

        if (enableControl && networkClient != null && networkClient.isConnected)
        {
            if (touchStartImg.isActiveAndEnabled)
            {
                ChargeSkillMsg csm = new ChargeSkillMsg(Client.Instance.gId, Client.Instance.uId, chargeStartTime.ToString(), Time.time, 0);
                Debug.Log("OnCharge " + csm);
                csmQueue.Enqueue(csm);
            }
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
//            eTCJoystick.activated = true;
//        else
//            eTCJoystick.activated = false;

        //if (coolingTime <= 0)
        //    eTCChargeButton.activated = true;
        //else
        //    eTCChargeButton.activated = false;
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
