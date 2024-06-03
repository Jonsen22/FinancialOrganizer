"use client"

import React, { useState, useEffect } from 'react';

const CookieConsent = ({ onAgree }) => {
  const [isVisible, setIsVisible] = useState(false);

  useEffect(() => {
    const cookieConsent = getCookie('cookieConsent');
    if (!cookieConsent) {
      setIsVisible(true);
    }
  }, []);

  const handleAgree = () => {
    setCookie('cookieConsent', 'true', 365);
    setIsVisible(false);
    onAgree();
  };

  const setCookie = (name, value, days) => {
    const d = new Date();
    d.setTime(d.getTime() + (days * 24 * 60 * 60 * 1000));
    const expires = "expires=" + d.toUTCString();
    document.cookie = `${name}=${value};${expires};path=/`;
  };

  const getCookie = (name) => {
    const nameEQ = `${name}=`;
    const ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
      let c = ca[i];
      while (c.charAt(0) === ' ') c = c.substring(1, c.length);
      if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
  };

  if (!isVisible) {
    return null;
  }

  return (
    <>
      <div className="fixed inset-0 bg-black bg-opacity-50 z-40" />
      <div className="fixed bottom-0 left-0 w-full bg-gray-800 bg-opacity-80 text-white p-4 z-50 flex justify-center items-center">
        <div className="max-w-xl w-full text-center p-4">
          <p className="mb-4">We use cookies to ensure you get the best experience on our website. By continuing to use our site, you accept our use of cookies.</p>
          <button className="bg-green-500 hover:bg-green-600 text-white py-2 px-4 rounded" onClick={handleAgree}>Agree</button>
        </div>
      </div>
    </>
  );
};

export default CookieConsent;
