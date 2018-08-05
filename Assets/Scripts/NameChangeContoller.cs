using UnityEngine;
using UnityEngine.UI;

public class NameChangeContoller : MonoBehaviour
{
    public InputField inputField;
    private PanelController panelController;
    public Button submitButton;


    public static string NameProperty = "playerName";

    private void Start()
    {
//        PlayerPrefs.DeleteKey(NameProperty);
        panelController = FindObjectOfType<PanelController>();
        if (PlayerPrefs.HasKey(NameProperty))
        {
            Client.Instance.playerName = PlayerPrefs.GetString(NameProperty);
            panelController.SwitchToStageUI(Stage.StartStage);
        }

        Debug.Log("name changer");

        submitButton.onClick.AddListener(delegate
        {
            Debug.Log("submit name");
            Client.Instance.playerName = inputField.text;
            PlayerPrefs.SetString(NameProperty, inputField.text);
            Debug.Log("plyaer name: " + Client.Instance.playerName);
            panelController.SwitchToStageUI(Stage.StartStage);
        });
    }
}