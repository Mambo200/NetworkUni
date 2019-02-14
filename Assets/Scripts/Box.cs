using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*************************************
 * Client                    Server  *
 *  --------- Command ------------>  *
 *  <-------- Client RPC ----------  *
 *  <-------- Sync Var ------------  *
 *************************************/



public class Box : NetworkBehaviour
{
    public float m_MovementSpeed;

    [SyncVar(hook = "OnHpChanged")]
    public float m_HP = 100;

    private Vector3 m_lastPosition;
    private Vector3 m_nextPosition;
    private float m_currentLerpTime;
    private float m_totalLerpTime;
    private List<Vector3> m_nextPositions
        = new List<Vector3>();
    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            m_currentLerpTime += Time.deltaTime;
            transform.position =
                Vector3.Lerp(m_lastPosition, m_nextPosition,
                  m_currentLerpTime / m_totalLerpTime);
            if ((m_currentLerpTime / m_totalLerpTime) > 1)
            {
                if (m_nextPositions.Count > 0)
                {
                    m_currentLerpTime = 0;
                    m_lastPosition = transform.position;
                    m_nextPosition = m_nextPositions[0];
                    m_nextPositions.RemoveAt(0);
                    m_totalLerpTime = (m_nextPosition - transform.position).magnitude
                        / m_MovementSpeed;
                }
            }
            return;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            CmdTakeDamage(10);
        }
        Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0,
            Input.GetAxisRaw("Vertical"));
        dir = dir.normalized * Time.deltaTime * m_MovementSpeed;
        transform.position += dir;


        if (Time.frameCount % 10 == 0)
            CmdSetPosition(transform.position);
    }

    [Command]
    private void CmdTakeDamage(float _damage)
    {
        m_HP -= _damage;
    }

    // Nachricht vom Client an den Server
    [Command(channel = 1)]
    private void CmdSetPosition(Vector3 _position)
    {
        // if ((transform.position - _position).sqrMagnitude >
        //    Mathf.Pow((m_MovementSpeed * (Time.fixedDeltaTime * 2))
        //    , 2))
        // {
        //     RpcSetPosition(_position);
        //     return;
        // }
        if (_position != transform.position)
        {
            m_nextPositions.Add(_position);
            RpcSetPosition(_position);
        }
    }

    // Nachricht vom Server an die Clients
    [ClientRpc(channel = 1)]
    private void RpcSetPosition(Vector3 _position)
    {
        if (!isLocalPlayer)
        {
            if (_position != transform.position)
            {
                m_nextPositions.Add(_position);
            }
        }
    }

    private void OnHpChanged(float _newHP)
    {
        gameObject.name = m_HP + "|" + _newHP;
    }
}
