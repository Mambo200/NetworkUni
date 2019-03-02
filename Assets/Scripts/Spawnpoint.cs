using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spawnpoint : NetworkBehaviour {

    private static Transform[] get;

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
        Transform[] all = GetComponentsInChildren<Transform>();
        List<Transform> getList = new List<Transform>();

        // remove Platforms and parent
        foreach (Transform t in all)
        {
            if (t.name == "Spawnpoint")
            {
                getList.Add(t);
            }
        }
        get = getList.ToArray();
        Debug.Log(get.Length);
    }

    public static Transform[] GetAll()
    {
        return get;
    }

    public static Transform GetOne()
    {
        int randomSpawnpoint = Random.Range(0, (get.Length - 1));
        return get[randomSpawnpoint];
    }
}
