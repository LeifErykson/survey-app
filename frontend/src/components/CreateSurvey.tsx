import React, { useState } from 'react';
import { surveysApi } from '../services/api';

interface CreateSurveyProps {
  onSurveyCreated: () => void;
}

const CreateSurvey: React.FC<CreateSurveyProps> = ({ onSurveyCreated }) => {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [isOpen, setIsOpen] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await surveysApi.create({ title, description });
      setTitle('');
      setDescription('');
      setIsOpen(false);
      onSurveyCreated();
    } catch (error) {
      console.error('Error creating survey:', error);
      alert('Failed to create survey');
    }
  };

  if (!isOpen) {
    return (
      <button onClick={() => setIsOpen(true)}>Create New Survey</button>
    );
  }

  return (
    <div>
      <h3>Create New Survey</h3>
      <form onSubmit={handleSubmit}>
        <div>
          <input
            type="text"
            placeholder="Survey Title"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
          />
        </div>
        <div>
          <textarea
            placeholder="Description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
        </div>
        <button type="submit">Save</button>
        <button type="button" onClick={() => setIsOpen(false)}>Cancel</button>
      </form>
    </div>
  );
};

export default CreateSurvey;