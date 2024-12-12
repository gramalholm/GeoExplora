using UnityEngine;

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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<Animator>();
        respawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
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
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Respawn")
        {
            transform.position = respawnPoint;
        }
    }

}
