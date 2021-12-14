using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int maxLives = 5;
    private int lives = 5;

    [HideInInspector]
    public PlayerAttack playerAttack;

    public float movementSpeed = 5.0f;

    public Animator movementAnimator;
    public BoxCollider _collider;

    private Plane[] planes;

    void Start()
    {
        lives = 1;
        playerAttack = GetComponent<PlayerAttack>();

        planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0.0f || vertical != 0.0f)
        {

            //if(vertical != 0.0f && Mathf.Abs(transform.eulerAngles.x) < 38)
            //{
            //    transform.Rotate(transform.right, 100 * -vertical * Time.deltaTime);
            //}
            //else if(Mathf.Abs(transform.eulerAngles.x) < 38)
            //{
            //    transform.Rotate(transform.right, 100 * -vertical * Time.deltaTime);
            //}
            movementAnimator.SetFloat("blendTilt", vertical);



            //if (GeometryUtility.TestPlanesAABB(planes, _collider.bounds))
            //{
            //    transform.Translate(new Vector3(horizontal, vertical, 0.0f) * movementSpeed * Time.deltaTime, Space.World);
            //}

            Vector3 screenPosition = Camera.main.WorldToViewportPoint(transform.position + (new Vector3(horizontal, vertical, 0.0f) * movementSpeed * Time.deltaTime));
            if (screenPosition.x < 0f || screenPosition.x > 1f || screenPosition.y > 1f || screenPosition.y < 0f)
            {
                //Cant move, we would go offscreen
            }
            else
            {
                //Can move, the position is on screen
                transform.Translate(new Vector3(horizontal, vertical, 0.0f) * movementSpeed * Time.deltaTime, Space.World);
            }



        }
    }

    public void IncreaseLives(int amountToIncrease)
    {
        lives = Mathf.Clamp(lives + amountToIncrease, lives, maxLives);
        Debug.Log("Current lives amount: " + lives.ToString());
    }

    public void DecreaseLives(int amountToDecrease)
    {
        lives = Mathf.Clamp(lives - amountToDecrease, 0, lives);
    }
}
