[System.Serializable]
public class Question
{
    public string questionText;  // Texto da pergunta
    public string[] options;     // Opções de resposta
    public int correctIndex;     // Índice da opção correta
}

[System.Serializable]


public class QuestionList
{
    public Question[] questions; // Lista de perguntas
}
