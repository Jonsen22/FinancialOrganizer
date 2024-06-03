import axios from "axios";

const API_URL = 'http://localhost:8004';

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

  export async function loginUser(email, password) {
    const url = API_URL +'/api/auth/login?useCookies=true';
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
      console.log(response)
      return response;
    } catch (error) {
      return('Error:', error.response ? error.response.data : error.message);
    }
  }
  


