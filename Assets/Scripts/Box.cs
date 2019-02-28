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

    [SyncVar]
    public float m_DeadCounter = 0;
    private Vector3 m_lastPosition;
    private Vector3 m_nextPosition;
    private float m_currentLerpTime;
    private float m_totalLerpTime;
    private List<Vector3> m_nextPositions
        = new List<Vector3>();

    [SerializeField]
    private Camera m_playerCamera;

    private Rigidbody m_rigidbody;

    public override void OnStartLocalPlayer()
    {
        m_playerCamera.gameObject.SetActive(true);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0,
                Input.GetAxisRaw("Vertical"));
            dir = dir.normalized * Time.deltaTime * m_MovementSpeed;
            transform.Translate(dir);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isServer)
            return;

        ResetPosition();
    }

    [Server]
    private void ResetPosition()
    {
        m_DeadCounter++;
        RpcResetPosition(Spawnpoint.GetOne().gameObject.transform.position);
    }

    [ClientRpc]
    private void RpcResetPosition(Vector3 _newPosition)
    {
        gameObject.transform.position = _newPosition;
        Debug.Log(_newPosition);
    }
}
