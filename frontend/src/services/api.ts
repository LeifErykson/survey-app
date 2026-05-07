import axios from 'axios';

const API_BASE_URL = 'http://localhost:5276/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add token to requests if available
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const surveysApi = {
  // Public surveys (anyone can view/take)
  getPublic: () => api.get('/surveys/public'),
  
  // User's own surveys (created by logged-in user)
  getMySurveys: () => api.get('/surveys/my'),
  
  // Get single survey by ID
  getById: (id: number) => api.get(`/surveys/${id}`),
  
  // Create new survey
  create: (data: any) => api.post('/surveys', data),
  
  // Update survey
  update: (id: number, data: any) => api.put(`/surveys/${id}`, data),
  
  // Delete survey
  delete: (id: number) => api.delete(`/surveys/${id}`),
};

export const usersApi = {
  getAll: () => api.get('/users'),
  getById: (id: number) => api.get(`/users/${id}`),
  create: (data: any) => api.post('/users', data),
  update: (id: number, data: any) => api.put(`/users/${id}`, data),
  delete: (id: number) => api.delete(`/users/${id}`),
};

export default api;