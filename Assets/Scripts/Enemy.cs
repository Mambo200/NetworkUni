using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Enemy : NetworkBehaviour {

    public float m_speed;
    private NavMeshAgent m_mesh;

    public override void OnStartServer()
    {
        if (!isServer)
            return;

        m_mesh = GetComponent<NavMeshAgent>();
        m_mesh.Warp(new Vector3(1, 0, 1));
    }
    // Update is called once per frame
    void Update()
    {
        if (!isServer)
            return;

        // get first player to find
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            m_mesh.SetDestination(player.transform.position);
            //m_mesh.destination = player.transform.position;
            //m_mesh.Move(player.transform.position);
            //MoveToPlayer(player);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
    }

    //private void MoveToPlayer(GameObject _player)
    //{
    //    Vector3 posEmeny = transform.position;
    //    Vector3 posPlayer = _player.transform.position;
    //
    //    float division = GetDistance(transform.position, _player.transform.position) * m_speed;
    //
    //    Vector3 toMove = ((posPlayer - posEmeny) / division);
    //    transform.Translate(toMove);
    //}
    //
    //private float GetDistance(Vector3 _this, Vector3 _other)
    //{
    //    Vector3 v = _other - _this;
    //    float allSquare = (v.x * v.x) + (v.y * v.y) + (v.z * v.z);
    //    allSquare = Mathf.Sqrt(allSquare);
    //
    //    return allSquare;
    //}
}
