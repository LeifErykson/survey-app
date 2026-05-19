import React, { useState } from 'react';
import SurveyList from './SurveyList';
import MySurveys from './MySurveys';
import ResponseHistory from './ResponseHistory';

const SurveyTabs: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'public' | 'my' | 'history'>('public');

  return (
    <div>
      <div style={{ borderBottom: '1px solid #ccc', marginBottom: '20px' }}>
        <button
          onClick={() => setActiveTab('public')}
          style={{
            padding: '10px 20px',
            background: activeTab === 'public' ? '#007bff' : '#f0f0f0',
            color: activeTab === 'public' ? 'white' : 'black',
            border: 'none',
            cursor: 'pointer',
            marginRight: '10px'
          }}
        >
          Browse Surveys
        </button>
        <button
          onClick={() => setActiveTab('my')}
          style={{
            padding: '10px 20px',
            background: activeTab === 'my' ? '#007bff' : '#f0f0f0',
            color: activeTab === 'my' ? 'white' : 'black',
            border: 'none',
            cursor: 'pointer',
            marginRight: '10px'
          }}
        >
          My Surveys
        </button>
        <button
          onClick={() => setActiveTab('history')}
          style={{
            padding: '10px 20px',
            background: activeTab === 'history' ? '#007bff' : '#f0f0f0',
            color: activeTab === 'history' ? 'white' : 'black',
            border: 'none',
            cursor: 'pointer'
          }}
        >
          Response History
        </button>
      </div>
      
      {activeTab === 'public' && <SurveyList />}
      {activeTab === 'my' && <MySurveys />}
      {activeTab === 'history' && <ResponseHistory />}
    </div>
  );
};

export default SurveyTabs;