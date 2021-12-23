using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public UDPObject udpObject;

    private int score = 0;


    void Start()
    {
        instance = this;
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        Debug.Log(score);
    }

    public void OnObjectCollided(GameObject colliderObject, GameObject collidedObject)
    {
        if (udpObject.connectionType == ConnectionType.CLIENT)
        {
            Client client = udpObject as Client;

            NetworkedObject networkedCollider = colliderObject.GetComponent<NetworkedObject>();
            NetworkedObject networkedCollided = collidedObject.GetComponent<NetworkedObject>();


            CollisionMessage collisionMessage = new CollisionMessage(networkedCollider.id, networkedCollided.id);
            client.Send(collisionMessage);
        }
    }

    public void OnObjectDead(GameObject deadObject)
    {
        NetworkedObject networkedObject = deadObject.GetComponent<NetworkedObject>();

        if (networkedObject != null)
        {

            //Debug.Log("Dead object id: " + networkedObject.id);
        }
    }
}
