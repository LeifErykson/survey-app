import axios from 'axios';

const API_BASE_URL = 'http://localhost:5276/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const surveysApi = {
  getAll: () => api.get('/surveys'),
  getById: (id: number) => api.get(`/surveys/${id}`),
  create: (data: any) => api.post('/surveys', data),
  update: (id: number, data: any) => api.put(`/surveys/${id}`, data),
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