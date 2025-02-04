using System.Collections;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Multiplayer.Center.Common;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip walkSound;
    public float speed = 4f;
    public float jumpSpeed = 8f;
    private float direction = 0f;

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool isTouchingGround;
    private Rigidbody2D player;

    private Animator playerAnimation;

    private Vector3 respawnPoint;
    public GameObject fallDetector;

    private int respostasCorretas = 0;
    private int score;
    private bool isPlayerDead;
    private bool isMoving = false;

    public Text scoreText;

    public static PlayerMovement instance {  get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        player = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<Animator>();
        respawnPoint = transform.position;
        scoreText.text = "Placar: " + Scoring.totalScore;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (isPlayerDead) return;

        isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        direction = Input.GetAxis("Horizontal");

        if (direction != 0f)
        {
            player.linearVelocity = new Vector2(direction * speed, player.linearVelocity.y);

            // Só toca o som se ele ainda não estiver tocando
            if (!isMoving)
            {
                AudioManager.instance.playSFX(walkSound);
                isMoving = true; // Marca que o som começou
            }

            // Inverte o sprite apenas se a direção mudar
            if (direction > 0)
                transform.localScale = new Vector2(1f, 1f);
            else
                transform.localScale = new Vector2(-1f, 1f);
        }
        else
        {
            player.linearVelocity = new Vector2(0, player.linearVelocity.y);
            isMoving = false; // Marca que o jogador parou
        }

        if (Input.GetButtonDown("Jump") && isTouchingGround)
        {
            AudioManager.instance.playSFX(jumpSound);
            player.linearVelocity = new Vector2(player.linearVelocity.x, jumpSpeed);
        }

        playerAnimation.SetFloat("speed", Mathf.Abs(player.linearVelocity.x));
        playerAnimation.SetBool("onGround", isTouchingGround);

        fallDetector.transform.position = new Vector2(transform.position.x, fallDetector.transform.position.y);
    }
    public void VerificarResposta(bool correta)
    {
        if (correta)
        {
            respostasCorretas++;
            scoreText.text = "Placar: " + Scoring.totalScore;
        }
    }

    private void IrParaProximaFase()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Respawn")
        { 
            transform.position = respawnPoint;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "FallDetector")
        {
            transform.position = respawnPoint;
        }
        else if (collision.tag == "Finish")
        {
            // Passa de fase se tiver 3 respostas corretas
            if (respostasCorretas >= 3)
            {
               IrParaProximaFase();
               respawnPoint = transform.position;
            }
        }
    }

}
