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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        direction = Input.GetAxis("Horizontal");
        if(direction > 0f){
            player.linearVelocity = new Vector2(direction*speed, player.linearVelocity.y);
        }else if(direction < 0f){
            player.linearVelocity = new Vector2(direction * speed, player.linearVelocity.y);
        }
        else{
            player.linearVelocity = new Vector2(0, player.linearVelocity.y);
        }

        if(Input.GetButtonDown("Jump")){
            player.linearVelocity = new Vector2(player.linearVelocity.x, jumpSpeed);
        }
    }
}
