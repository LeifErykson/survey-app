import React, { useEffect, useState } from 'react';
import { surveysApi } from '../services/api';

interface AnswerStats {
  answerId: number;
  answerText: string;
  count: number;
  percentage: number;
}

interface QuestionStats {
  questionId: number;
  questionText: string;
  questionType: string;
  answerStats: AnswerStats[];
}

interface SurveyData {
  surveyId: number;
  surveyTitle: string;
  totalResponses: number;
  questions: QuestionStats[];
}

interface SurveyStatsProps {
  surveyId: number;
  surveyTitle: string;
  onClose: () => void;
}

const SurveyStats: React.FC<SurveyStatsProps> = ({ surveyId, surveyTitle, onClose }) => {
  const [stats, setStats] = useState<SurveyData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
  const loadStats = async () => {
    try {
      const response = await surveysApi.getSurveyStats(surveyId);
      setStats(response.data);
    } catch (err: any) {
      console.error('Error loading stats:', err);
      setError(err.response?.data || 'Failed to load statistics');
    } finally {
      setLoading(false);
    }
  };
  
  loadStats();
}, [surveyId]);

  if (loading) return <div style={{ padding: '20px' }}>Loading statistics...</div>;
  if (error) return <div style={{ padding: '20px', color: 'red' }}>{error}</div>;
  if (!stats) return <div style={{ padding: '20px' }}>No data available</div>;

  return (
    <div style={{
      position: 'fixed',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: 'rgba(0,0,0,0.5)',
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      zIndex: 9999
    }}>
      <div style={{
        backgroundColor: 'white',
        padding: '20px',
        borderRadius: '8px',
        maxWidth: '600px',
        maxHeight: '80vh',
        overflow: 'auto',
        width: '90%',
        boxShadow: '0 4px 6px rgba(0,0,0,0.1)'
      }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
          <h2>{stats.surveyTitle} - Statistics</h2>
          <button onClick={onClose} style={{ padding: '5px 10px', cursor: 'pointer' }}>Close</button>
        </div>
        
        <p><strong>Total Responses:</strong> {stats.totalResponses}</p>
        
        {stats.questions.map((question, qIndex) => (
          <div key={question.questionId} style={{ marginBottom: '25px', border: '1px solid #ddd', padding: '15px', borderRadius: '5px' }}>
            <h3>Question {qIndex + 1}: {question.questionText}</h3>
            <p><small>Type: {question.questionType === 'single' ? 'Single Choice' : 'Multiple Choice'}</small></p>
            
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr>
                  <th style={{ textAlign: 'left', borderBottom: '1px solid #ddd', padding: '8px' }}>Answer</th>
                  <th style={{ textAlign: 'center', borderBottom: '1px solid #ddd', padding: '8px' }}>Count</th>
                  <th style={{ textAlign: 'center', borderBottom: '1px solid #ddd', padding: '8px' }}>Percentage</th>
                </tr>
              </thead>
              <tbody>
                {question.answerStats.map((answer) => (
                  <tr key={answer.answerId}>
                    <td style={{ padding: '8px', borderBottom: '1px solid #eee' }}>{answer.answerText}</td>
                    <td style={{ textAlign: 'center', padding: '8px', borderBottom: '1px solid #eee' }}>{answer.count}</td>
                    <td style={{ textAlign: 'center', padding: '8px', borderBottom: '1px solid #eee' }}>
                      <div>
                        <div style={{
                          display: 'inline-block',
                          width: `${answer.percentage}%`,
                          backgroundColor: '#007bff',
                          height: '20px',
                          borderRadius: '3px',
                          minWidth: '2px'
                        }} />
                        <span style={{ marginLeft: '5px' }}>{answer.percentage}%</span>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ))}
      </div>
    </div>
  );
};

export default SurveyStats;