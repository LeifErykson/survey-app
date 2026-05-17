import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { surveysApi } from '../services/api';
import CreateSurvey from './CreateSurvey';

interface Survey {
  id: number;
  title: string;
  description: string;
  createdAt: string;
  isActive: boolean;
}

const MySurveys: React.FC = () => {
  const [surveys, setSurveys] = useState<Survey[]>([]);
  const [loading, setLoading] = useState(true);

  const loadSurveys = async () => {
    try {
      const response = await surveysApi.getMySurveys();
      setSurveys(response.data);
    } catch (error) {
      console.error('Error loading my surveys:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadSurveys();
  }, []);

  const handleDelete = async (id: number) => {
    if (!window.confirm('Delete this survey permanently?')) return;
    try {
      await surveysApi.delete(id);
      await loadSurveys();
    } catch (error) {
      console.error('Delete failed:', error);
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
      console.error('Update failed:', error);
      alert('Failed to update status');
    }
  };

  if (loading) return <div>Loading your surveys...</div>;

  if (surveys.length === 0) {
    return (
      <div>
        <CreateSurvey onSurveyCreated={loadSurveys} />
        <p>You haven't created any surveys yet. Click "Create New Survey" to get started!</p>
      </div>
    );
  }

  return (
    <div>
      <h2>My Surveys</h2>
      <CreateSurvey onSurveyCreated={loadSurveys} />
      <ul>
        {surveys.map((survey) => (
          <li key={survey.id} style={{ opacity: survey.isActive ? 1 : 0.5, marginBottom: '20px', border: '1px solid #ddd', padding: '15px' }}>
            <strong>{survey.title}</strong> - {survey.description}
            <br />
            <small>Status: {survey.isActive ? 'Active' : 'Inactive'}</small>
            <br />
            <small>Created: {new Date(survey.createdAt).toLocaleDateString()}</small>
            <div style={{ marginTop: '10px' }}>
              <Link to={`/edit/${survey.id}`}>
                <button style={{ marginRight: '10px' }}>Edit Questions</button>
              </Link>
              <button onClick={() => handleToggleActive(survey.id, survey.isActive)} style={{ marginRight: '10px' }}>
                {survey.isActive ? 'Deactivate' : 'Activate'}
              </button>
              <button onClick={() => handleDelete(survey.id)}>
                Delete
              </button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default MySurveys;