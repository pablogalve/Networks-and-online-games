using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IPunObservable
{
    PhotonView view;

    private Vector3 startPosition;
    private bool isHit = false;

    public GameObject destroyParticles;
    public GameObject hitParticles;

    private int maxLives = 5;
    private int _lives = 5;

    public bool canMove = true;

    public int lives
    {
        set
        {

            if (value > liveDisplays.Count && liveHolder != null)
            {
                for (int i = 0; i < value; i++)
                {
                    GameObject temp = Instantiate(livePrefabs, liveHolder.transform);
                    liveDisplays.Add(temp.GetComponent<Image>());
                }
            }

            _lives = value;

            for (int i = 0; i < liveDisplays.Count; i++)
            {
                if (i < _lives)
                {
                    liveDisplays[i].color = liveActive;
                }
                else
                {
                    liveDisplays[i].color = liveDisabled;
                }
            }
        }

        get
        {
            return _lives;
        }
    }

    [HideInInspector]
    public PlayerAttack playerAttack;

    public float movementSpeed = 5.0f;

    public Animator movementAnimator;
    public BoxCollider _collider;

    Vector2 colliderScreenSize;

    [Header("UI")]
    public GameObject liveHolder;
    public List<Image> liveDisplays;
    public GameObject livePrefabs;

    public Color liveActive;
    public Color liveDisabled;

    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private Vector3 movement;

    void Start()
    {
        startPosition = transform.position;
        playerAttack = GetComponent<PlayerAttack>();

        view = GetComponent<PhotonView>();

        if (_collider != null)
        {
            colliderScreenSize = (Camera.main.WorldToViewportPoint(_collider.bounds.center + _collider.size) - Camera.main.WorldToViewportPoint(_collider.center));
            //Debug.Log(colliderScreenSize.x.ToString());
        }

        if (view.IsMine)
        {
           liveHolder = MenuManager.instance.liveHolder;
           MenuManager.instance.TurnConnectUI_ON();
        }
        lives = 3;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
      
        if (stream.IsWriting)
        {
            stream.SendNext(this.transform.position);
            stream.SendNext(this.transform.rotation);
            stream.SendNext(movement);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            movement = (Vector3)stream.ReceiveNext();


            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkPosition += (movement * lag);
        }
    }

    public void Update()
    {
        if (view.IsMine && canMove)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 oldPos = transform.position;

            if (horizontal != 0.0f || vertical != 0.0f)
            {
                movementAnimator.SetFloat("blendTilt", vertical);

                Vector3 newPosition = transform.position + (new Vector3(horizontal, vertical, 0.0f) * movementSpeed * Time.deltaTime);

                float screenFinalPosX = Mathf.Clamp(Camera.main.WorldToViewportPoint(newPosition).x, 0.0f - (colliderScreenSize.x / 4f), 1.0f + (colliderScreenSize.x / 1.5f));
                float screenFinalPosY = Mathf.Clamp(Camera.main.WorldToViewportPoint(newPosition).y, 0.0f + colliderScreenSize.y * 5.0f, 1.0f - colliderScreenSize.y * 5.0f);

                transform.position = new Vector3(Camera.main.ViewportToWorldPoint(new Vector3(screenFinalPosX, 0.0f, 0.0f)).x, Camera.main.ViewportToWorldPoint(new Vector3(0.0f, screenFinalPosY, 0.0f)).y, 0.0f);
            }
            movement = (transform.position - oldPos).normalized;
        }
        else if(!view.IsMine)
        {
            transform.position = Vector3.MoveTowards(transform.position, networkPosition, Time.deltaTime * movementSpeed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, networkRotation, Time.deltaTime * 100);
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
        //Debug.Log("Current lives amount: " + lives.ToString());
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Projectile") || collision.gameObject.CompareTag("Missile"))
        {
            //PhotonNetwork.Instantiate(hitParticles.name, transform.position, Quaternion.identity);
            if (view != null && view.IsMine)
            {
                //DecreaseLives(1);
            }
        }
    }

    public void DecreaseLives(int amountToDecrease)
    {
        lives = Mathf.Clamp(lives - amountToDecrease, 0, lives);

        if (lives <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (view != null && view.IsMine)
        {
            PhotonNetwork.Instantiate(destroyParticles.name, transform.position, Quaternion.identity);
            transform.position = startPosition;
        }
    }

    public bool WasPlayerHit()
    {
        bool ret = isHit;
        isHit = false;
        return ret;
    }
}
