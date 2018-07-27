using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrepareButtonOnclick : MonoBehaviour
{
	private PanelController panelController;
	
	// Use this for initialization
	void Start ()
	{
		panelController = FindObjectOfType<PanelController>();
		Button prepareButton = GetComponent<Button>();
		prepareButton.onClick.AddListener(delegate { panelController.SwitchToStage(Stage.ConnectToNetStage); });

	}	
}
