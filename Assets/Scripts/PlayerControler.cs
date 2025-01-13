using System.Collections;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
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

    private int questõesCertas;
    private int score;
    private bool isPlayerDead;

    public Text scoreText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        if (direction > 0f)
        {
            player.linearVelocity = new Vector2(direction * speed, player.linearVelocity.y);
        }
        else if (direction < 0f)
        {
            player.linearVelocity = new Vector2(direction * speed, player.linearVelocity.y);
            transform.localScale = new Vector2(-1f, 1f);
        }
        else
        {
            player.linearVelocity = new Vector2(0, player.linearVelocity.y);
            transform.localScale = new Vector2(1f, 1f);
        }

        if (Input.GetButtonDown("Jump") && isTouchingGround)
        {
            player.linearVelocity = new Vector2(player.linearVelocity.x, jumpSpeed);
        }

        playerAnimation.SetFloat("speed", Mathf.Abs(player.linearVelocity.x));
        playerAnimation.SetBool("onGround", isTouchingGround);

        fallDetector.transform.position = new Vector2(transform.position.x, fallDetector.transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "FallDetector")
        {
            transform.position = respawnPoint;
        }else if(collision.tag == "Finish")
        {
            //quando adicionarmos mais níveis, criar um algoritmo para escolher aleatóriamente o level do player
            if(questõesCertas == 3)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                respawnPoint = transform.position;
            }
            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Respawn")
        { 
            Scoring.totalScore -= 5;
            transform.position = respawnPoint;
        }

        if (collision.gameObject.tag == "Quest")
        {
            questõesCertas += 1;
            Scoring.totalScore += 20;
            scoreText.text = "Placar: " + Scoring.totalScore;
            Debug.Log(questõesCertas);
        }
    }
}
