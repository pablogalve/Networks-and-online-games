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
    public string id;

    public short instancedId = 0;

    private float interpolationSpeed  = 2.5f;
    [HideInInspector]
    public Vector3 desiredPosition;

    public void Init()
    {
        id = "-1";
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

    public virtual void OnCollisionEnter(Collision collision)
    {
        GameManager.instance.OnObjectCollided(gameObject, collision.gameObject);
    }
}
