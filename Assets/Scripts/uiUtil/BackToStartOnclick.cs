using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackToStartOnclick : MonoBehaviour
{

	private PanelController panelController;
	// Use this for initialization
	void Start () {
		panelController = FindObjectOfType<PanelController>();
		Button prepareButton = GetComponent<Button>();
		prepareButton.onClick.AddListener(delegate
		{
			Client.Instance.networkClient.Disconnect();
			panelController.SwitchToStageUI(Stage.StartStage);
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
