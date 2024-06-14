import axios from "axios";

const API_URL = 'https://localhost:8001';

interface Transaction {
  bankAccountId: number;
  categoryId: number;
  name: string;
  value: number;
  date: string; 
  description: string;
  recurring: string;
  type: string;
}

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
      return(error.response ? error.response.data : error.message);
    }
}

export async function postTransaction(_token, transaction: Transaction ){
   let url = API_URL +'/api/Transaction'

   try{

     const response = await axios.post(url, transaction, {
       headers: {
         'Content-Type': 'application/json',
        'Authorization' : 'bearer ' + _token
      }
    });
  return response
  } catch(error) {
    return(error.response ? error.response.data : error.message);
  }
}

export async function getBankAccounts(_token) {
  let url = API_URL +'/api/BankAccounts/GetBankAccountsByUser';

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
    return(error.response ? error.response.data : error.message);
  }
}

export async function getCategories(_token) {
  let url = API_URL +'/api/Category/GetAllCategories';

  try {
    const response = await axios.get(url, {
      headers: {
        'Content-Type': 'application/json',
        'Authorization' : 'bearer ' + _token
      }
    });
    console.log(response)
    return response.data;
  } catch (error) {
    return(error.response ? error.response.data : error.message);
  }
}