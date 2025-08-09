using UnityEngine;

[System.Serializable]
public class SQIReferences
{
  //  public TextDisplay QuestionTextDisplay => _questionTextDisplay;
    public TextDisplay AnswerInfo => _answerInfo;
    public SQISubmitButton SubmitButton => _submitButton;
    public SQIAnswerButton[] SelectionAnswers => _selectionAnswers;
    public AudioSource CorrectAnswerAudio => _correctAnswer;
    public AudioSource WrongAnswerAudio => _wrongAnswer;
    public TXRRadioButtonGroup RadioButtonGroup => _radioButtonGroup;

    [SerializeField] private SQISubmitButton _submitButton;
    //[SerializeField] private TextDisplay _questionTextDisplay;
    [SerializeField] private SQIAnswerButton[] _selectionAnswers;
    [SerializeField] private TXRRadioButtonGroup _radioButtonGroup;
    [SerializeField] private TextDisplay _answerInfo;
    [SerializeField] private AudioSource _correctAnswer;
    [SerializeField] private AudioSource _wrongAnswer;

    public void InitAnswerArray(SQIAnswerButton[] answers)
    {
        _selectionAnswers= answers;
    }
}