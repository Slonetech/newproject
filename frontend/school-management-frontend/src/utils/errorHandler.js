import { toast } from 'react-toastify';

export const handleApiError = (error, customMessage = null) => {
  const message = customMessage || 
    error.response?.data?.message || 
    error.response?.data?.error ||
    error.message || 
    'Something went wrong';
  
  toast.error(message);
  console.error('API Error:', error);
  return message;
};

export const handleApiSuccess = (message) => {
  toast.success(message);
};

export const handleApiWarning = (message) => {
  toast.warning(message);
};

export const handleApiInfo = (message) => {
  toast.info(message);
}; 