using UnityEngine.Networking;

public class AdvanceControlHandler
{
    /*
     * 在这里写回调，然后在 Client.StartClient 把回调注册上去
     * 可以自己定义消息体来用来分辨后面多加的功能，比如震动、声音什么
     * AdvanceControl 那个消息号预留给你了，需要的话自己再定义新的消息号
     * 要注册几个回调函数，几个消息号按照自己的需求和习惯，注意和miniGameServer的消息号数量保持一致
     */
    public void OnReceiveAdvanceControl(NetworkMessage netmsg)
    {
        
    }
}