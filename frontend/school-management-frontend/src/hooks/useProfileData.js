import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import axios from 'axios';

export const useProfileData = (endpoint) => {
  const { auth } = useAuth();
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!auth.token) return;
    setLoading(true);
    setError(null);
    axios
      .get(`http://localhost:5248/api${endpoint.startsWith('/') ? endpoint : '/' + endpoint}`, {
        headers: { Authorization: `Bearer ${auth.token}` },
      })
      .then((res) => setData(res.data))
      .catch((err) =>
        setError(
          err.response?.data?.message ||
            err.message ||
            'Failed to fetch data'
        )
      )
      .finally(() => setLoading(false));
  }, [endpoint, auth.token]);

  return { data, loading, error };
}; 