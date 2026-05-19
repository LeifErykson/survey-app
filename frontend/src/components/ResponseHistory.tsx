import React, { useEffect, useState } from 'react';
import { surveysApi } from '../services/api';

interface AnswerDetail {
  questionId: number;
  questionText: string;
  answerText: string;
}

interface ResponseHistoryItem {
  filledSurveyId: number;
  surveyId: number;
  surveyTitle: string;
  submittedAt: string;
  answers: AnswerDetail[];
}

const ResponseHistory: React.FC = () => {
  const [responses, setResponses] = useState<ResponseHistoryItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [expandedId, setExpandedId] = useState<number | null>(null);

  useEffect(() => {
    loadResponses();
  }, []);

  const loadResponses = async () => {
    try {
      const response = await surveysApi.getMyResponses();
      setResponses(response.data);
    } catch (error) {
      console.error('Error loading responses:', error);
    } finally {
      setLoading(false);
    }
  };

  const toggleExpand = (id: number) => {
    setExpandedId(expandedId === id ? null : id);
  };

  if (loading) return <div>Loading your responses...</div>;

  if (responses.length === 0) {
    return <div>You haven't taken any surveys yet. Go to Browse Surveys to get started!</div>;
  }

  return (
    <div>
      <h2>My Survey Responses</h2>
      {responses.map((response) => (
        <div
          key={response.filledSurveyId}
          style={{
            border: '1px solid #ddd',
            marginBottom: '15px',
            padding: '15px',
            borderRadius: '5px'
          }}
        >
          <div
            style={{
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center',
              cursor: 'pointer'
            }}
            onClick={() => toggleExpand(response.filledSurveyId)}
          >
            <div>
              <strong>{response.surveyTitle}</strong>
              <br />
              <small>Submitted: {new Date(response.submittedAt).toLocaleString()}</small>
            </div>
            <button>{expandedId === response.filledSurveyId ? 'Hide Answers' : 'Show Answers'}</button>
          </div>
          
          {expandedId === response.filledSurveyId && (
            <div style={{ marginTop: '15px', borderTop: '1px solid #eee', paddingTop: '10px' }}>
              <h4>Your Answers:</h4>
              <ul>
                {response.answers.map((answer, idx) => (
                  <li key={answer.questionId} style={{ marginBottom: '10px' }}>
                    <strong>{idx + 1}. {answer.questionText}</strong>
                    <br />
                    <span style={{ color: '#555' }}>Answer: {answer.answerText}</span>
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>
      ))}
    </div>
  );
};

export default ResponseHistory;