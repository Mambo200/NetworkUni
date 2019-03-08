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
    public Material m_thisPlayerMaterial;
    public float m_MovementSpeed;

    [SyncVar]
    public float m_DeadCounter = 0;

    [SerializeField]
    private Camera m_playerCamera;
    private Rigidbody m_rigidbody;
    private Renderer m_renderer;

    public override void OnStartLocalPlayer()
    {
        m_playerCamera.gameObject.SetActive(true);
    }

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
        // get rigidbody
        m_rigidbody = GetComponent<Rigidbody>();
        // freeze rotation of rigidbody
        m_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        // get renderer
        m_renderer = GetComponent<Renderer>();

    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            this.gameObject.name += "(this)";
            m_renderer.material = m_thisPlayerMaterial;
        }
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
        if (collision.transform.tag == "Player")
            m_rigidbody.isKinematic = true;

        if (!isServer)
            return;

        if (collision.transform.tag != "Enemy")
            return;

        ResetPosition();
    }

    private void OnCollisionExit(Collision collision)
    {
        m_rigidbody.isKinematic = false;
    }

    [Server]
    private void ResetPosition()
    {
        // do nothing if enemy is confused
        if (Enemy.DebuffType == Assets.Scripts.ItemType.CONFUSED)
            return;

        m_DeadCounter++;
        RpcResetPosition(Spawnpoint.GetOne().gameObject.transform.position);
    }

    [ClientRpc]
    private void RpcResetPosition(Vector3 _newPosition)
    {
        gameObject.transform.position = _newPosition;
    }
}
