using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AdvanceControlHandler: MonoBehaviour
{
    /*
     * 在这里写回调，然后在 Client.StartClient 把回调注册上去
     * 可以自己定义消息体来用来分辨后面多加的功能，比如震动、声音什么
     * AdvanceControl 那个消息号预留给你了，需要的话自己再定义新的消息号
     * 要注册几个回调函数，几个消息号按照自己的需求和习惯，注意和miniGameServer的消息号数量保持一致
     */
    public void OnReceiveAdvanceControl(NetworkMessage netmsg)
    {
//#if UNITY_ANDROID todo handhled.viberate 根据文档是跨平台的
        AdvanceControlMsg advanceControlMsg = netmsg.ReadMessage<AdvanceControlMsg>();
        switch (advanceControlMsg.type)
        {
                case AdvanceControlType.Viberate:
                    StartCoroutine(Viberate(advanceControlMsg.duration, advanceControlMsg.interval));
                Debug.Log("接收到振动");
                break;
        }
//#endif
    }

    private IEnumerator Viberate(float duration, float interval)
    {
        WaitForSeconds wait = new WaitForSeconds(interval);
        for (float time = 0;  time < duration; time += interval)
        {
            Handheld.Vibrate();
            yield return wait;
        }
    }
    
    
}