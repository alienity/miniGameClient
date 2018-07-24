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
    //public SkillButton skillButton;
    [HideInInspector]
    public bool skill = false;
    public bool finish = false;
    
    public void ChangeSkillState(bool state)
    {
        skillButton.interactable = state;

    }

    private void Start()
    {
        //skillButton.onClick = delegate { skill = true; };
        //skillButton.onFinish = delegate { finish = true; };
        skillButton.onClick.AddListener(delegate { skill = true; });
    }

}