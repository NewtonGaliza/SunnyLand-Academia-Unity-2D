 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

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

    private ControllerGame _ControleGame;

    public AudioSource fxGame;
    public AudioClip fxPulo;

    private SpriteRenderer srPlayer;
    public int Vidas = 3;
    public Color hitColor;
    public Color noHitColor;
    private bool playerInvencivel;

    public GameObject playerDie;
    [SerializeField] ParticleSystem _poeira;


    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerRigidbody2d = GetComponent<Rigidbody2D>();

        _ControleGame = FindObjectOfType(typeof(ControllerGame)) as ControllerGame;

        srPlayer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        isGround = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        playerAnimator.SetBool("IsGrounded", isGround);
        
        //touchRun = Input.GetAxisRaw("Horizontal");
        /*
        if(Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
        */

        touchRun = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        
        SetaMovimentos();

        if(CrossPlatformInputManager.GetButtonDown("Jump"))
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
        CriarPoeira();
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
            CriarPoeira();
        }

        if(isGround || numberJumps < maximoJump)
        {
            playerRigidbody2d.AddForce(new Vector2(0f, jumpForce));
            isGround = false;
            numberJumps++;

            fxGame.PlayOneShot(fxPulo);
            CriarPoeira();             
        }
        jump = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Coletaveis":
                _ControleGame.Pontuacao(1);
                Destroy(collision.gameObject);
                break;

            case "Inimigo":
                GameObject tempExplosao = Instantiate(_ControleGame.hitPrefab, transform.position, transform.localRotation);
                Destroy(tempExplosao, 0.5f);

                Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(new Vector2(0, 400));
                _ControleGame.fxGame.PlayOneShot(_ControleGame.fxExplosao);

                Destroy(collision.gameObject);
                break;

            case "Damage":
                Hurt();
                break;
        }    
    }

    void OnCollisionEnter2D(Collision2D  collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Plataforma":
                //transformando o player em filho da plataforma para ele permanecer em cima dela
                this.transform.parent = collision.transform;
                break;

            case"Inimigo":
                Hurt();
                break;

        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Plataforma":
                this.transform.parent = null;
                break;
        }
        
    }

    void Hurt()
    {
        if(!playerInvencivel)
        {
            playerInvencivel = true;
            Vidas--;
            _ControleGame.BarraVida(Vidas);
            StartCoroutine("Dano");

            if(Vidas < 1)
            {
                //quaeternion.identity trava a rotacao do eixo Z
                GameObject pDieTemp = Instantiate(playerDie, transform.position, Quaternion.identity);
                Rigidbody2D rbDie = pDieTemp.GetComponent<Rigidbody2D>();
                rbDie.AddForce(new Vector2(150f, 500f));

                _ControleGame.fxGame.PlayOneShot(_ControleGame.fxDie);

                Invoke("CarregaJogo", 4f);

                //desabilitar o player na cena apos morrer
                gameObject.SetActive(false);
            }
        }
    }

    void CarregaJogo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator Dano()
    {
        srPlayer.color = noHitColor;
        yield return new WaitForSeconds(0.1f);

        for(float i = 0;i < 1; i += 0.1f)
        { 
            srPlayer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            srPlayer.enabled = true;
            yield return new WaitForSeconds(0.1f);

        }

        srPlayer.color = Color.white;
        playerInvencivel = false;
    }

    void CriarPoeira()
    {
        _poeira.Play();
    }

}
