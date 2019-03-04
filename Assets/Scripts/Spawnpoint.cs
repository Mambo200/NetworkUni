using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Extensions;

public class Spawnpoint : NetworkBehaviour {

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
        Get = getList.ToArray();
        Debug.Log(Get.Length);
    }

    public static Transform[] GetAll()
    {
        return Get;
    }

    public static Transform GetOne()
    {
        return Get.GetOne();
    }
}
