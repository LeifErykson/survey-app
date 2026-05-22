import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import { useAuth } from '../context/AuthContext';

interface User {
  id: number;
  login: string;
  email: string;
  createdAt: string;
  isAdmin: boolean;
  surveyCount: number;
  responseCount: number;
}

const AdminPanel: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [actionLoading, setActionLoading] = useState<number | null>(null);
  const { user: currentUser } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    loadUsers();
  }, []);

  const loadUsers = async () => {
    try {
      const response = await api.get('/admin/users');
      setUsers(response.data);
    } catch (err: any) {
      console.error('Error loading users:', err);
      setError(err.response?.data || 'Failed to load users');
    } finally {
      setLoading(false);
    }
  };

  const makeAdmin = async (userId: number) => {
    if (!window.confirm('Make this user an admin?')) return;
    setActionLoading(userId);
    try {
      await api.put(`/admin/users/${userId}/make-admin`);
      await loadUsers();
    } catch (err: any) {
      alert(err.response?.data || 'Failed to make admin');
    } finally {
      setActionLoading(null);
    }
  };

  const removeAdmin = async (userId: number) => {
    if (!window.confirm('Remove admin rights from this user?')) return;
    setActionLoading(userId);
    try {
      await api.put(`/admin/users/${userId}/remove-admin`);
      await loadUsers();
    } catch (err: any) {
      alert(err.response?.data || 'Failed to remove admin');
    } finally {
      setActionLoading(null);
    }
  };

  const deleteUser = async (userId: number, userName: string) => {
    if (!window.confirm(`Delete user "${userName}"? This will delete all their surveys and responses.`)) return;
    setActionLoading(userId);
    try {
      await api.delete(`/admin/users/${userId}`);
      await loadUsers();
    } catch (err: any) {
      alert(err.response?.data || 'Failed to delete user');
    } finally {
      setActionLoading(null);
    }
  };

  if (loading) return <div>Loading users...</div>;
  if (error) return <div style={{ color: 'red' }}>{error}</div>;
  if (!currentUser?.isAdmin) return <div>Access denied. Admin privileges required.</div>;

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
        <h1>Admin Panel - User Management</h1>
        <button onClick={() => navigate('/')} style={{ padding: '8px 16px', cursor: 'pointer' }}>
          ← Back to Surveys
        </button>
      </div>
      
      <table style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr style={{ backgroundColor: '#f0f0f0' }}>
            <th style={{ padding: '10px', textAlign: 'left', borderBottom: '1px solid #ddd' }}>ID</th>
            <th style={{ padding: '10px', textAlign: 'left', borderBottom: '1px solid #ddd' }}>Login</th>
            <th style={{ padding: '10px', textAlign: 'left', borderBottom: '1px solid #ddd' }}>Email</th>
            <th style={{ padding: '10px', textAlign: 'left', borderBottom: '1px solid #ddd' }}>Created</th>
            <th style={{ padding: '10px', textAlign: 'center', borderBottom: '1px solid #ddd' }}>Surveys</th>
            <th style={{ padding: '10px', textAlign: 'center', borderBottom: '1px solid #ddd' }}>Responses</th>
            <th style={{ padding: '10px', textAlign: 'center', borderBottom: '1px solid #ddd' }}>Admin</th>
            <th style={{ padding: '10px', textAlign: 'center', borderBottom: '1px solid #ddd' }}>Actions</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr key={user.id} style={{ borderBottom: '1px solid #eee' }}>
              <td style={{ padding: '10px' }}>{user.id}</td>
              <td style={{ padding: '10px' }}>{user.login}</td>
              <td style={{ padding: '10px' }}>{user.email}</td>
              <td style={{ padding: '10px' }}>{new Date(user.createdAt).toLocaleDateString()}</td>
              <td style={{ textAlign: 'center', padding: '10px' }}>{user.surveyCount}</td>
              <td style={{ textAlign: 'center', padding: '10px' }}>{user.responseCount}</td>
              <td style={{ textAlign: 'center', padding: '10px' }}>
                {user.isAdmin ? '✅ Yes' : '❌ No'}
              </td>
              <td style={{ textAlign: 'center', padding: '10px' }}>
                {!user.isAdmin && (
                  <button
                    onClick={() => makeAdmin(user.id)}
                    disabled={actionLoading === user.id}
                    style={{ marginRight: '5px' }}
                  >
                    Make Admin
                  </button>
                )}
                {user.isAdmin && currentUser.id !== user.id && (
                  <button
                    onClick={() => removeAdmin(user.id)}
                    disabled={actionLoading === user.id}
                    style={{ marginRight: '5px', backgroundColor: '#ffc107' }}
                  >
                    Remove Admin
                  </button>
                )}
                {currentUser.id !== user.id && (
                  <button
                    onClick={() => deleteUser(user.id, user.login)}
                    disabled={actionLoading === user.id}
                    style={{ backgroundColor: '#dc3545', color: 'white' }}
                  >
                    Delete
                  </button>
                )}
                {currentUser.id === user.id && (
                  <span style={{ color: '#666' }}>You</span>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default AdminPanel;