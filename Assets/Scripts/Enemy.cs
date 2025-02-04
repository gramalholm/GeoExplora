using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f; // Velocidade da abelha
    public static bool parado = false;
    public Vector2 pontoA;   // Coordenada do primeiro ponto
    public Vector2 pontoB;   // Coordenada do segundo ponto

    private Vector2 destino; // Próximo ponto da patrulha
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        destino = pontoB; // Começa indo para o ponto B
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update(){
      if (parado) return; // Se a variável for verdadeira, para o movimento

      // Move a abelha até o destino
      transform.position = Vector2.MoveTowards(transform.position, destino, speed * Time.deltaTime);

      // Se chegar perto do destino, alterna o destino
      if (Vector2.Distance(transform.position, destino) < 0.1f)
      {
          destino = (destino == pontoA) ? pontoB : pontoA;
          spriteRenderer.flipX = !spriteRenderer.flipX; // Inverte o sprite
      }
    }
}
