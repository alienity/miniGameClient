using UnityEngine;
 using UnityEngine.UI;
 
 public class ReNameButtonOnclick: MonoBehaviour
 {
    private PanelController panelController;
     private void Start()
     {
         panelController = FindObjectOfType<PanelController>();
         GetComponent<Button>().onClick.AddListener(delegate
         {
             panelController.SwitchToStageUI(Stage.ChangeNameStage);
         });
     }
 }