using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleChoosingUIController : MonoBehaviour {
    public Button P1Pengu;
    public Button P1Pig;
    public Button P2Pengu;
    public Button P2Pig;
    public Button P3Pengu;
    public Button P3Pig;
    public Button P4Pengu;
    public Button P4Pig;

    public Button confirmButton;

    [HideInInspector] public Button[] buttons;

    public bool roleSelected { get; private set; } 
    public int selectedGid { get; private set; }
    public int selectedUid { get; private set; }

    public System.Action confirmFinalAction;

    
    public void InitButtons()
    {
        buttons = new Button[8];
        buttons[0] = P1Pengu;
        buttons[1] = P1Pig;
        buttons[2] = P2Pengu;
        buttons[3] = P2Pig;
        buttons[4] = P3Pengu;
        buttons[5] = P3Pig;
        buttons[6] = P4Pengu;
        buttons[7] = P4Pig;
    }

    public void OnConfirm()
    {
        if (roleSelected == true)
        {
            // todo 在选择角色按钮上添加一层“蒙布”  而不是直接设置按钮 enable 属性为 false
            foreach (Button button in buttons)
                button.interactable = false;
        }
    }
    
    // todo 到时候在这里为 button 设置效果
    public void SetButtonRoleAvailable(int gid, int uid)
    {
        Debug.Log(gid+ " " + uid + " available");

        buttons[gid*2 + uid].interactable = true;
    }

    // todo 到时候在这里为 button 设置效果
    public void SetButtonRoleUnavailable(int gid, int uid)
    {
        Debug.Log(gid + " " + uid + " selected");
        buttons[gid*2 + uid].interactable = false;
    }

    // todo 到时候在这里为 button 设置效果
    public void SetRoleSelected(int gid, int uid)
    {
        roleSelected = true;
        //selectedGid = gid;
        //selectedUid = uid;
        buttons[gid*2 + uid].interactable = false;
    }
}
