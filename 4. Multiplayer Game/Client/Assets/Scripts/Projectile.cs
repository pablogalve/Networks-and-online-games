using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : NetworkedObject
{
    public float speed = 5.0f;

    public GameObject bulletsContainer;

    // Start is called before the first frame update
    void Start()
    {
       base.Init();

       //transform.SetParent(bulletsContainer.transform);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(1.0f, 0.0f, 0.0f) * speed * Time.deltaTime, Space.World);
    }

    void OnCollisionEnter(Collision collision)
    {
        Die();
    }

    public override void Die()
    {
        base.Die();
    }
}
