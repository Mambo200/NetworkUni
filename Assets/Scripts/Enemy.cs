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
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        if (player.Length > 0 && player != null)
        {
            float[] distance = GetDistanceOfAllPlayer(player);
            GameObject playerToFollow = player[ClosestPlayer(distance)];

            m_mesh.SetDestination(playerToFollow.transform.position);
            //m_mesh.destination = player.transform.position;
            //m_mesh.Move(player.transform.position);
            //MoveToPlayer(player);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
    }

    /// <summary>
    /// Get Distance of gameobject array
    /// </summary>
    /// <param name="_players">all players found</param>
    /// <returns>distance of all players</returns>
    private float[] GetDistanceOfAllPlayer(GameObject[] _players)
    {
        float[] arrayToReturn = new float[_players.Length];

        // get distance
        for (int i = 0; i < _players.Length; i++)
            arrayToReturn[i] = Vector3.Distance(this.gameObject.transform.position, _players[i].transform.position);

        // return distance
        return arrayToReturn;
    }

    /// <summary>
    /// looking for the smallest amount
    /// </summary>
    /// <param name="_playerDistances">Distance array</param>
    /// <returns>indexposition of lowest distance</returns>
    private int ClosestPlayer(params float[] _playerDistances)
    {
        int arrayPos = 0;
        float lowest = float.MaxValue;

        for (int i = 0; i < _playerDistances.Length; i++)
        {
            if (_playerDistances[i] < lowest)
            {
                lowest = _playerDistances[i];
                arrayPos = i;
            }
        }

        return arrayPos;
    }

    /// <summary>
    /// looking for the smallest amount
    /// </summary>
    /// <param name="lowest">lowest value found</param>
    /// <param name="_playerDistances">Distance array</param>
    /// <returns>indexposition of lowest distance</returns>
    private int ClosestPlayer(out float lowest, params float[] _playerDistances)
    {
        int arrayPos = 0;
        lowest = float.MaxValue;

        for (int i = 0; i < _playerDistances.Length; i++)
        {
            if (_playerDistances[i] < lowest)
            {
                lowest = _playerDistances[i];
                arrayPos = i;
            }
        }

        return arrayPos;
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
