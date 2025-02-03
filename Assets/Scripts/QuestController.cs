using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class QuestController : MonoBehaviour
{
    public GameObject PainelQuestionario;  // Painel da UI
    public TMP_Text TextoQuestionario;  // Texto onde aparecerá a pergunta
    public Button Buttom1Quest, Buttom2Quest, Buttom3Quest;  // Botões das opções
    public GameObject BlocoQuest; // O bloco que o jogador toca para ativar a quest (o "gatilho")
    public GameObject PainelBackground;  // Painel de fundo para a quest
    public GameObject PainelQuest;  // Painel da quest

    private List<Question> perguntas;  // Lista de perguntas
    private List<Question> perguntasRespondidas;  // Lista para armazenar as perguntas já feitas
    private bool jogadorDentro = false;
    public static bool rightAnswer = true;

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

    void AbrirQuestionario()
    {
        if (perguntas.Count == 0)
        {
            Debug.LogError("Não há perguntas carregadas!");
        }

        // Se todas as perguntas foram feitas, reorganize e reinicie o ciclo
        if (perguntasRespondidas.Count == perguntas.Count)
        {
            perguntasRespondidas.Clear();
            EmbaralharPerguntas();  // Reembaralha as perguntas quando todas tiverem sido feitas
        }

        PainelQuestionario.SetActive(true);
        PainelBackground.SetActive(true);
        PainelQuest.SetActive(true);

        // Seleciona a próxima pergunta não respondida
        Question perguntaAtual = PerguntaNaoRespondida();

        // Exibe o texto da pergunta
        TextoQuestionario.text = perguntaAtual.questionText;

        // Atribui as opções de resposta aos botões
        Buttom1Quest.GetComponentInChildren<TMP_Text>().text = perguntaAtual.options[0];
        Buttom2Quest.GetComponentInChildren<TMP_Text>().text = perguntaAtual.options[1];
        Buttom3Quest.GetComponentInChildren<TMP_Text>().text = perguntaAtual.options[2];

        // Remove eventuais listeners anteriores
        Buttom1Quest.onClick.RemoveAllListeners();
        Buttom2Quest.onClick.RemoveAllListeners();
        Buttom3Quest.onClick.RemoveAllListeners();

        // Adiciona listeners para verificar a resposta correta
        Buttom1Quest.onClick.AddListener(() => Responder(0 == perguntaAtual.correctIndex));
        Buttom2Quest.onClick.AddListener(() => Responder(1 == perguntaAtual.correctIndex));
        Buttom3Quest.onClick.AddListener(() => Responder(2 == perguntaAtual.correctIndex));
    }

    Question PerguntaNaoRespondida()
    {
        // Seleciona a primeira pergunta não respondida
        foreach (var pergunta in perguntas)
        {
            if (!perguntasRespondidas.Contains(pergunta))
            {
                return pergunta;
            }
        }
        return null; // Retorna null caso todas as perguntas tenham sido feitas (não esperado)
    }

    void Responder(bool correta)
    {
        if (correta)
        {
            Debug.Log("Resposta correta!");
            rightAnswer = true;
        }
        else
        {
            Debug.Log("Resposta errada!");
            // Recarrega a cena atual para reiniciar a fase
            ReiniciarFase();
            rightAnswer = false;
        }

        // Adiciona a pergunta à lista de perguntas respondidas
        perguntasRespondidas.Add(PerguntaNaoRespondida());

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
        PainelBackground.SetActive(false);
        PainelQuest.SetActive(false);
    }
}
