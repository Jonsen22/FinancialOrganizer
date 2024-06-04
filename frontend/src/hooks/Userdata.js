import axios from "axios";

const API_URL = 'https://localhost:8001';

const fetchData = async() =>{
    const response = axios.post()
}

export async function registerUser(email, password) {
    const url = API_URL +'/api/auth/register';
    const data = {
      email: email,
      password: password
    };
  
    try {
      const response = await axios.post(url, data, {
        headers: {
          'Content-Type': 'application/json'
        }
      });
      return response;
    } catch (error) {
      return('Error:', error.response ? error.response.data : error.message);
    }
  }

  export async function loginUser(email, password, remember) {
    let url = API_URL +'/api/auth/login';
    if(remember)
      url += "?useCookie=true";
    const data = {
      email: email,
      password: password
    };
  
    try {
      const response = await axios.post(url, data, {
        withCredentials: true,
        headers: {
          'Content-Type': 'application/json',
        }
      });
      // console.log(response)
      return response;
    } catch (error) {
      return('Error:', error.response ? error.response.data : error.message);
    }
  }

  export const refreshTokenFetch = async (refreshToken) => {
    try {
      const response = await fetch(API_URL+'/api/auth/refresh-token', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${refreshToken}`,
        },
      });
  
      if (!response.ok) {
        throw new Error('Failed to refresh access token');
      }
  
      const { accessToken } = await response.json();
  
      localStorage.setItem('accessToken', accessToken);
  
      return accessToken;
    } catch (error) {
      console.error('Error refreshing access token:', error);
      throw error;
    }
  };
  


