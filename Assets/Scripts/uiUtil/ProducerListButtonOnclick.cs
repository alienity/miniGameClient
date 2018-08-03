using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProducerListButtonOnclick : MonoBehaviour {

    private PanelController panelController;

    // Use this for initialization
    void Start()
    {
        panelController = FindObjectOfType<PanelController>();
        Button producerListButton = GetComponent<Button>();
        // 进入制作者名单界面
        producerListButton.onClick.AddListener(delegate { panelController.SwitchToStage(Stage.ProducerListStage); });

    }
}
