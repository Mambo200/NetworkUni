﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using Assets.Scripts;
using Extensions;

public class Enemy : NetworkBehaviour {

    /// <summary>Speed of Enemy</summary>
    public float m_speed;
    /// <summary>NavMeshAgent of Enemy</summary>
    private NavMeshAgent m_mesh;
    /// <summary>Rigidbody of Enemy</summary>
    private Rigidbody m_rigidBody;
    /// <summary>Enemy does not hunt for players who have a higher y-position</summary>
    private float m_distanceToSpawn;
    
    public static float DefaultDebuffTime { get { return 10f; } }
    /// <summary>Player debuff</summary>
    private static ItemType debuffType = ItemType.NONE;
    /// <summary>get Player debuff</summary>
    public static ItemType DebuffType { get { return debuffType; } }
    /// <summary>Time the debuf lasts</summary>
    private static float debuffTime;
    /// <summary>Distance enemy can spot player</summary>
    private float MaxDistance { get { return debuffType == ItemType.BLIND ? 100f : 250f; } }

    /// <summary>Hunt Behaviour</summary>
    public static EnemyBehaviour HuntBeh { get { return EnemyBehaviour.HUNT; } }
    /// <summary>Walk Behaviour</summary>
    public static EnemyBehaviour WALKBeh { get { return EnemyBehaviour.WALK; } }
    /// <summary>when hunted player is out of range, walk to last known position</summary>
    public static EnemyBehaviour WALKLASTPOSBeh { get { return EnemyBehaviour.CHECKLASTPOSITION; } }
    /// <summary>Enemy can be this amount away from accurate WalkPosition</summary>
    public static float WalkPositionDistance { get { return 1f; } }

    /// <summary>Last position of hunted player</summary>
    private Vector3 m_lastPositionPlayer;

    /// <summary>Last Behaviour</summary>
    private EnemyBehaviour m_lastBehaviour;
    /// <summary>Next Behaviour</summary>
    private EnemyBehaviour m_nextBehaviour;
    /// <summary>DO NOT USE! SEE <see cref="WalkIndex"/></summary>
    private int m_walkIndex = 0;
    /// <summary>Index of Walk position</summary>
    private int WalkIndex
    {
        get { return m_walkIndex; }
        set
        {
            m_walkIndex = (value >= EnemyWalkPositions.Get.Length) ? m_walkIndex = 0 : m_walkIndex = value;
        }
        
    }

    public override void OnStartServer()
    {
        m_rigidBody = GetComponent<Rigidbody>();

        if (!isServer)
            return;

        m_mesh = GetComponent<NavMeshAgent>();
        m_mesh.Warp(new Vector3(1, 0, 1));   
    }

    private void Start()
    {
        if (m_rigidBody == null)
            m_rigidBody = GetComponent<Rigidbody>();

        if (!isServer)
            return;

        m_distanceToSpawn = Spawnpoint.GetOne().transform.position.y - 10;

    }
    // Update is called once per frame
    void Update()
    {
        if (!isServer) return;
        switch (m_nextBehaviour)
        {
            case EnemyBehaviour.WALK:
                Walk();
                m_lastBehaviour = WALKBeh;
                break;
            case EnemyBehaviour.HUNT:
                Hunt();
                m_lastBehaviour = HuntBeh;
                break;
            case EnemyBehaviour.CHECKLASTPOSITION:
                LastPos();
                m_lastBehaviour = WALKLASTPOSBeh;
                break;
            default:
                m_lastBehaviour = WALKBeh;
                m_nextBehaviour = WALKBeh;
                break;
        }

        switch (debuffType)
        {
            case ItemType.NONE:
                break;
            case ItemType.BLIND:
                {
                    debuffTime -= Time.deltaTime;

                    if (debuffTime <= 0)
                        debuffType = ItemType.NONE;
                }
                break;
            case ItemType.CONFUSED:
                {
                    debuffTime -= Time.deltaTime;

                    if (debuffTime <= 0)
                        debuffType = ItemType.NONE;
                    else
                        m_nextBehaviour = EnemyBehaviour.WALK;
                }
                break;
            default:
                break;
        }
    }

