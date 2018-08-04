using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChargeSkillMsg : MessageBase
{

    public int gId;
    public int uId;
    public string processId;          // 记录按键的一个过程
    public float chargeCurrentTime;   // 蓄力当前时刻
    public int touchId;               // 0表示charge  1表示finish

    public ChargeSkillMsg(int gId, int uId, string processId, float chargeCurrentTime, int touchId)
    {
        this.gId = gId;
        this.uId = uId;
        this.processId = processId;
        this.chargeCurrentTime = chargeCurrentTime;
        this.touchId = touchId;
    }

    public ChargeSkillMsg() { }

    public override string ToString()
    {
        return string.Format("GId: {0}, UId: {1}, ProcessId: {2}, ChargeCurrentTime: {3}, TouchId: {4}", gId, uId, processId, chargeCurrentTime, touchId);
    }
}
