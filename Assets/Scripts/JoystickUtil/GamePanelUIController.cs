using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanelUIController : MonoBehaviour {

    [Header("技能CD和名字Icone")]
    public Image IconeImage;
    public Image MaskImage;
    public Text NameText;
    public Text CoolingTimeText;
    [System.Serializable]
    public struct CoolingIcone
    {
        public Sprite RoleIcone;
        public Sprite MaskIcone;
    }
    public List<CoolingIcone> coolingIcone = new List<CoolingIcone>();

    public void showIconeAndName(int gid,int uid, string name)
    {
        IconeImage.sprite = coolingIcone[gid * 2 + uid].RoleIcone;
        MaskImage.sprite = coolingIcone[gid * 2 + uid].MaskIcone;
        NameText.text = name;
    }

    public void showCoolingTimeAndICone(float remainTime, float totleTime)
    {
        if(remainTime > 0.1f)
        {
            CoolingTimeText.gameObject.SetActive(true);
            CoolingTimeText.text = ((int)remainTime).ToString();
        }
        else
        {
            CoolingTimeText.gameObject.SetActive(false);
        }
        
        MaskImage.fillAmount = remainTime / totleTime;
    }
}
