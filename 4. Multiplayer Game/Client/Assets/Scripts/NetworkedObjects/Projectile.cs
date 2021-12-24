using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : NetworkedObject
{
    public float speed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
       base.Init();

        networkedObjectType = NetworkedObjectType.PROJECTILE;

        if (GameManager.instance.udpObject.connectionType == ConnectionType.SERVER)
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                //TODO: Uncomment
                collider.enabled = false;
            }
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        transform.Translate(new Vector3(1.0f, 0.0f, 0.0f) * speed * Time.deltaTime, Space.World);
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Destroy"))
        {
            Destroy(gameObject);
            //TODO: Tell the server to destroy this object
        }
    }
}
