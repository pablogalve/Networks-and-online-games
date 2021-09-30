using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;

public class ThreadQueuer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Start() -- Started.");
        functionsToRunInMainThread = new List<Action>();
        
        //StartThreadedFunction( () => { SlowFunctionThatDoesAUnityThing(Vector3.zero); } );
        //Debug.Log("Start() -- Done.");
    }

    void Update(){
        while(functionsToRunInMainThread.Count > 0){
            //Grab the first/oldest function in the list
            Action someFunc = functionsToRunInMainThread[0];
            functionsToRunInMainThread.RemoveAt(0);

            someFunc();
        }
    }

    List<Action> functionsToRunInMainThread;

    public void StartThreadedFunction( Action someFunctionWithNoParams){
        Thread t = new Thread ( new ThreadStart( someFunctionWithNoParams));
        t.Start();
    }

    public void QueueMainThreadFunction( Action someFunctionWithNoParams ){
        //We need to make sure that someFunctionWithNoParams is running from the main thread
        functionsToRunInMainThread.Add(someFunctionWithNoParams);
    }

    //This is an example, but this could be on any script
    void SlowFunctionThatDoesAUnityThing(Vector3 vec){
        //First we do a really slow thing
        Thread.Sleep(2000); //Sleep for 2 seconds

        //Now we need to modify a Unity gameobject
        Action aFunction = () => {
            //Debug.Log("The results of the child thread are being applied to a Unity GameObject safely.");
            this.transform.position = new Vector3(1,1,1);
        };
        
        QueueMainThreadFunction( aFunction );
    }
}