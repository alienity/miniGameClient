//To use this example, attach this script to an empty GameObject.
//Create two buttons (Create>UI>Button). Next, click your empty GameObject in the Hierarchy and click and drag each of your Buttons from the Hierarchy to the Your Button and "Your Second Button" fields in the Inspector.
//Click the Button in Play Mode to output the message to the console.

using UnityEngine;
using UnityEngine.UI;

public class ButtonOnClick : MonoBehaviour
{
    //Make sure to attach these Buttons in the Inspector
    public Button m_YourButton;
    //public Button m_YourSecondButton;

    void Start()
    {
        Button btn = m_YourButton.GetComponent<Button>();
        //Button btn2 = m_YourSecondButton.GetComponent<Button>();

        //Calls the TaskOnClick method when you click the Button
        btn.onClick.AddListener(TaskOnClick);

        //m_YourSecondButton.onClick.AddListener(delegate { TaskWithParameters("Hello"); });
    }

    void TaskOnClick()
    {
        //Output this to console when the Button is clicked
        Debug.Log("You have clicked the button!");
    }

    //void TaskWithParameters(string message)
    //{
    //    //Output this to console when the Button is clicked
    //    Debug.Log(message);
    //}
}
