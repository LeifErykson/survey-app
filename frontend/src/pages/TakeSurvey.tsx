import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { surveysApi } from '../services/api';

interface AnswerOption {
  id: number;
  text: string;
}

interface Question {
  id: number;
  text: string;
  type: string;
  options: AnswerOption[];
}

interface SurveyDetail {
  id: number;
  title: string;
  description: string;
  isActive: boolean;
  questions: Question[];
}

type SelectedAnswers = Record<number, number[]>;

const TakeSurvey: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [survey, setSurvey] = useState<SurveyDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [selectedAnswers, setSelectedAnswers] = useState<SelectedAnswers>({});
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    loadSurvey();
  }, [id]);

  const loadSurvey = async () => {
    try {
      const response = await surveysApi.getSurveyDetails(Number(id));
      setSurvey(response.data);
    } catch (error) {
      console.error('Error loading survey:', error);
      alert('Survey not found');
      navigate('/');
    } finally {
      setLoading(false);
    }
  };

  const handleSingleSelect = (questionId: number, answerId: number) => {
    setSelectedAnswers(prev => ({
      ...prev,
      [questionId]: [answerId]
    }));
  };

  const handleMultipleSelect = (questionId: number, answerId: number) => {
    setSelectedAnswers(prev => {
      const current = prev[questionId] || [];
      if (current.includes(answerId)) {
        return {
          ...prev,
          [questionId]: current.filter(id => id !== answerId)
        };
      } else {
        return {
          ...prev,
          [questionId]: [...current, answerId]
        };
      }
    });
  };

  const handleSubmit = async () => {
    if (!survey) return;
    
    // Check if all questions are answered
    const allAnswered = survey.questions.every(q => {
      const answers = selectedAnswers[q.id];
      return answers && answers.length > 0;
    });
    
    if (!allAnswered) {
      alert('Please answer all questions before submitting');
      return;
    }

    setSubmitting(true);
    try {
      // Flatten answers for submission
      const answers = Object.entries(selectedAnswers).flatMap(([questionId, answerIds]) =>
        answerIds.map(answerId => ({
          questionId: Number(questionId),
          answerId: answerId
        }))
      );

      await surveysApi.submitResponse({
        surveyId: survey.id,
        answers: answers
      });
      
      alert('Survey submitted successfully!');
      navigate('/');
    } catch (error) {
      console.error('Error submitting survey:', error);
      alert('Failed to submit survey');
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) return <div>Loading survey...</div>;
  if (!survey) return <div>Survey not found</div>;

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
        <h1>{survey.title}</h1>
        <button onClick={() => navigate('/')}>Back to Surveys</button>
      </div>
      <p>{survey.description}</p>
      
      <form onSubmit={(e) => { e.preventDefault(); handleSubmit(); }}>
        {survey.questions.map((question, index) => (
          <div key={question.id} style={{ marginBottom: '30px', border: '1px solid #ccc', padding: '15px', borderRadius: '5px' }}>
            <h3>
              Question {index + 1}: {question.text}
              {question.type === 'multiple' && <small style={{ marginLeft: '10px', color: '#666' }}>(Select all that apply)</small>}
            </h3>
            <div>
              {question.options.map(option => (
                <label key={option.id} style={{ display: 'block', margin: '10px 0' }}>
                  <input
                    type={question.type === 'multiple' ? 'checkbox' : 'radio'}
                    name={`question_${question.id}`}
                    value={option.id}
                    checked={selectedAnswers[question.id]?.includes(option.id) || false}
                    onChange={() => {
                      if (question.type === 'multiple') {
                        handleMultipleSelect(question.id, option.id);
                      } else {
                        handleSingleSelect(question.id, option.id);
                      }
                    }}
                  />
                  {option.text}
                </label>
              ))}
            </div>
          </div>
        ))}
        
        <div style={{ display: 'flex', gap: '10px', marginTop: '20px' }}>
          <button type="button" onClick={() => navigate('/')}>Cancel</button>
          <button type="submit" disabled={submitting}>
            {submitting ? 'Submitting...' : 'Submit Survey'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default TakeSurvey;