using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetConnectMaskController : MonoBehaviour
{

    public Image netConnectionPanel;
    
    public void EnableNetConnectionMaskPanel(bool flag)
    {
        netConnectionPanel.gameObject.SetActive(flag);
    }

}
