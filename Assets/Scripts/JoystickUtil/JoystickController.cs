using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class JoystickController : MonoBehaviour
{
    [Header("Joystick")]
    public FixedJoystick joystick;
    [Header("SkillButton")]
    public Button skillButton;
    [HideInInspector]
    public bool skill = false;
    
    public void ChangeSkillState(bool state)
    {
        skillButton.interactable = state;
    }

    private void Start()
    {
        skillButton.onClick.AddListener(delegate { skill = true; });
    }

}