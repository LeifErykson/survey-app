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
  options: AnswerOption[];
}

interface SurveyDetail {
  id: number;
  title: string;
  description: string;
  isActive: boolean;
  questions: Question[];
}

const TakeSurvey: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [survey, setSurvey] = useState<SurveyDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [selectedAnswers, setSelectedAnswers] = useState<Record<number, number>>({});
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

  const handleAnswerSelect = (questionId: number, answerId: number) => {
    setSelectedAnswers(prev => ({
      ...prev,
      [questionId]: answerId
    }));
  };

  const handleSubmit = async () => {
    // Check if all questions are answered
    if (!survey) return;
    
    const allAnswered = survey.questions.every(q => selectedAnswers[q.id]);
    if (!allAnswered) {
      alert('Please answer all questions before submitting');
      return;
    }

    setSubmitting(true);
    try {
      const answers = Object.entries(selectedAnswers).map(([questionId, answerId]) => ({
        questionId: Number(questionId),
        answerId: answerId
      }));

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
      <h1>{survey.title}</h1>
      <p>{survey.description}</p>
      
      <form onSubmit={(e) => { e.preventDefault(); handleSubmit(); }}>
        {survey.questions.map((question, index) => (
          <div key={question.id} style={{ marginBottom: '30px', border: '1px solid #ccc', padding: '15px' }}>
            <h3>Question {index + 1}: {question.text}</h3>
            <div>
              {question.options.map(option => (
                <label key={option.id} style={{ display: 'block', margin: '10px 0' }}>
                  <input
                    type="radio"
                    name={`question_${question.id}`}
                    value={option.id}
                    checked={selectedAnswers[question.id] === option.id}
                    onChange={() => handleAnswerSelect(question.id, option.id)}
                  />
                  {option.text}
                </label>
              ))}
            </div>
          </div>
        ))}
        
        <div style={{ display: 'flex', gap: '10px', marginTop: '20px' }}>
          <button type="button" onClick={() => navigate('/')}>Back</button>
          <button type="submit" disabled={submitting}>
            {submitting ? 'Submitting...' : 'Submit Survey'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default TakeSurvey;