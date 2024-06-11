using System.Collections.Generic;

namespace Glitch9.Game.Dialogue
{
    public class QuestionnaireHandler
    {
        private readonly Dictionary<string /* questionId */, int /* answerIndex */> _questionnaireAnswers = new();
        public bool IsQuestionnaireAvailable => _questionnaireAnswers.Count > 0;
        

        public void SetAnswer(string questionId, int answerIndex)
        {
            if (_questionnaireAnswers.ContainsKey(questionId))
            {
                _questionnaireAnswers[questionId] = answerIndex;
            }
            else
            {
                _questionnaireAnswers.Add(questionId, answerIndex);
            }
        }

        // TODO: get results
        // How will this handle results and apply them to the User data?

    }
}