using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GetRTTController : MonoBehaviour
{
    private Text rttText;

    private void Awake()
    {
        rttText = GetComponent<Text>();
        StartCoroutine(DisPlayRTT());
    }
    
    private IEnumerator DisPlayRTT()
    {
        while (true)
        {
            if (Client.Instance.networkClient != null && Client.Instance.networkClient.isConnected)
            {
                rttText.text = Client.Instance.networkClient.GetRTT() + "ms";
                Debug.Log("RTT is " + Client.Instance.networkClient.GetRTT());
            }
            yield return new WaitForSeconds(5);

        }
    }

}