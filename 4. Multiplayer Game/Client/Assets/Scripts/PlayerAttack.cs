using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    PhotonView view;

    public float defaultTimeBetweenShots;

    private Player player;
   

    [SerializeField]
    private GameObject shootPoint;

    [HideInInspector]
    public float timeBetweenShots;

    [HideInInspector]
    public float shotsTimer = 0.0f;

    public GameObject projectile;

    void Start()
    {
        timeBetweenShots = defaultTimeBetweenShots;
        
        player = GetComponent<Player>();
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(view.IsMine)
        {
            if (shotsTimer > 0.0f)
            {
                shotsTimer -= Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Space) && shotsTimer <= 0.0f)
            {
                PhotonNetwork.Instantiate(projectile.name, shootPoint.transform.position, Quaternion.identity);
            }
        }
    }
}
