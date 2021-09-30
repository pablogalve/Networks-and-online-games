using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;

public class ThreadedObjectSpawner : MonoBehaviour
{
    public float cooldownTime = 5.0f;
    private float passedTime = 0;
    public GameObject projectile;
    public float throwForce;

    List<Action> functionsToRunInMainThread;
    List<Thread> activeThreads;

    List<GameObject> createdObjects;

    // Start is called before the first frame update
    void Start()
    {
        functionsToRunInMainThread = new List<Action>();
        createdObjects = new List<GameObject>();
        activeThreads = new List<Thread>();
    }

    // Update is called once per frame
    void Update()
    {
        while (functionsToRunInMainThread.Count > 0)
        {
            //Grab the first/oldest function in the list
            Action someFunc = functionsToRunInMainThread[0];
            functionsToRunInMainThread.RemoveAt(0);
            someFunc();
        }

        //Exercise 1
        /*
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            createNewThread(throwProjectile);
        }
        */

        //Exercise 2
        /*
        if (Input.GetKeyDown(KeyCode.Mouse0) && activeThreads.Count == 0)
        {
            createNewThread(throwProjectile);
        }
        */

        //Exercise 3
        /*
        if (Input.GetKeyDown(KeyCode.Mouse0) && activeThreads.Count < 5)
        {
            createNewThread(throwProjectile);
        }
        */

        //Exercise 4
        if (Input.GetKeyDown(KeyCode.Mouse0) && activeThreads.Count < 5)
        {
            createNewThread(throwProjectile);
        }
    }

    void createNewThread(Action functionToExecute)
    {
        Thread thread = new Thread(new ThreadStart(functionToExecute));
        activeThreads.Add(thread);
        thread.Start();

        Debug.Log("New thread created");
    }

    void RemoveThread()
    {
        activeThreads.RemoveAt(0);
    }

    public void QueueMainThreadFunction(Action someFunctionWithNoParams)
    {
        //We need to make sure that someFunctionWithNoParams is running from the main thread
        functionsToRunInMainThread.Add(someFunctionWithNoParams);
    }


    GameObject createObject()
    {
        GameObject projectileInstance = Instantiate(projectile, transform.position, transform.rotation);
        projectileInstance.GetComponent<Rigidbody>().AddForce(transform.forward * throwForce);

        createdObjects.Add(projectileInstance);

        return projectileInstance;
    }

     void destroyFirstObject()
    {
        if(createdObjects.Count > 0)
        {
            GameObject firstGameObject = createdObjects[0];
            createdObjects.RemoveAt(0);

            Destroy(firstGameObject);
        }
    }

    void throwProjectile()
    {
        Action createAction = () =>
        {
            createObject();
        };

        QueueMainThreadFunction(createAction);

        //Exercise 1-3
        //Thread.Sleep(5000);

        //Exercise 4
        for(passedTime = 0; passedTime < cooldownTime * 1000; passedTime += 1.0f / activeThreads.Count)
        {
            Thread.Sleep(1);
        }

        Action destroyAction = () =>
        {
            destroyFirstObject();
        };

        QueueMainThreadFunction(destroyAction);

        RemoveThread();
    }
}
