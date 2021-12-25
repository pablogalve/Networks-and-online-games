using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NetworkedObjectType
{
    PLAYER,
    PROJECTILE,
    ENEMY,
    POWER_UP
}

public class NetworkedObject : MonoBehaviour
{
    public NetworkedObjectType networkedObjectType;

    [HideInInspector]
    public string id = "-1";

    private float interpolationSpeed  = 5.0f;
    [HideInInspector]
    public Vector3 desiredPosition;

    public UDPObject udpObject = null;

    public void Init()
    {
        if(id.Length == 0 || id == "-1")
        {
            id = Random.Range(0, int.MaxValue).ToString();
        }

        interpolationSpeed = 2.5f;
        desiredPosition = Vector3.zero;
    }

    public virtual void Update()
    {
        if(desiredPosition != Vector3.zero)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, interpolationSpeed * Time.deltaTime);
        }
    }

    public virtual void Die()
    {

    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        GameManager.instance.OnObjectCollided(gameObject, collision.gameObject);
    }
}