    private void Walk()
    {
        // get all player exept player at spawn
        GameObject[] player = NearbyPlayer().ToArray();
        if (player.Length > 0 && player != null && debuffType != ItemType.CONFUSED)
        { 
            m_nextBehaviour = HuntBeh;
        }
        else
        {
            // walk to next point
            m_mesh.SetDestination(EnemyWalkPositions.Get[WalkIndex].position);

            // if enemy is near position set new position
            if (IsInRange(this.transform.position.x, EnemyWalkPositions.Get[WalkIndex].position.x) &&
                IsInRange(this.transform.position.z, EnemyWalkPositions.Get[WalkIndex].position.z))
            {
                WalkIndex++;
            }
        }

        
    }

    private void Hunt()
    {
        // get all player exept player at spawn
        GameObject[] player = NearbyPlayer().ToArray();
        if (player.Length > 0 && player != null)
        {
            float[] distance = GetDistanceOfAllPlayer(player);
            GameObject playerToFollow = player[ClosestPlayer(distance)];

            m_lastPositionPlayer = playerToFollow.transform.position;
            m_mesh.SetDestination(m_lastPositionPlayer);

        }
        else
        {
            // get next behaviour
            m_nextBehaviour = WALKLASTPOSBeh;
        }

    }

    private void LastPos()
    {
        // get all player exept player at spawn
        GameObject[] player = NearbyPlayer().ToArray();
        if (player.Length > 0 && player != null)
        {
            m_nextBehaviour = HuntBeh;
            return;
        }

        // if enemy is near last known player position change behaviour
        if (IsInRange(this.transform.position.x, m_lastPositionPlayer.x) &&
            IsInRange(this.transform.position.z, m_lastPositionPlayer.z))
        {
            m_nextBehaviour = WALKBeh;
        }
        else
        {
            m_mesh.SetDestination(m_lastPositionPlayer);
        }

    }

    private List<GameObject> NearbyPlayer()
    {
        // get first player to find
        List<GameObject> playerList = GameObject.FindGameObjectsWithTag("Player").ToList();
        // remove player who is at spawn
        playerList = RemovePlayerAtSpawn(playerList);
        return RemoveFromHighDistance(playerList);
    }

    private List<GameObject> RemoveFromHighDistance(List<GameObject> _player)
    {
        List<GameObject> listToReturn = new List<GameObject>();

        foreach (GameObject go in _player)
        {
            // calculate Distance
            Vector3 v = go.transform.position - this.transform.position;
            float distance = (v.x * v.x) + (v.y * v.y) + (v.z * v.z);
            if (distance <= MaxDistance)
            {
                listToReturn.Add(go);
            }
        }

        return listToReturn;
    }

    /// <summary>
    /// check if two values are almost equal (for value see <b> <see cref="WalkPositionDistance"/></b>)
    /// </summary>
    /// <param name="_pos1">first position</param>
    /// <param name="_pos2">second position</param>
    /// <returns>
    ///   <c>true</c> if positions are almose equal; otherwise, <c>false</c>.
    /// </returns>
    private bool IsInRange(float _pos1, float _pos2)
    {
        if (_pos1 + WalkPositionDistance >= _pos2 &&
            _pos1 - WalkPositionDistance <= _pos2)
        {
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Remove Gameobjects from array when player is at spawnpoint
    /// </summary>
    /// <param name="_player"></param>
    private List<GameObject> RemovePlayerAtSpawn(List<GameObject> _player)
    {
        List<GameObject> list = new List<GameObject>();
        foreach (GameObject go in _player)
        {
            if (go.transform.position.y <= m_distanceToSpawn)
            {
                list.Add(go);
            }
        }

        return list;
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

    /// <summary>
    /// Set debuf status
    /// </summary>
    public static void SetDebuff(ItemType _type, float _debuffTime)
    {
        debuffType = _type;
        debuffTime = _debuffTime;
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
