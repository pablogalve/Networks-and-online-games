using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cube : MonoBehaviour
{
    public float timer = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {        
        Move();
        if(timer <= 0.0f){            
            Destroy(this.gameObject);
        }
        timer -= Time.deltaTime;
    }

    void Move(){
        transform.Translate(Vector3.up * Time.deltaTime);
    }
}
