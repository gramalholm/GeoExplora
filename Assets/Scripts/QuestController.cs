using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  // Importa o namespace necessário para manipulação de cenas

public class QuestController : MonoBehaviour
{
    public GameObject PainelQuestionario;  // Painel da UI
    public TMP_Text TextoQuestionario;  // Texto onde aparecerá a pergunta
    public Button Buttom1Quest, Buttom2Quest, Buttom3Quest;  // Botões das opções
    public GameObject BlocoQuest;  // O bloco que o jogador toca para ativar a quest (o "gatilho")

    private List<Question> perguntas;  
    // Lista de perguntas carregadas
    private bool jogadorDentro = false;

    void Start()
    {
        PainelQuestionario.SetActive(false); // Painel começa invisível
        CarregarPerguntas(); // Carrega as perguntas do JSON
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("Player") && !jogadorDentro)
        {
            jogadorDentro = true;
            AbrirQuestionario();
            BlocoQuest.SetActive(false);  // Desativa o bloco da quest depois que o jogador interage com ele
        }
    }

    void CarregarPerguntas()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("quests1"); // Carrega o JSON da pasta Resources
        if (jsonFile != null)
        {
            QuestionList lista = JsonUtility.FromJson<QuestionList>(jsonFile.text);
            perguntas = new List<Question>(lista.questions);
        }
        else
        {
            Debug.LogError("Arquivo JSON não encontrado!");
            perguntas = new List<Question>();
        }
    }

    void AbrirQuestionario()
    {
        if (perguntas.Count == 0)
        {
            Debug.LogError("Não há perguntas carregadas!");
            return;
        }

        PainelQuestionario.SetActive(true);
        // Seleciona uma pergunta aleatória
        Question perguntaAleatoria = perguntas[UnityEngine.Random.Range(0, perguntas.Count)];

        // Exibe o texto da pergunta
        TextoQuestionario.text = perguntaAleatoria.questionText;

        // Atribui as opções de resposta aos botões
        Buttom1Quest.GetComponentInChildren<TMP_Text>().text = perguntaAleatoria.options[0];
        Buttom2Quest.GetComponentInChildren<TMP_Text>().text = perguntaAleatoria.options[1];
        Buttom3Quest.GetComponentInChildren<TMP_Text>().text = perguntaAleatoria.options[2];

        // Remove eventuais listeners anteriores
        Buttom1Quest.onClick.RemoveAllListeners();
        Buttom2Quest.onClick.RemoveAllListeners();
        Buttom3Quest.onClick.RemoveAllListeners();

        // Adiciona listeners para verificar a resposta correta
        Buttom1Quest.onClick.AddListener(() => Responder(0 == perguntaAleatoria.correctIndex));
        Buttom2Quest.onClick.AddListener(() => Responder(1 == perguntaAleatoria.correctIndex));
        Buttom3Quest.onClick.AddListener(() => Responder(2 == perguntaAleatoria.correctIndex));
    }

    void Responder(bool correta)
    {
        if (correta)
        {
            Debug.Log("Resposta correta!");
        }
        else
        {
            Debug.Log("Resposta errada!");
            // Recarrega a cena atual para reiniciar a fase
            ReiniciarFase();
        }

        FecharQuestionario();
    }

    void ReiniciarFase()
    {
        // Recarrega a cena atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Reativa o bloco da quest após a reinicialização da fase
        BlocoQuest.SetActive(true);
    }

    void FecharQuestionario()
    {
        PainelQuestionario.SetActive(false);
    }
}
