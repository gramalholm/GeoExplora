using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class QuestController : MonoBehaviour
{
    [SerializeField] AudioClip coinSound;
    [SerializeField] AudioClip correctSound;
    [SerializeField] AudioClip wrongSound;
    public GameObject PainelQuestionario;  // Painel da UI
    public TMP_Text TextoQuestionario;  // Texto onde aparecerá a pergunta
    public Button Buttom1Quest, Buttom2Quest, Buttom3Quest;  // Botões das opções
    public GameObject BlocoQuest; // O bloco que o jogador toca para ativar a quest (o "gatilho")
    public GameObject PainelBackground;  // Painel de fundo para a quest
    public GameObject PainelQuest;  // Painel da quest
    private Question perguntaAtual;

    private List<Question> perguntas;  // Lista de perguntas
    private List<Question> perguntasRespondidas;  // Lista para armazenar as perguntas já feitas
    private bool jogadorDentro = false;
    public bool rightAnswer = true;

    public string nomeArquivoQuestao = "quests1";  // Valor inicial (pode ser alterado dinamicamente)

    void Start()
    {
        PainelQuestionario.SetActive(false); // Painel começa invisível
        PainelBackground.SetActive(false); // Painel de fundo começa invisível
        PainelQuest.SetActive(false); // Painel da quest começa invisível
        perguntasRespondidas = new List<Question>(); // Lista para armazenar perguntas realizadas
        CarregarPerguntas(nomeArquivoQuestao); // Carrega as perguntas do arquivo JSON
        EmbaralharPerguntas();  // Embaralha as perguntas para começar de forma aleatória
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("Player") && !jogadorDentro)
        {
            AudioManager.instance.playSFX(coinSound);
            jogadorDentro = true;
            AbrirQuestionario();
            BlocoQuest.SetActive(false);  // Desativa o bloco da quest depois que o jogador interage com ele
        }
    }

    void CarregarPerguntas(string nomeArquivo)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(nomeArquivo); // Carrega o JSON da pasta Resources com o nome do arquivo recebido
        if (jsonFile != null)
        {
            QuestionList lista = JsonUtility.FromJson<QuestionList>(jsonFile.text);
            perguntas = new List<Question>(lista.questions); // Carrega todas as perguntas na lista
        }
        else
        {
            Debug.LogError("Arquivo JSON não encontrado!");
        }
    }

    void EmbaralharPerguntas()
    {
        // Embaralha a lista de perguntas para garantir que a ordem seja aleatória
        for (int i = 0; i < perguntas.Count; i++)
        {
            Question temp = perguntas[i];
            int randomIndex = Random.Range(i, perguntas.Count);
            perguntas[i] = perguntas[randomIndex];
            perguntas[randomIndex] = temp;
        }
    }

Question PerguntaNaoRespondida()
{
    // Filtra as perguntas que ainda não foram respondidas
    List<Question> perguntasNaoRespondidas = perguntas.FindAll(p => !perguntasRespondidas.Contains(p));

    // Se todas as perguntas já foram feitas, retorna null (o que não deveria ocorrer, pois já limpamos antes)
    if (perguntasNaoRespondidas.Count == 0)
    {
        return null;
    }

    // Retorna uma pergunta aleatória entre as não respondidas
    return perguntasNaoRespondidas[Random.Range(0, perguntasNaoRespondidas.Count)];
}

    void AbrirQuestionario()
{
        if (perguntas.Count == 0)
        {
            Debug.LogError("Não há perguntas carregadas!");
            return;
        }

        if (perguntasRespondidas.Count == perguntas.Count)
        {
            perguntasRespondidas.Clear();
            EmbaralharPerguntas();
        }

        // Evita repetir a última pergunta feita antes de reiniciar
        string ultimaPergunta = PlayerPrefs.GetString("ultimaPergunta", "");
        do
        {
            perguntaAtual = PerguntaNaoRespondida();
        } while (perguntaAtual != null && perguntaAtual.questionText == ultimaPergunta);

        // Exibe a pergunta e as opções
        TextoQuestionario.text = perguntaAtual.questionText;
        Buttom1Quest.GetComponentInChildren<TMP_Text>().text = perguntaAtual.options[0];
        Buttom2Quest.GetComponentInChildren<TMP_Text>().text = perguntaAtual.options[1];
        Buttom3Quest.GetComponentInChildren<TMP_Text>().text = perguntaAtual.options[2];

        Buttom1Quest.onClick.RemoveAllListeners();
        Buttom2Quest.onClick.RemoveAllListeners();
        Buttom3Quest.onClick.RemoveAllListeners();

        Buttom1Quest.onClick.AddListener(() => Responder(0 == perguntaAtual.correctIndex));
        Buttom2Quest.onClick.AddListener(() => Responder(1 == perguntaAtual.correctIndex));
        Buttom3Quest.onClick.AddListener(() => Responder(2 == perguntaAtual.correctIndex));

        // Ativa os painéis
        EnemyPatrol.parado = true;
        PainelQuestionario.SetActive(true);
        PainelBackground.SetActive(true);
        PainelQuest.SetActive(true);
}


    void Responder(bool correta)
{
        if (correta)
        {
            AudioManager.instance.playSFX(correctSound);
            Debug.Log("Resposta correta!");
            Scoring.isRightAnswer = true;
            Scoring.totalScore += 10;
        }
        else
        {
            AudioManager.instance.playSFX(wrongSound);
            Debug.Log("Resposta errada!");
            Scoring.isRightAnswer = false;
            Scoring.wrongAnswers++;
            Scoring.totalScore -= 5;
            // Recarrega a cena atual para reiniciar a fase
            ReiniciarFase();
        }

        // Adiciona a pergunta respondida à lista
        if (perguntaAtual != null)
        {
            perguntasRespondidas.Add(perguntaAtual);
            PlayerPrefs.SetString("ultimaPergunta", perguntaAtual.questionText);
            PlayerPrefs.Save();
        }
        PlayerMovement.instance.VerificarResposta(correta);

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
        EnemyPatrol.parado = false;
        PainelQuestionario.SetActive(false);
        PainelBackground.SetActive(false);
        PainelQuest.SetActive(false);
    }
}
