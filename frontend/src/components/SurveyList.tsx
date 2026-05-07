import React, { useEffect, useState } from 'react';
import { surveysApi } from '../services/api';
import CreateSurvey from './CreateSurvey';

interface Survey {
  id: number;
  title: string;
  description: string;
  createdAt: string;
  isActive: boolean;
}

interface SurveyListProps {
  isMySurveys?: boolean;
}

const SurveyList: React.FC<SurveyListProps> = ({ isMySurveys = false }) => {
  const [surveys, setSurveys] = useState<Survey[]>([]);
  const [loading, setLoading] = useState(true);

  const loadSurveys = async () => {
    try {
      const response = await surveysApi.getPublic();
      setSurveys(response.data);
    } catch (error) {
      console.error('Error loading surveys:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadSurveys();
  }, []);

  if (loading) return <div>Loading surveys...</div>;

  if (surveys.length === 0) {
    return <div>No active surveys available. Check back later!</div>;
  }

  return (
    <div>
      <h2>Available Surveys</h2>
      <ul>
        {surveys.map((survey) => (
          <li key={survey.id} style={{ marginBottom: '20px' }}>
            <strong>{survey.title}</strong> - {survey.description}
            <br />
            <small>Created: {new Date(survey.createdAt).toLocaleDateString()}</small>
            <div>
              <button>Take Survey</button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default SurveyList;