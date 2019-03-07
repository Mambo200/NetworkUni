using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHorse : MonoBehaviour {

    public float m_startXPos;
    public float m_endXPos;
    
    public float m_moveSpeed;
    public float m_downRotationSpeed;
    public float m_upRotationSpeed;
    public float m_maxZRotation;

    /// <summary>true: moves from start to end; false: moves from end to start</summary>
    private bool startToEnd = true;
    private bool rotationDown = true;
    private SpriteRenderer m_spriteRenderer;

    // Use this for initialization
    void Start ()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_spriteRenderer.flipX = false;
        m_spriteRenderer.flipY = false;
        transform.position = new Vector3(m_startXPos, 0, 0);
        transform.rotation = new Quaternion();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (startToEnd)
            StartToEnd();
        else
            EndToStart();
	}

    private void StartToEnd()
    {
        transform.position += new Vector3(m_moveSpeed, 0, 0) * Time.deltaTime;

        // rotate down
        if (rotationDown) // then -
        {
            transform.Rotate(new Vector3(0, 0, -m_downRotationSpeed));

            // check if rotation is unter m_maxZRotation
            if (transform.eulerAngles.z <= (360-m_maxZRotation) &&
                transform.eulerAngles.z >= (360 - (m_maxZRotation * 2)))
            { 
                rotationDown = false;
                transform.eulerAngles.Set(0, 0, 360 - m_maxZRotation);
            }
        }
        // rotate up
        else // then +
        {
            transform.Rotate(new Vector3(0, 0, m_upRotationSpeed));

            // check if rotation is above m_maxZRotation
            if (transform.eulerAngles.z >= m_maxZRotation &&
                transform.eulerAngles.z <= m_maxZRotation * 2)
            {
                rotationDown = true;
                transform.eulerAngles.Set(0, 0, m_maxZRotation);
            }
        }

        // check if sprite is out of vision
        if (transform.position.x >= m_endXPos)
        {
            SetEndPos();
        }
    }

    private void EndToStart()
    {
        transform.position -= new Vector3(m_moveSpeed, 0, 0) * Time.deltaTime;

        // rotate up
        if (rotationDown) // then -
        {
            transform.Rotate(new Vector3(0, 0, -m_upRotationSpeed));

            // check if rotation is unter m_maxZRotation
            if (transform.eulerAngles.z <= (360 - m_maxZRotation) &&
                transform.eulerAngles.z >= (360 - (m_maxZRotation * 2)))
            {
                rotationDown = false;
                transform.eulerAngles.Set(0, 0, 360 - m_maxZRotation);
            }
        }
        // rotate down
        else // then +
        {
            transform.Rotate(new Vector3(0, 0, m_downRotationSpeed));

            // check if rotation is above m_maxZRotation
            if (transform.eulerAngles.z >= m_maxZRotation &&
                transform.eulerAngles.z <= m_maxZRotation * 2)
            {
                rotationDown = true;
                transform.eulerAngles.Set(0, 0, m_maxZRotation);
            }
        }

        // check if sprite is out of vision
        if (transform.position.x <= m_startXPos)
        {
            SetStartPos();
        }

    }

    private void SetStartPos()
    {
        transform.position = new Vector3(m_startXPos, 0, 0);
        rotationDown = true;
        transform.eulerAngles.Set(0, 0, 0);
        startToEnd = true;
        m_spriteRenderer.flipX = false;
    }

    private void SetEndPos()
    {
        transform.position = new Vector3(m_endXPos, 0, 0);
        rotationDown = false;
        transform.eulerAngles.Set(0, 0, 0);
        startToEnd = false;
        m_spriteRenderer.flipX = true;

    }
}
