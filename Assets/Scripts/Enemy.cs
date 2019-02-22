using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Enemy : NetworkBehaviour {

    public float m_speed;

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
            return;
        // get first player to find
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            MoveToPlayer(player);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
    }

    private void MoveToPlayer(GameObject _player)
    {
        Vector3 posEmeny = transform.position;
        Vector3 posPlayer = _player.transform.position;

        float division = GetDistance(transform.position, _player.transform.position) * m_speed;

        Vector3 toMove = ((posPlayer - posEmeny) / division);
        transform.Translate(toMove);
    }

    private float GetDistance(Vector3 _this, Vector3 _other)
    {
        Vector3 v = _other - _this;
        float allSquare = (v.x * v.x) + (v.y * v.y) + (v.z * v.z);
        allSquare = Mathf.Sqrt(allSquare);

        return allSquare;
    }
}
