using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PenguSwitch2TouchpadOnclick : MonoBehaviour {

    private PanelController panelController;

    // Use this for initialization
    void Awake()
    {
        panelController = FindObjectOfType<PanelController>();
        Button button = GetComponent<Button>();
        // 进入制作者名单界面
        button.onClick.AddListener(delegate
        {
            panelController.penguJoystickPanel.SetActive(false);
            panelController.penguTouchPadPanel.SetActive(true);
        });

    }
}
