import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { surveysApi } from '../services/api';
import api from '../services/api';

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

const EditSurvey: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [survey, setSurvey] = useState<SurveyDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [newQuestionText, setNewQuestionText] = useState('');
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');

  useEffect(() => {
    loadSurvey();
  }, [id]);

  const loadSurvey = async () => {
    try {
      const response = await surveysApi.getSurveyDetails(Number(id));
      setSurvey(response.data);
      setTitle(response.data.title);
      setDescription(response.data.description);
    } catch (error) {
      console.error('Error loading survey:', error);
      alert('Failed to load survey');
      navigate('/');
    } finally {
      setLoading(false);
    }
  };


  const saveSurvey = async () => {
    setSaving(true);
    try {
      await surveysApi.update(Number(id), {
        title: title,
        description: description,
        isActive: survey?.isActive || true
      });
      alert('Survey saved successfully!');
      navigate('/');
    } catch (error) {
      console.error('Error saving survey:', error);
      alert('Failed to save survey');
    } finally {
      setSaving(false);
    }
  };

  const addQuestion = async () => {
    if (!newQuestionText.trim()) return;
    
    try {
      await api.post('/questions', {
        text: newQuestionText,
        surveyId: Number(id)
      });
      setNewQuestionText('');
      await loadSurvey();
    } catch (error) {
      console.error('Error adding question:', error);
      alert('Failed to add question');
    }
  };

  const deleteQuestion = async (questionId: number) => {
    if (!window.confirm('Delete this question? All answers will be deleted too.')) return;
    
    try {
      await api.delete(`/questions/${questionId}`);
      await loadSurvey();
    } catch (error) {
      console.error('Error deleting question:', error);
      alert('Failed to delete question');
    }
  };

  const addOption = async (questionId: number, optionText: string) => {
    if (!optionText.trim()) return;
    
    try {
      await api.post('/answers', {
        text: optionText,
        questionId: questionId
      });
      await loadSurvey();
    } catch (error) {
      console.error('Error adding option:', error);
      alert('Failed to add option');
    }
  };

  const deleteOption = async (optionId: number) => {
    try {
      await api.delete(`/answers/${optionId}`);
      await loadSurvey();
    } catch (error) {
      console.error('Error deleting option:', error);
      alert('Failed to delete option');
    }
  };

  if (loading) return <div>Loading...</div>;
  if (!survey) return <div>Survey not found</div>;

   return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
        <h1>Edit Survey</h1>
        <div>
          <button onClick={() => navigate('/')} style={{ marginRight: '10px' }}>Back</button>
          <button onClick={saveSurvey} disabled={saving}>
            {saving ? 'Saving...' : 'Save Survey'}
          </button>
        </div>
      </div>
      
      <div style={{ marginBottom: '20px' }}>
        <div>
          <label>Title: </label>
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            style={{ width: '300px', marginBottom: '10px', padding: '5px' }}
          />
        </div>
        <div>
          <label>Description: </label>
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            style={{ width: '300px', padding: '5px' }}
          />
        </div>
      </div>
      
      <hr />
      
      <h2>Questions</h2>
      
      <div style={{ marginBottom: '20px' }}>
        <input
          type="text"
          placeholder="New question text"
          value={newQuestionText}
          onChange={(e) => setNewQuestionText(e.target.value)}
          style={{ marginRight: '10px', padding: '5px', width: '300px' }}
        />
        <button onClick={addQuestion}>Add Question</button>
      </div>
      
      {survey.questions.map((question, qIndex) => (
        <div key={question.id} style={{ border: '1px solid #ccc', marginBottom: '20px', padding: '15px', borderRadius: '5px' }}>
          <h3>Question {qIndex + 1}: {question.text}</h3>
          <button onClick={() => deleteQuestion(question.id)}>Delete Question</button>
          
          <h4>Answer Options</h4>
          <ul>
            {question.options.map((option) => (
              <li key={option.id}>
                {option.text}
                <button onClick={() => deleteOption(option.id)} style={{ marginLeft: '10px' }}>Delete</button>
              </li>
            ))}
          </ul>
          
          <AddOptionForm onAdd={(text) => addOption(question.id, text)} />
        </div>
      ))}
    </div>
  );
};

const AddOptionForm: React.FC<{ onAdd: (text: string) => void }> = ({ onAdd }) => {
  const [text, setText] = useState('');
  
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (text.trim()) {
      onAdd(text);
      setText('');
    }
  };
  
  return (
    <form onSubmit={handleSubmit}>
      <input
        type="text"
        placeholder="New answer option"
        value={text}
        onChange={(e) => setText(e.target.value)}
        style={{ marginRight: '10px', padding: '5px', width: '200px' }}
      />
      <button type="submit">Add Option</button>
    </form>
  );
};

export default EditSurvey;