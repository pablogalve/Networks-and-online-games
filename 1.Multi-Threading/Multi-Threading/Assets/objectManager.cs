using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectManager : MonoBehaviour
{
    public GameObject cube;
    public float cooldown = 5.0f;
    private float cooldownTimer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        cooldownTimer -= Time.deltaTime;
        
        if(Input.GetKeyDown("space")){
            if(cooldownTimer <= 0.0f){
                //ThreadQueuer.StartThreadedFunction( CreateCube );   
                CreateCube();             
                cooldownTimer = cooldown;
            }else{
                Debug.Log("You can only have 1 object at the same time");
            }            
        }
    }

    void CreateCube(){
        Object.Instantiate(cube);
    }
}
