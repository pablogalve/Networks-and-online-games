using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkedObject
{
    private int maxLives = 5;
    private int lives = 5;

    [HideInInspector]
    public PlayerAttack playerAttack;

    public float movementSpeed = 5.0f;

    public Animator movementAnimator;
    public BoxCollider _collider;

    Vector2 colliderScreenSize;

    [SerializeField]
    private Client client;

    void Start()
    {
        networkedObjectType = NetworkedObjectType.PLAYER;

        lives = 1;
        playerAttack = GetComponent<PlayerAttack>();

        colliderScreenSize = (Camera.main.WorldToViewportPoint(_collider.bounds.center + _collider.size) - Camera.main.WorldToViewportPoint(_collider.center));

        Debug.Log(colliderScreenSize.x.ToString());
        //Debug.Log("sIZE IS: " + _collider.size + "and " + (_collider.bounds.center + new Vector3(0.0f, _collider.size.y, 0.0f)));
        //Debug.Log("This is stupid " + Camera.main.WorldToViewportPoint((_collider.bounds.center + new Vector3(0.0f, _collider.size.y, 0.0f))));
        //Debug.Log("Screen size is: " + Camera.main.WorldToViewportPoint(_collider.bounds.center + new Vector3(0.0f, _collider.size.y, 0.0f)));
        //Debug.Log("Center screen is: " + Camera.main.WorldToViewportPoint(_collider.bounds.center));
        //Debug.Log(Camera.main.WorldToViewportPoint(_collider.bounds.center + new Vector3(_collider.size.x, 0.0f, 0.0f)) - Camera.main.WorldToViewportPoint(_collider.bounds.center));

        StartCoroutine(SendCurrentPosition());
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0.0f || vertical != 0.0f)
        {
            movementAnimator.SetFloat("blendTilt", vertical);

            //backPoint = _collider.bounds.center - new Vector3(_collider.size.x, 0, 0);
            //frontPoint = _collider.bounds.center + new Vector3(_collider.size.x, 0, 0);
            //topPoint = _collider.bounds.center + new Vector3(0, _collider.size.y, 0);
            //downPoint = _collider.bounds.center - new Vector3(0, _collider.size.y, 0);


            //float corrX = 0.0f;
            //float corrY = 0.0f;
            //Vector3 finalPosition = transform.position;
            //if(vertical != 0.0f)
            //{
            //    if (CheckVerticalScreenPoint(Camera.main.WorldToViewportPoint(topPoint)))
            //    {
            //        finalPosition.y = topPoint.y - _collider.size.y;
            //        //Debug.Log("top out");
            //    }
            //    else if (CheckVerticalScreenPoint(Camera.main.WorldToViewportPoint(downPoint)))
            //    {
            //        finalPosition.y = downPoint.y + _collider.size.y;
            //        //Debug.Log("down out");
            //    }
            //    else
            //    {
            //        finalPosition.y += vertical * movementSpeed * Time.deltaTime;
            //    }
            //}
            //if(horizontal != 0.0f)
            //{
            //    if (CheckHorizontalScreenPoint(Camera.main.WorldToViewportPoint(frontPoint)))
            //    {
            //        finalPosition.x = frontPoint.x - _collider.size.x;
            //        //Debug.Log("front out");
            //    }
            //    else if (CheckHorizontalScreenPoint(Camera.main.WorldToViewportPoint(backPoint)))
            //    {
            //        finalPosition.x = backPoint.x + _collider.size.x;
            //        //Debug.Log("back out");
            //    }
            //    else
            //    {
            //        finalPosition.x += horizontal * movementSpeed * Time.deltaTime;
            //    }
            //}

            Vector3 newPosition = transform.position + (new Vector3(horizontal, vertical, 0.0f) * movementSpeed * Time.deltaTime);
            //Debug.Log(corrX + " // " + corrY);

            float screenFinalPosX = Mathf.Clamp(Camera.main.WorldToViewportPoint(newPosition).x, 0.0f - (colliderScreenSize.x / 4f), 1.0f + (colliderScreenSize.x / 1.5f));
            float screenFinalPosY = Mathf.Clamp(Camera.main.WorldToViewportPoint(newPosition).y, 0.0f + colliderScreenSize.y*5.0f, 1.0f - colliderScreenSize.y*5.0f);

            //Debug.Log(Camera.main.rect);
            transform.position = new Vector3(Camera.main.ViewportToWorldPoint(new Vector3(screenFinalPosX, 0.0f, 0.0f)).x, Camera.main.ViewportToWorldPoint(new Vector3(0.0f, screenFinalPosY, 0.0f)).y, 0.0f);

            //transform.position.x = Camera.main.ViewportToWorldPoint(new Vector3(screenFinalPosX, 0.0f, 0.0f)).x;
            //transform.position.y = Camera.main.ViewportToWorldPoint(new vector3(0.0f, screenFinalPosY, 0.0f)).y;
        }
    }

    IEnumerator SendCurrentPosition()
    {
        while(gameObject.activeSelf)
        {
            VectorMessage positionMessage = new VectorMessage(MessageType.OBJECT_POSITION, this.id.ToString(), transform.position + new Vector3(0.0f, -10.0f, 0.0f));
            client.Send(positionMessage);
            yield return new WaitForSeconds(client.secondsBetweenPlayerPositionUpdates);
        }
    }

    public bool CheckVerticalScreenPoint(Vector2 point)
    {
        return (point.y > 0.6f || point.y < 0f);
    }

    public bool CheckHorizontalScreenPoint(Vector2 point)
    {
        return (point.x < 0f || point.x > 1f);
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
