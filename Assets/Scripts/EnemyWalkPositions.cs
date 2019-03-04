using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyWalkPositions : NetworkBehaviour
{
    public static Transform[] Get { get; private set; }


    public override void OnStartServer()
    {
        base.OnStartServer();
        Initialize();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Initialize();
    }

    private void Initialize()
    {
        // get every object
        Transform[] go = this.GetComponentsInChildren<Transform>();

        List<Transform> list = new List<Transform>();
        // filter this
        foreach (Transform t in go)
        {
            if (t != this.transform)
            {
                list.Add(t);
            }
        }

        Get = list.ToArray();
    }

    // Use this for initialization
    void Start()
    {

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
