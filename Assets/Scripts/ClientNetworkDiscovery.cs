using UnityEngine;
using UnityEngine.Networking;

public class ClientNetworkDiscovery : NetworkDiscovery
{
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        StopBroadcast();
        string ip = "";
        bool ok = false;
        foreach (var c in fromAddress)
        {
            if (c >= '0' && c <= '9') ok = true;
            if (ok) ip += c;
        }
        Debug.Log(ip);
        Client.ipv4 = ip;
    }
}
