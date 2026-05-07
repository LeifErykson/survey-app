import React, { useEffect, useState } from 'react';
import { surveysApi } from '../services/api';
import CreateSurvey from './CreateSurvey';

interface Survey {
  id: number;
  title: string;
  description: string;
  createdAt: string;
  isActive: boolean;
  user_id: number;
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

  const handleDelete = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this survey?')) return;
    
    try {
      await surveysApi.delete(id);
      await loadSurveys();
    } catch (error) {
      console.error('Error deleting survey:', error);
      alert('Failed to delete survey');
    }
  };

  const handleToggleActive = async (id: number, currentStatus: boolean) => {
    const survey = surveys.find(s => s.id === id);
    if (!survey) return;
    
    try {
      await surveysApi.update(id, {
        title: survey.title,
        description: survey.description,
        isActive: !currentStatus
      });
      await loadSurveys();
    } catch (error) {
      console.error('Error updating survey:', error);
      alert('Failed to update survey status');
    }
  };

  if (loading) return <div>Loading surveys...</div>;

  return (
    <div>
      <h1>Surveys</h1>
      <CreateSurvey onSurveyCreated={loadSurveys} />
      <ul>
        {surveys.map((survey) => (
          <li key={survey.id} style={{ opacity: survey.isActive ? 1 : 0.5 }}>
            <strong>{survey.title}</strong> - {survey.description}
            <br />
            <small>Created by user: {survey.user_id}</small>
            <br />
            <small>Status: {survey.isActive ? 'Active' : 'Inactive'}</small>
            <div>
              <button onClick={() => handleToggleActive(survey.id, survey.isActive)}>
                {survey.isActive ? 'Deactivate' : 'Activate'}
              </button>
              <button onClick={() => handleDelete(survey.id)} style={{ marginLeft: '10px' }}>
                Delete
              </button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default SurveyList;