using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCamera : MonoBehaviour
{
    public float offsetX = 3f;
    public float smooth = 0.1f;

    public float limiteUp = 2f;
    public float limiteDown = 0f;
    public float limiteLeft = 0f;
    public float limiteRight = 100f;

    private Transform player;
    private float playerX;
    private float playerY;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(player != null)
        {
            playerX = Mathf.Clamp(player.position.x + offsetX, limiteLeft, limiteRight);
            playerY = Mathf.Clamp(player.position.y , limiteDown, limiteUp);

             transform.position = Vector3.Lerp(transform.position, new Vector3(playerX, playerY, transform.position.z), smooth);


        }
    }
}
