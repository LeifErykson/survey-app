import React, { useState } from 'react';
import SurveyList from './SurveyList';
import MySurveys from './MySurveys';

const SurveyTabs: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'public' | 'my'>('public');

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
            cursor: 'pointer'
          }}
        >
          My Surveys
        </button>
      </div>
      
      {activeTab === 'public' && <SurveyList />}
      {activeTab === 'my' && <MySurveys />}
    </div>
  );
};

export default SurveyTabs;