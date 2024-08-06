// /signup
"use client";

import React, { useState } from "react";
import { registerUser, loginUser } from "../../hooks/Userdata";
import CookieConsent from "../../components/cookie";
import { useRouter } from 'next/navigation'; 


const SignUp = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isModalOpen, setIsModalOpen] = useState(false);
  
  const router = useRouter();
  
  const handleLoginSuccess = (response: {
    data: { accessToken: any; refreshToken: any; expiresIn: any; tokenType: any };
  }) => {
    const { accessToken, refreshToken, expiresIn, tokenType } = response.data;
  
    localStorage.setItem("accessToken", accessToken);
    localStorage.setItem("refreshToken", refreshToken);
    localStorage.setItem("expiresIn", expiresIn);
    localStorage.setItem("tokenType", tokenType);
  
    router.push('/dashboard');
  };

  const submit = async () => {
    try {

      if (!email && !password ) {
        setError("Please fill out the informations.");
        return;
      }  else if (!email) {
        setError("Email empty.");
        return;
      } else if (!password) {
        setError("Password empty.");
        return;
      }

      let response = await registerUser(email, password);
      // console.log(response);
      if(response.status == 200){
        let loginResponse = await loginUser(email, password);
        if(loginResponse.status == 200)
          handleLoginSuccess(loginResponse);
      }
    } catch (error) {
      console.error("Error during user registration:", error);
    }
  };

  const handleEmail = (event: React.SetStateAction<string>) => {
    setEmail(event);
  };

  const handleAgree = () => {
    setIsModalOpen(false);
  };

  const handlePassword = (event: React.SetStateAction<string>) => {
    setPassword(event);
  };

  return (
    <div className="flex flex-col justify-center items-center min-h-screen w-full bg-background p-4">
      <div className="w-full max-w-sm bg-card rounded-xl border-2 border-border flex flex-col justify-around items-center p-4 space-y-4 md:space-y-6">
        <div className="text-center w-full">
          <b>Financial Organizer</b>
        </div>

        <div className="flex flex-col justify-center items-start w-full space-y-4">
          <label className="text-left w-full">Email:</label>
          <input
            value={email}
            onChange={(e) => handleEmail(e.target.value)}
            type="email"
            className="rounded-md w-full mb-2 p-2 border"
          />
          <label className="text-left w-full">Password:</label>
          <input
            value={password}
            onChange={(e) => handlePassword(e.target.value)}
            type="password"
            className="rounded-md w-full p-2 border"
          />
        </div>
        {error && <div className="text-error text-sm">{error}</div>}

        <button
          onClick={submit}
          className="bg-grape text-text p-2 rounded-full w-full"
        >
          Sign Up
        </button>
        <div>Already have an account? <a href="./" className="text-grape">Sign In</a></div>
      </div>
      <CookieConsent onAgree={handleAgree} />
    </div>
  );
};

export default SignUp;
