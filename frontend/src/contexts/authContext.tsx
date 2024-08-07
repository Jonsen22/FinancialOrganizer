"use client"

import React, { createContext, useState, useEffect, useContext } from 'react';
import { useRouter } from 'next/navigation';
import { parseCookies, setCookie, destroyCookie } from 'nookies';
import { refreshTokenFetch } from '../hooks/Userdata';

const AuthContext = createContext({
  user: null,
  loading: true,
  login: (token) => {},
  logout: () => {},
  email: null,
  monitorToken: (token) => {},
});

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true); 
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
    setLoading(false); 
  }, []); 

  const login = (accessToken) => {
    const decodedToken = decodeAccessToken(accessToken);
    setUser(decodedToken);
    setLoading(false);
  };

  const logout = () => {
    destroyCookie(null, 'loginCookie');
    localStorage.removeItem("AccessToken");
    setUser(null);
    router.push('/');
    setLoading(false);
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

  const isTokenExpired = (token) => {
    const currentTime = Math.floor(Date.now() / 1000);
    return token.exp < currentTime;
  }


    const monitorToken =(token) => {
      const decoded = decodeAccessToken(token);
      if (isTokenExpired(decoded)) {
        refreshTokenFetch(token).then(success => {
          if (!success) {
            logout();
          }
        });
      }
    
    }




  const emailClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
  const email = user ? user[emailClaim] : '';

  return (
    <AuthContext.Provider value={{ user, loading, login, logout, email, monitorToken }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
