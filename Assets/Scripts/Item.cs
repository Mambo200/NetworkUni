using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts;

public class Item : NetworkBehaviour {

    public float m_rotationSpeed = 100;
    public ItemType m_type;

    public float deactivatedTime;

    // Update is called once per frame
    void Update ()
    {
        RotateScript.RotationY(this.gameObject, m_rotationSpeed);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (deactivatedTime != 0) return;

        if (!isServer) return;

        if (other.tag == "Player")
        {
            Enemy.SetDebuff(m_type, Enemy.DefaultDebuffTime);
            ItemManager.AddItem(this);
        }
    }

}
