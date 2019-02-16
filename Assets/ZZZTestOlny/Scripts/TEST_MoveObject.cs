using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_MoveObject : MonoBehaviour {

    
    public float m_Speed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        float leftRight = Input.GetAxis("Horizontal");
        float forwardBackwards = Input.GetAxis("Vertical");
        float yMovement = 0;

        if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.LeftControl))
            yMovement = 0;
        else if (Input.GetKey(KeyCode.Space))
            yMovement = m_Speed;
        else if (Input.GetKey(KeyCode.LeftControl))
            yMovement = -m_Speed;


        leftRight *= Time.deltaTime;
        forwardBackwards *= Time.deltaTime;
        yMovement *= Time.deltaTime;

        Vector3 translateVector = new Vector3(leftRight, yMovement, forwardBackwards);
        Debug.Log(leftRight);
        gameObject.transform.Translate(translateVector);
	}
}
