"use client"

import React, { createContext, useState, useEffect, useContext } from 'react';
import { useRouter } from 'next/navigation';
import { parseCookies, setCookie, destroyCookie } from 'nookies';

const AuthContext = createContext({
  user: null,
  login: (token) => {},
  logout: () => {},
  refreshAccessToken: () => {},
});

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const router = useRouter();

  useEffect(() => {
    const { loginCookie } = parseCookies();
    if (loginCookie) {
      const decodedToken = decodeAccessToken(loginCookie);
      if (decodedToken) {
        setUser(decodedToken);
      } else {
        logout();
      }
    }
  }, []); 

  const login = (accessToken) => {
  
    const decodedToken = decodeAccessToken(accessToken);
    setUser(decodedToken);
  };

  const logout = () => {
    destroyCookie(null, 'loginCookie');
    localStorage.removeItem("AccessToken");
    setUser(null);
    router.push('/');
  };

  const decodeAccessToken = (accessToken) => {
    try {
      const tokenParts = accessToken.split('.');
      const encodedPayload = tokenParts[1];
      const decodedPayload = atob(encodedPayload);
      return JSON.parse(decodedPayload);
    } catch (error) {
      console.error('Error decoding access token:', error);
      return null;
    }
  };

  const refreshAccessToken = async () => {
    const { accessToken } = parseCookies();
    if (!accessToken) {
      logout();
      return;
    }

    const decodedToken = decodeAccessToken(accessToken);
    if (decodedToken) {
      setUser(decodedToken);
    } else {
      logout();
    }
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, refreshAccessToken }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
