import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { surveysApi } from '../services/api';

interface Survey {
  id: number;
  title: string;
  description: string;
  createdAt: string;
  isActive: boolean;
}

const SurveyList: React.FC = () => {
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
          <li key={survey.id} style={{ marginBottom: '20px', border: '1px solid #ddd', padding: '15px' }}>
            <strong>{survey.title}</strong> - {survey.description}
            <br />
            <small>Created: {new Date(survey.createdAt).toLocaleDateString()}</small>
            <div>
              <Link to={`/survey/${survey.id}`}>
                <button>Take Survey</button>
              </Link>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default SurveyList;