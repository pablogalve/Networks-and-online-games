using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkedObject
{
    public GameObject destroyParticles;

    public bool isPlayerContolled = false;

    private int maxLives = 5;
    private int _lives = 5;
    public int lives
    {
        set
        {

            if (value > liveDisplays.Count && isPlayerContolled == true)
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

    [SerializeField]
    private Client client;

    [Header("UI")]
    public GameObject liveHolder;
    public List<Image> liveDisplays;
    public GameObject livePrefabs;

    public Color liveActive;
    public Color liveDisabled;

    void Start()
    {
        networkedObjectType = NetworkedObjectType.PLAYER;

        lives = 3;
        playerAttack = GetComponent<PlayerAttack>();

        if (_collider != null)
        {
            colliderScreenSize = (Camera.main.WorldToViewportPoint(_collider.bounds.center + _collider.size) - Camera.main.WorldToViewportPoint(_collider.center));
            Debug.Log(colliderScreenSize.x.ToString());
        }
    }

    public override void Update()
    {
        base.Update();

        if(isPlayerContolled)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (horizontal != 0.0f || vertical != 0.0f)
            {
                movementAnimator.SetFloat("blendTilt", vertical);

                Vector3 newPosition = transform.position + (new Vector3(horizontal, vertical, 0.0f) * movementSpeed * Time.deltaTime);
                //Debug.Log(corrX + " // " + corrY);

                float screenFinalPosX = Mathf.Clamp(Camera.main.WorldToViewportPoint(newPosition).x, 0.0f - (colliderScreenSize.x / 4f), 1.0f + (colliderScreenSize.x / 1.5f));
                float screenFinalPosY = Mathf.Clamp(Camera.main.WorldToViewportPoint(newPosition).y, 0.0f + colliderScreenSize.y * 5.0f, 1.0f - colliderScreenSize.y * 5.0f);

                //Debug.Log(Camera.main.rect);
                transform.position = new Vector3(Camera.main.ViewportToWorldPoint(new Vector3(screenFinalPosX, 0.0f, 0.0f)).x, Camera.main.ViewportToWorldPoint(new Vector3(0.0f, screenFinalPosY, 0.0f)).y, 0.0f);
            }
        }
    }

    public void ActivatePlayer()
    {
        _collider.enabled = true;
        isPlayerContolled = true;

        StartCoroutine(SendCurrentPosition());
    }

    IEnumerator SendCurrentPosition()
    {
        while (gameObject.activeSelf)
        {
            VectorMessage positionMessage = new VectorMessage(MessageType.PLAYER_POSITION, this.id, transform.position);
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

        if (lives <= 0 && isPlayerContolled)
        {
            Die();
        }
    }

    public override void Die()
    {
        base.Die();

        SpawnParticles(destroyParticles);

        //TODO: Add condition to check if is player
        if (isPlayerContolled)
        {
            Client client = udpObject as Client;
            IdMessage deathMessage = new IdMessage(MessageType.PLAYER_DEATH, id);
            client.SendMessage(deathMessage);
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile") || collision.gameObject.CompareTag("PowerUp"))
        {
            base.OnCollisionEnter(collision);
        }
    }
}
