using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PanelController : MonoBehaviour
{

    public GameObject preparePanel;
    public GameObject startPanel;
    public GameObject roomCanvas;
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    public GameObject connectingToNet;

    public NetworkClient netClient;

    private List<GameObject> panels = new List<GameObject>();

    private void Start()
    {
        panels.Add(preparePanel);
        panels.Add(startPanel);
        panels.Add(roomCanvas);
        panels.Add(gamePanel);
        panels.Add(gameOverPanel);
        panels.Add(connectingToNet);
    }

    private void Update()
    {
        if (netClient == null || Client.Instance.networkClient == null)
        {
            netClient = Client.Instance.networkClient;
            netClient.RegisterHandler(CustomMsgType.ClientChange, ChangePanel);
        }
    }

    // 手动切换界面
    public void ChangePanelByHand()
    {

    }

    public void ChangePanel(NetworkMessage netmsg)
    {
        Debug.Log("改变界面");
        SceneTransferMsg stm = netmsg.ReadMessage<SceneTransferMsg>();
        string curSceneName = stm.curSceneName;
        string nextSceneName = stm.nextSceneName;
        Debug.Log(nextSceneName);
        // 暂时先设置成只从要转换的下一个场景读
        if(nextSceneName == "GameScene")
        {
            foreach (GameObject pgo in panels)
            {
                pgo.SetActive(false);
            }
            gamePanel.SetActive(true);

        }
    }

    public void EnableNetConnectionMaskPanel(bool enable)
    {
        connectingToNet.SetActive(enable);
    }

}
