import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { useNavigate, useLocation, Navigate } from 'react-router-dom';
import * as authService from '../services/authService'; // Correct import for named exports

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
    const [auth, setAuth] = useState({
        isAuthenticated: false,
        user: null,
        token: null,
        loading: false, // Added loading state
        error: null,    // Added error state
    });
    const navigate = useNavigate();

    // Load authentication state from localStorage on initial load
    useEffect(() => {
        const storedUser = localStorage.getItem('user');
        const storedToken = localStorage.getItem('authToken'); // Use 'authToken' as per your authService
        // === AUTH CONTEXT DEBUG ===
        console.log('=== AUTH CONTEXT DEBUG ===');
        console.log('Loaded user:', storedUser);
        console.log('Loaded token:', storedToken);
        console.log('========================');
        if (storedUser && storedToken) {
            try {
                const user = JSON.parse(storedUser);
                setAuth(prev => ({
                    ...prev,
                    isAuthenticated: true,
                    user: user,
                    token: storedToken,
                }));
            } catch (error) {
                console.error("Failed to parse user from localStorage:", error);
                localStorage.removeItem('user');
                localStorage.removeItem('authToken');
                setAuth(prev => ({ ...prev, isAuthenticated: false, user: null, token: null }));
            }
        }
    }, []);

    // This is the 'login' function that will be exposed via the context
    // It now expects a single 'credentials' object {email, password}
    const login = async (email, password) => { // Modified to directly accept email and password
        setAuth(prev => ({ ...prev, loading: true, error: null })); // Set loading true, clear error
        try {
            // Call the 'login' function from the imported 'authService' object
            // authService.login expects email, password directly
            const data = await authService.login(email, password);

            localStorage.setItem('authToken', data.token); // authService already sets this but good to be explicit
            localStorage.setItem('user', JSON.stringify(data.user)); // authService already sets this

            setAuth(prev => ({
                ...prev,
                isAuthenticated: true,
                user: data.user,
                token: data.token,
                loading: false,
            }));

            // The immediate redirect logic moved from LoginPage to here, as discussed
            if (data.user && Array.isArray(data.user.roles)) {
                if (data.user.roles.includes('Admin')) {
                    navigate('/admin', { replace: true });
                } else if (data.user.roles.includes('Teacher')) {
                    navigate('/teacher', { replace: true });
                } else if (data.user.roles.includes('Student')) {
                    navigate('/student', { replace: true });
                } else if (data.user.roles.includes('Parent')) {
                    navigate('/parent', { replace: true });
                } else {
                    navigate('/', { replace: true }); // Default fallback
                }
            } else {
                navigate('/', { replace: true }); // Fallback if roles are missing
            }
            return true; // Indicate success
        } catch (error) {
            console.error('Login failed in AuthContext:', error);
            localStorage.removeItem('user');
            localStorage.removeItem('authToken');
            setAuth(prev => ({
                ...prev,
                isAuthenticated: false,
                user: null,
                token: null,
                loading: false,
                error: error.message || 'Login failed. Please check your credentials.', // Capture error message
            }));
            throw error; // Re-throw to be handled by LoginPage
        }
    };

    const logout = useCallback(() => {
        authService.logout(); // Call authService's logout function
        setAuth(prev => ({
            ...prev,
            isAuthenticated: false,
            user: null,
            token: null,
            loading: false,
            error: null,
        }));
        navigate('/login', { replace: true });
    }, [navigate]);

    return (
        <AuthContext.Provider value={{ auth, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};

export const ProtectedRoute = ({ children, allowedRoles }) => {
    const { auth } = useAuth();
    const location = useLocation();

    if (auth.loading) { // Optionally show a loading spinner here
        return <div className="text-center py-20 text-sky-700">Loading user data...</div>;
    }

    if (!auth.isAuthenticated) {
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    // Role-based redirect for the root path '/' is handled within AuthProvider's login
    // and also explicitly here for any direct navigation to '/' when already logged in.
    if (location.pathname === '/') {
        if (auth.user?.roles && Array.isArray(auth.user.roles)) {
            if (auth.user.roles.includes('Admin')) {
                return <Navigate to="/admin" replace />;
            } else if (auth.user.roles.includes('Teacher')) {
                return <Navigate to="/teacher" replace />;
            } else if (auth.user.roles.includes('Student')) {
                return <Navigate to="/student" replace />;
            } else if (auth.user.roles.includes('Parent')) {
                return <Navigate to="/parent" replace />;
            }
        }
        // If authenticated but no specific role dashboard, allow to render children (e.g., HomePage)
    }

    if (allowedRoles && (!auth.user?.roles || !Array.isArray(auth.user.roles) || !allowedRoles.some(role => auth.user.roles.includes(role)))) {
        return <Navigate to="/unauthorized" replace />;
    }

    return children;
};