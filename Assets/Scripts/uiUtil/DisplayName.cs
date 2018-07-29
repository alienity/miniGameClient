using UnityEngine;
using UnityEngine.UI;

public class DisplayName: MonoBehaviour
{
    private Text displayNameText;

    private void Start()
    {
        displayNameText = GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        displayNameText.text = "欢迎回来，" + Client.Instance.playerName;
    }
}