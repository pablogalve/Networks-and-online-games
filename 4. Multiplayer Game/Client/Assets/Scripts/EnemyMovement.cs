using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    //Vehicle Movement
    private float maxHeight = 1.0f;
    private float verticalSpeed = 0.5f;
    private bool goingUp = true;
    private float currPercentageOfAnimation = 0.0f;
    private Vector3 initialPos;

    //Vehicle Rotation
    private float currRotation = 0.0f;
    private float maxRotation;

    public void Start()
    {
        initialPos = gameObject.transform.localPosition;
        SetVehicleProperties();
        SetRandomInitialValues();
    }

    public void Move()
    {
        if (goingUp) currPercentageOfAnimation += Time.deltaTime * verticalSpeed;
        else currPercentageOfAnimation -= Time.deltaTime * verticalSpeed;

        if (currPercentageOfAnimation > 1.0f) goingUp = false;
        else if (currPercentageOfAnimation < 0.0f) goingUp = true;

        VehicleRotation();
        VehicleMovement();
    }

    public float ParametricBlend(float t) => ((t * t) / (2.0f * ((t * t) - t) + 1.0f));

    private float GetRandomValue(float min, float max)
    {
        System.Random random = new System.Random();
        double val = (random.NextDouble() * (max - min) + min);
        return (float)val;
    }

    private void VehicleRotation()
    {
        if (currPercentageOfAnimation <= 0.5f)
        {
            currRotation = currPercentageOfAnimation * 2.0f * maxRotation;
        }
        else
        {
            float localPercentage = (currPercentageOfAnimation - 0.5f) * 2.0f;
            currRotation = -1.0f * (1.0f - localPercentage) * maxRotation;
        }

        gameObject.transform.Rotate(0,0, currRotation, Space.Self);
    }

    private void VehicleMovement()
    {
        float yPos = ParametricBlend(currPercentageOfAnimation);
        Vector3 newPos = new Vector3(initialPos.x, initialPos.y, initialPos.z);
        newPos.y += yPos * maxHeight;

        gameObject.transform.localPosition = newPos;
    }

    private void SetVehicleProperties()
    {       
        maxHeight = 5.0f;
        verticalSpeed = 0.5f;
        maxRotation = 0.05f;                
    }

    private void SetRandomInitialValues()
    {
    }
}
