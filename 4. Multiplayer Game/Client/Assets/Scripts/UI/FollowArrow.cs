using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowArrow : MonoBehaviour
{
    public GameObject player;

    public float verticalOffset = 1.0f;

    private void Update()
    {
        gameObject.transform.position = player.transform.position + new Vector3(0.0f, verticalOffset, 0.0f);
    }
}
