using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int score = 0;

    [SerializeField]
    private GameObject particlesContainer;

    void Start()
    {
        instance = this;
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        Debug.Log(score);
    }

    public void InstantiateParticles(GameObject particlesObject, Vector3 position)
    {
        GameObject particlesInstance = Instantiate(particlesObject, position, Quaternion.identity);

        ParticleSystem particleSystem = particlesInstance.GetComponent<ParticleSystem>();
        if(particleSystem != null)
        {
            particleSystem.Play();
            particlesInstance.transform.SetParent(particlesContainer.transform);
            Destroy(particlesInstance, particleSystem.main.duration * 0.8f);
        }
        else
        {
            Destroy(particlesInstance);
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
