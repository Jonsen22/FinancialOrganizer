"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "../../contexts/authContext";
import Feed from "../../components/financialFeed";
import { getTransactions } from "../../hooks/Transactionsdata";
import Navbar from "../../components/navbar";
import Graphs from "../../components/graphs";

interface Transaction {
  bankAccount: {
    bankAccountId: number;
    name: string;
    balance: number;
  };
  bankAccountId: number;
  category: {
    categoryId: number;
    name: string;
    description: string;
    colorhex: string;
  };
  categoryId: number;
  date: Date;
  description: string;
  name: string;
  recurring: string;
  transactionId: number;
  type: string;
  userId: string;
  value: number;
}

interface Balance {
  name: string;
  income: number;
  expense: number;
  id: number;
}

const Dashboard = () => {
  const { user, loading, login } = useAuth(); // Get loading state from auth context
  const router = useRouter();
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [error, setError] = useState("");
  const [total, setTotal] = useState<Balance[]>([]);
  const [month, setMonth] = useState(new Date().getMonth() + 1); // State for month

  useEffect(() => {
    if (!localStorage.getItem("AccessToken")) {
      router.push("/");
    }
  }, [router]);

  const fetchTransactions = async () => {
    if (localStorage.getItem("AccessToken")) {
      const result = await getTransactions(localStorage.getItem("AccessToken"));
      console.log("Fetched transactions:", result);
      if (typeof result === "string" && result.startsWith("Error:")) {
        setError(result);
      } else {
        setTransactions(result);
      }
    } else {
      setError("User is not authenticated");
    }
  };


  useEffect(() => {
    if (localStorage.getItem("AccessToken")) {
      login(localStorage.getItem("AccessToken"))
      fetchTransactions();
    }
  }, []);

  useEffect(() => {
    if (transactions.length > 0) {
      const data = getExpensiveIncome();
      // console.log("Calculated data:", data);
      setTotal(data);
    }
  }, [month, transactions]);

  const getExpensiveIncome = (): Balance[] => {
    const data: { [key: string]: Balance } = {};

    transactions.forEach((transaction) => {
      const transactionMonth = new Date(transaction.date).getMonth() + 1;
      if (transactionMonth === month) {
        const bankAccountId = transaction.bankAccount.bankAccountId;

        if (!data[bankAccountId]) {
          data[bankAccountId] = { name: transaction.bankAccount.name, income: 0, expense: 0, id:transaction.bankAccount.bankAccountId };
        }

        if (transaction.type === "Income") {
          data[bankAccountId].income += transaction.value;
        } else {
          data[bankAccountId].expense += -1*transaction.value;
        }
      }
    });

    console.log("Income and expense data per bank account:", data);
    return Object.values(data); // Convert the object to an array of values
  };

  if (loading) {
    return <div>Loading...</div>; // Show loading state while checking authentication
  }

  if (!user) {
    return <div>User not authenticated</div>; // Handle unauthenticated state
  }

  return (
    <div className="h-full md:h-[100vh] flex flex-col">
      <Navbar />
      <div className="flex flex-col gap-2 md:flex-row h-full min-h-96 m-5 md:m-2">
        <div className="flex-grow md:w-3/5 h-full">
          <Feed transactions={transactions} setMonth={setMonth} />
        </div>
        <div className="overflow-hidden w-full h-[100vh] md:h-full md:w-2/5 flex-grow mt-5 md:mt-2">
          <Graphs data={total} transactions={transactions} />
        </div>
      </div>
    </div>
  );
};

export default Dashboard;