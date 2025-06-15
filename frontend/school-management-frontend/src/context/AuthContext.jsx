import React, { createContext, useState, useEffect, useContext, useMemo, useCallback } from 'react';
import { jwtDecode } from 'jwt-decode'; // Make sure you have 'jwt-decode' installed (npm install jwt-decode)
import { useNavigate } from 'react-router-dom'; // Import useNavigate
import { login as apiLogin, logout as apiLogout, getUser } from '../services/authService'; // Correct imports

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
    // State to hold the authenticated user's information: { id, username, email, roles }
    const [auth, setAuth] = useState(() => {
        try {
            const storedToken = localStorage.getItem('authToken');
            const storedUser = localStorage.getItem('user');

            if (storedToken && storedUser) {
                const user = JSON.parse(storedUser);
                const decodedToken = jwtDecode(storedToken);
                const expiresAt = new Date(decodedToken.exp * 1000); // Convert Unix timestamp to Date
                const now = Date.now();

                // Check if token is expired
                if (expiresAt.getTime() > now) {
                    return {
                        token: storedToken,
                        user: user
                    };
                } else {
                    console.warn('Authentication token expired on initial load. Clearing storage.');
                    apiLogout();
                }
            }
        } catch (err) {
            console.error('Error loading auth state from localStorage:', err);
            apiLogout();
        }
        return { token: null, user: null };
    });

    const [loading, setLoading] = useState(false); // Only used during login/logout ops
    const [error, setError] = useState(null); // Used for login/registration errors

    const navigate = useNavigate();

    // Use useCallback for memoized functions to prevent unnecessary re-renders
    const handleLogout = useCallback(() => {
        apiLogout(); // Call authService logout to clear storage
        setAuth({ token: null, user: null }); // Clear state
        setError(null); // Clear any errors
        navigate('/login'); // Redirect to login page
    }, [navigate]);

    // Effect to continuously check token expiration
    useEffect(() => {
        const checkTokenValidity = () => {
            if (auth.token) {
                try {
                    const decodedToken = jwtDecode(auth.token);
                    const expiresAt = new Date(decodedToken.exp * 1000);
                    const now = new Date();

                    if (now > expiresAt) {
                        console.warn("Authentication token expired. Logging out.");
                        handleLogout(); // Use the internal handleLogout
                    }
                } catch (err) {
                    console.warn("Invalid token. Logging out.");
                    handleLogout();
                }
            }
        };

        // Set up an interval to check token validity (e.g., every minute)
        const intervalId = setInterval(checkTokenValidity, 60 * 1000); // Check every minute

        // Cleanup function
        return () => clearInterval(intervalId);
    }, [auth.token, handleLogout]); // Depend on relevant auth state and handleLogout


    const handleLogin = useCallback(async (email, password) => {
        setLoading(true);
        setError(null);
        try {
            const data = await apiLogin(email, password);
            setAuth({
                token: data.token,
                user: data.user
            });
            return true;
        } catch (err) {
            const errorMessage = err.message || 'Login failed. Please try again.';
            setError(errorMessage);
            console.error('AuthContext Login error:', err);
            throw err;
        } finally {
            setLoading(false);
        }
    }, []);


    const hasRole = useCallback((requiredRole) => {
        if (!auth.user || !auth.user.roles) return false;
        return auth.user.roles.includes(requiredRole);
    }, [auth.user]);

    const isAdmin = useCallback(() => hasRole("Admin"), [hasRole]);
    const isTeacher = useCallback(() => hasRole("Teacher"), [hasRole]);
    const isStudent = useCallback(() => hasRole("Student"), [hasRole]);
    const isParent = useCallback(() => hasRole("Parent"), [hasRole]);

    // Memoize the context value to avoid unnecessary re-renders for consumers
    const contextValue = useMemo(() => ({
        auth,
        loading,
        error,
        login: handleLogin, // Expose the memoized login function
        logout: handleLogout, // Expose the memoized logout function
        hasRole,
        isAdmin,
        isTeacher,
        isStudent,
        isParent
    }), [auth, loading, error, handleLogin, handleLogout, hasRole, isAdmin, isTeacher, isStudent, isParent]);

    return (
        <AuthContext.Provider value={contextValue}>
            {children}
        </AuthContext.Provider>
    );
};

// Custom hook for easier consumption of AuthContext
export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};

// ProtectedRoute Component: Handles redirection based on authentication and roles
export const ProtectedRoute = ({ children, allowedRoles }) => {
    const { auth, loading } = useAuth(); // Get auth state and loading from context
    const navigate = useNavigate();

    // Show loading indicator or null while authentication state is being determined
    if (loading) { // Check for true explicitly if loading state can be false initially before check
        return <div className="flex justify-center items-center h-screen text-lg text-gray-700">Loading authentication...</div>;
    }

    // Check if user is authenticated
    if (!auth.token || !auth.user) {
        // If not authenticated, redirect to login
        useEffect(() => {
            navigate('/login');
        }, [navigate]);
        return null; // Don't render children until redirect
    }

    // If roles are specified, check if user has any of the allowed roles
    if (allowedRoles && allowedRoles.length > 0) {
        const userHasRequiredRole = allowedRoles.some(role => auth.user.roles.includes(role));
        if (!userHasRequiredRole) {
            // If authenticated but no required role, redirect to unauthorized page
            useEffect(() => {
                navigate('/unauthorized'); // Redirect to a dedicated unauthorized page
            }, [navigate]);
            return null; // Don't render children until redirect
        }
    }

    // If authenticated and has allowed roles, render children
    return children;
};
