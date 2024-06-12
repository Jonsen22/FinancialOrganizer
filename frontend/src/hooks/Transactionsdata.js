import axios from "axios";

const API_URL = 'https://localhost:8001';

export async function getTransactions(_token) {
    let url = API_URL +'/api/Transaction/GetTransactionsByUser';
  
    try {
      const response = await axios.get(url, {
        headers: {
          'Content-Type': 'application/json',
          'Authorization' : 'bearer ' + _token
        }
      });
      // console.log(response)
      return response.data;
    } catch (error) {
      return('Error:', error.response ? error.response.data : error.message);
    }
}