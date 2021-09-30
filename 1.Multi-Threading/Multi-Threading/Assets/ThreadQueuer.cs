using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;

public class ThreadQueuer : MonoBehaviour
{
    private static int maxThreads = 5;
    private static List<Thread> activeThreads;

    static List<Action> functionsToRunInMainThread;
    static List<Action> functionsToRunInSecondaryThread;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Start() -- Started.");
        functionsToRunInMainThread = new List<Action>();
        functionsToRunInSecondaryThread = new List<Action>();

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
    

    public static void StartThreadedFunction( Action someFunctionWithNoParams){
        Debug.Log("StartThreadedFunction() -- Started.");
        if(activeThreads.Count >= maxThreads){
            
        }else{
            Thread t = new Thread ( new ThreadStart( someFunctionWithNoParams));
            t.Start();
            activeThreads.Add(t);
        }        
    }


    public static void QueueMainThreadFunction( Action someFunctionWithNoParams ){
        //We need to make sure that someFunctionWithNoParams is running from the main thread
        functionsToRunInMainThread.Add(someFunctionWithNoParams);
    }

    public static void QueueSecondaryThreadFunction( Action someFunctionWithNoParams ){
        functionsToRunInSecondaryThread.Add(someFunctionWithNoParams);
    }






    // ----- END OF SCRIPT -----

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