import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { getAuthenticatedUser } from '../utils/jwtHelper';

export const ProtectedRoute: React.FC = () => {
    const user = getAuthenticatedUser();

    if(!user) {
        return <Navigate to="/login" replace />
    }

    return <Outlet />
}