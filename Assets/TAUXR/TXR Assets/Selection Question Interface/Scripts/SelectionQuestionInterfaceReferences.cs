using UnityEngine;

[System.Serializable]
public class SelectionQuestionInterfaceReferences
{
    public TextDisplay QuestionTextDisplay => _questionTextDisplay;
    public TextDisplay AnswerInfo => _answerInfo;
    public SelectionQuestionSubmitButton SubmitButton => _submitButton;
    public SelectionAnswerButton[] SelectionAnswers => _selectionAnswers;
    public GameObject Graphics => _graphics;

    public AudioSource CorrectAnswerAudio => _correctAnswer;
    public AudioSource WrongAnswerAudio => _wrongAnswer;

    [SerializeField] private TextDisplay _questionTextDisplay;
    [SerializeField] private TextDisplay _answerInfo;
    [SerializeField] private SelectionQuestionSubmitButton _submitButton;
    [SerializeField] private SelectionAnswerButton[] _selectionAnswers;
    [SerializeField] private GameObject _graphics;
    [SerializeField] private AudioSource _correctAnswer;
    [SerializeField] private AudioSource _wrongAnswer;
}