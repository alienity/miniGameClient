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
		// 进入等待连接的界面
		prepareButton.onClick.AddListener(delegate { panelController.SwitchToStage(Stage.Prepare); });

	}	
}
