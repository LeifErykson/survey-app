import React, { useEffect, useState } from 'react';
import { surveysApi } from '../services/api';
import CreateSurvey from './CreateSurvey';

interface Survey {
  id: number;
  title: string;
  description: string;
  created_at: string;
  is_active: boolean;
}

const SurveyList: React.FC = () => {
  const [surveys, setSurveys] = useState<Survey[]>([]);
  const [loading, setLoading] = useState(true);

  const loadSurveys = async () => {
    try {
      const response = await surveysApi.getAll();
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

  return (
    <div>
      <h1>Surveys</h1>
      <CreateSurvey onSurveyCreated={loadSurveys} />
      <ul>
        {surveys.map((survey) => (
          <li key={survey.id}>
            <strong>{survey.title}</strong> - {survey.description}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default SurveyList;