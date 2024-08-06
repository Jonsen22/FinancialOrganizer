"use client";

import React, { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { loginUser } from "../hooks/Userdata";
import { useAuth } from '../contexts/authContext';
import CookieConsent from "../components/cookie";
import {parseCookies} from "nookies";

const Home = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [rememberMe, setRememberMe] = useState(false);
  const { login } = useAuth();

  const router = useRouter();

  const handleLoginSuccess = (response: { data: { token: any } }) => {
    
   let token = null;

    // if (!rememberMe) {
    //   token = response.data.token;
    // } else {
    //   const cookies = parseCookies();
    //   console.log(cookies)
    //   token = cookies.loginCookie; 
    //   console.log(token)
    // }

    token = response.data.token;

    localStorage.setItem("AccessToken", token);
    // console.log(token)
    login(token);
    router.push("/dashboard");
  };

  useEffect(() => {
    const cookieConsent = document.cookie
      .split(";")
      .some((item) => item.trim().startsWith("cookieConsent="));
    if (!cookieConsent) {
      setIsModalOpen(true);
    }
  }, []);

  const submit = async () => {
    try {
      if (!email && !password) {
        setError("Please enter both your email address and password.");
        return;
      } else if (!email) {
        setError("Please enter your email address.");
        return;
      } else if (!password) {
        setError("Please enter your password.");
        return;
      }

      let response = await loginUser(email, password, rememberMe);
      // console.log(response)

      if (response.title == "Unauthorized") setError("wrong email or password");
      else handleLoginSuccess(response);
    } catch (error) {
      console.error("Error during user registration:", error);
    }
  };

  const handleEmail = (event) => {
    setEmail(event.target.value);
  };

  const handlePassword = (event) => {
    setPassword(event.target.value);
  };

  const handleAgree = () => {
    setIsModalOpen(false);
  };

  const handleRemeberMe = () => {
    setRememberMe(!rememberMe);
  }

  return (
    <div>
      <div
        className={`flex flex-col justify-center items-center min-h-screen w-full bg-background p-4 ${
          isModalOpen ? "pointer-events-none" : ""
        }`}
      >
        <div className="w-full max-w-sm bg-card rounded-xl border-2 border-border flex flex-col justify-around items-center p-4 space-y-4 md:space-y-6">
          <div className="text-center w-full">
            <b>Financial Organizer</b>
          </div>

          <div className="flex flex-col justify-center items-start w-full space-y-4">
            <label className="text-left w-full">Email:</label>
            <input
              value={email}
              onChange={handleEmail}
              type="email"
              className="rounded-md w-full mb-2 p-2 border"
            />
            <label className="text-left w-full">Password:</label>
            <input
              value={password}
              onChange={handlePassword}
              type="password"
              className="rounded-md w-full p-2 border mb-2"
            />
            <div
              className="flex justify-between items-center w-full text-sm text-grape"
              style={{ marginTop: "0.5rem" }}
            >
              <div onClick={handleRemeberMe} tabIndex={0} onKeyDown={(e) => {
                if(e.key === 'Enter') {
                  handleRemeberMe()
                }
              }}>
                <input type="checkbox" checked={rememberMe}  onChange={handleRemeberMe} className="mr-1"/>
                <span>Remember me.</span>
              </div>
              <div>
                <a className="cursor-pointer">Forgot your password?</a>
              </div>
            </div>
          </div>

          {error && <div className="text-error text-sm">{error}</div>}
          <button
            className="bg-grape text-text p-2 rounded-full w-full"
            onClick={submit}
          >
            Login
          </button>

          <div>
            Don't have an account?{" "}
            <a className="text-grape" href="/signup">
              Register
            </a>
          </div>
        </div>
      </div>
      <CookieConsent onAgree={handleAgree} />
    </div>
  );
};

export default Home;
