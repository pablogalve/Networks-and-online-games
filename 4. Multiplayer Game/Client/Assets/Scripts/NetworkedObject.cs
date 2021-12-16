using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedObject : MonoBehaviour
{
    [HideInInspector]
    public int id;

    public short instancedId = 0;

    public void Init()
    {
        id = Random.Range(0, int.MaxValue);
    }

    public virtual void Die()
    {
        GameManager.instance.OnObjectDead(gameObject);

        Destroy(gameObject);
    }
}
