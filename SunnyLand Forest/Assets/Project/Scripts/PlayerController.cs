 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator playerAnimator;
    private Rigidbody2D playerRigidbody2d;

    public Transform groundCheck;
    public bool isGround = false;

    public float speed;

    public float touchRun = 0.0f;

    public bool facingRight = true;

    public bool jump = false;
    public int numberJumps = 0;
    public int maximoJump = 2;
    public float jumpForce;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerRigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isGround = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        playerAnimator.SetBool("IsGrounded", isGround);
        touchRun = Input.GetAxisRaw("Horizontal");
        SetaMovimentos();

        if(Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    void FixedUpdate()
    {
        MovePlayer(touchRun);

        if(jump)
        {
            JumpPlayer();
        }
    }

    void MovePlayer(float movimentoH)
    {
         playerRigidbody2d.velocity = new Vector2(movimentoH * speed, playerRigidbody2d.velocity.y);

         if(movimentoH < 0 && facingRight)
         {
            Flip(); 
         }

         if(movimentoH > 0 && !facingRight)
         {
            Flip();
         }
    }

    void Flip()
    {
        facingRight = !facingRight;
        //Vector3 theScale = transform.localScale;
        //theScale.x *= -1;

        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.y);
    }

    void SetaMovimentos()
    {
        playerAnimator.SetBool("Walk", playerRigidbody2d.velocity.x != 0 && isGround);
        playerAnimator.SetBool("Jump", !isGround);
    }

      
    void JumpPlayer()
    {
        if(isGround)
        {
            numberJumps = 0;
        }

        if(isGround || numberJumps < maximoJump)
        {
            playerRigidbody2d.AddForce(new Vector2(0f, jumpForce));
            isGround = false;
            numberJumps++;
             
        }
        jump = false;
    }

}
