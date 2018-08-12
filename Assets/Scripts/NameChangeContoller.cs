using UnityEngine;
using UnityEngine.UI;

public class NameChangeContoller : MonoBehaviour
{
    public InputField inputField;
    private PanelController panelController;
    public Button submitButton;
    public AudioClip NormalButtonAudio;

    public static string NameProperty = "playerName";

    private void Start()
    {
//        PlayerPrefs.DeleteKey(NameProperty);
        panelController = FindObjectOfType<PanelController>();
        if (PlayerPrefs.HasKey(NameProperty) && PlayerPrefs.GetString(NameProperty) != "")
        {
            Client.Instance.playerName = PlayerPrefs.GetString(NameProperty);
            // panelController.SwitchToStageUI(Stage.StartStage);
        }

        Debug.Log("name changer");

        submitButton.onClick.AddListener(delegate
        {
            this.gameObject.GetComponent<AudioSource>().clip = NormalButtonAudio;
            this.gameObject.GetComponent<AudioSource>().pitch = 2;
            this.gameObject.GetComponent<AudioSource>().Play();
            Debug.Log("submit name");
            Client.Instance.playerName = inputField.text;
            PlayerPrefs.SetString(NameProperty, inputField.text);
            Debug.Log("plyaer name: " + Client.Instance.playerName);
            panelController.SwitchToStageUI(Stage.StartStage);
        });
    }
}