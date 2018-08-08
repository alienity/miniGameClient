using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class reStartCountdown : MonoBehaviour
{
    public int countdown = 10;
    private Text countdownText;
    private PanelController panelController;


    private void Start()
    {
        countdownText = GetComponent<Text>();
        panelController = FindObjectOfType<PanelController>();
        Client.Instance.networkClient.Disconnect();
        StartCoroutine(CountDownToStart(countdown));
    }

    IEnumerator CountDownToStart(int time)
    {
        while (time > 0)
        {
            countdownText.text = "重启倒计时 " + time;
            yield return new WaitForSeconds(1);
            --time;
        }
        panelController.SwitchToStage(Stage.StartStage);
    }
}