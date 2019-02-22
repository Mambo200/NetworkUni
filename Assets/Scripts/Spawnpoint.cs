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
        get = GetComponentsInChildren<Transform>();
        Debug.Log(get.Length);
    }

    public static Transform[] GetAll()
    {
        return get;
    }

    public static Transform GetOne()
    {
        int randomSpawnpoint = Random.Range(0, (get.Length - 1));
        Debug.Log("Spawnpoint nr " + randomSpawnpoint);
        return get[randomSpawnpoint];
    }
}
