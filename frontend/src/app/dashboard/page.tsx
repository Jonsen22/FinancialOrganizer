"use client"

import { useRouter } from 'next/navigation'; 
import { useEffect, useState } from 'react';

const Dashboard = () => {
    const router = useRouter();
  
    useEffect(() => {
      // Check if access token (bearer token) is present in local storage
      const accessToken = localStorage.getItem('accessToken');
      if (!accessToken) {
        // Redirect to login page if access token is not present
        router.push('./');
      }
    }, []);
  
    const handleLogout = () => {
      // Clear access token from local storage
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      // Redirect to login page
      router.push('./');
    };
  
    return (
      <div>
        <h1>Welcome to the Dashboard!</h1>
        <button onClick={handleLogout}>Logout</button>
      </div>
    );
  };
  
  export default Dashboard;
