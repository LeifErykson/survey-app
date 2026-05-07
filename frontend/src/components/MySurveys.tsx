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

const MySurveys: React.FC = () => {
  const [surveys, setSurveys] = useState<Survey[]>([]);
  const [loading, setLoading] = useState(true);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [editTitle, setEditTitle] = useState('');
  const [editDescription, setEditDescription] = useState('');

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

  const startEdit = (survey: Survey) => {
    setEditingId(survey.id);
    setEditTitle(survey.title);
    setEditDescription(survey.description);
  };

  const saveEdit = async (id: number) => {
    try {
      await surveysApi.update(id, {
        title: editTitle,
        description: editDescription,
        isActive: surveys.find(s => s.id === id)?.isActive || true
      });
      setEditingId(null);
      await loadSurveys();
    } catch (error) {
      console.error('Save failed:', error);
      alert('Failed to save changes');
    }
  };

  const cancelEdit = () => {
    setEditingId(null);
    setEditTitle('');
    setEditDescription('');
  };

  if (loading) return <div>Loading your surveys...</div>;

  if (surveys.length === 0) {
    return <div>You haven't created any surveys yet. Click "Create New Survey" to get started!</div>;
  }

  return (
    <div>
      <h2>My Surveys</h2>
      <div style={{ marginBottom: '20px' }}>
        <CreateSurvey onSurveyCreated={loadSurveys} />
      </div>
      <ul>
        {surveys.map((survey) => (
          <li key={survey.id} style={{ opacity: survey.isActive ? 1 : 0.5, marginBottom: '20px' }}>
            {editingId === survey.id ? (
              <div>
                <input
                  type="text"
                  value={editTitle}
                  onChange={(e) => setEditTitle(e.target.value)}
                  style={{ width: '100%', marginBottom: '10px' }}
                />
                <textarea
                  value={editDescription}
                  onChange={(e) => setEditDescription(e.target.value)}
                  style={{ width: '100%', marginBottom: '10px' }}
                />
                <button onClick={() => saveEdit(survey.id)}>Save</button>
                <button onClick={cancelEdit}>Cancel</button>
              </div>
            ) : (
              <div>
                <strong>{survey.title}</strong> - {survey.description}
                <br />
                <small>Status: {survey.isActive ? 'Active' : 'Inactive'}</small>
                <div>
                  <button onClick={() => handleToggleActive(survey.id, survey.isActive)}>
                    {survey.isActive ? 'Deactivate' : 'Activate'}
                  </button>
                  <button onClick={() => startEdit(survey)} style={{ marginLeft: '10px' }}>
                    Edit
                  </button>
                  <button onClick={() => handleDelete(survey.id)} style={{ marginLeft: '10px' }}>
                    Delete
                  </button>
                </div>
              </div>
            )}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default MySurveys;