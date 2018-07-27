using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyJoyTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnJoystickMove(Vector2 move)
    {
        Debug.Log(move);
    }
    

}
