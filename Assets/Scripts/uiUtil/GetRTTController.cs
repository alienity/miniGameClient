using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GetRTTController : MonoBehaviour
{
    private Text rttText;

    private void Awake()
    {
        rttText = GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        rttText.text = Client.Instance.networkClient.GetRTT() + "ms";
    }
}