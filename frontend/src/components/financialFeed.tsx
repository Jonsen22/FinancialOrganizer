"use client";

import React, { useEffect, useState } from "react";
import FeedCard from "./feedCard";
import Modal from "./modal";

const Feed = ({ transactions = [], setMonth, updateTransaction, deleteTransaction  }) => {
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

  interface TransactionEdit {
    transactionId: number,
    bankAccountId: number;
    categoryId: number;
    name: string;
    value: number;
    date: string; 
    description: string;
    recurring: string;
    type: string;
  }

  if (!Array.isArray(transactions)) {
    transactions = [];
  }

  console.log(transactions)

  const year = new Date().getFullYear();
  const presentMonth = new Date().getMonth() + 1;
  const years = Array.from(new Array(20), (val, index) => year - index);

  const [month, setMonthState] = useState(presentMonth);
  const [modal, setModal] = useState(false);
  const [update, setUpdate] = useState(false);
  const [transactionEdit, setTransactionEdit] = useState<TransactionEdit>();

  const handleChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
    const newMonth = parseInt(event.target.value);
    setMonthState(newMonth);
    setMonth(newMonth);
  };

  const handleModal = () => {
    setModal(!modal);
    setUpdate(false);
  };

  const handleEditModal = (transaction: TransactionEdit) => {
    setModal(!modal);
    setUpdate(true);
    setTransactionEdit(transaction);
  }



  const groupedTransactions = Array.isArray(transactions)
    ? transactions.reduce((groups, transaction) => {
        const transactionMonth = new Date(transaction.date).getMonth() + 1;
        if (transactionMonth === month) {
          const date = new Date(transaction.date).toLocaleDateString();
          if (!groups[date]) {
            groups[date] = [];
          }
          groups[date].push(transaction);
        }
        return groups;
      }, {})
    : {};

  const handleScroll = (event: React.UIEvent<HTMLDivElement>) => {
    const feedContainer = event.currentTarget;
    const scrollThreshold = 200;
    const scrollTop = feedContainer.scrollTop;
    const scrollHeight = feedContainer.scrollHeight;
    const clientHeight = feedContainer.clientHeight;

    if (scrollHeight > clientHeight) {
      if (scrollTop === 0) {
        // Scrolled to the top, change to previous month
        setMonthState((prevMonth) => prevMonth - 1);
        setMonth((prevMonth) => prevMonth - 1); // Update month in Dashboard component
      } else if (scrollHeight - scrollTop === clientHeight) {
        // Scrolled to the bottom, change to next month
        setMonthState((prevMonth) => prevMonth + 1);
        setMonth((prevMonth) => prevMonth + 1); // Update month in Dashboard component
      }
    }
  };

  return (
    <div className="w-full h-[90vh] md:h-full flex flex-col items-center justify-center">
      <div className="flex items-center justify-between w-full md:mb-2">
        <div className="flex flex-row items-center pl-2 justify-evenly md:justify-center w-full h-10 md:gap-2">
          <select id="month" value={month} onChange={handleChange}>
            <option value="1">January</option>
            <option value="2">February</option>
            <option value="3">March</option>
            <option value="4">April</option>
            <option value="5">May</option>
            <option value="6">June</option>
            <option value="7">July</option>
            <option value="8">August</option>
            <option value="9">September</option>
            <option value="10">October</option>
            <option value="11">November</option>
            <option value="12">December</option>
          </select>
          <select>
            {years.map((year, index) => (
              <option key={`year${index}`} value={year}>
                {year}
              </option>
            ))}
          </select>
          <div className="flex gap-2">
            <input type="checkbox" />
            <span>Income</span>
            <input type="checkbox" />
            <span>Outcome</span>
          </div>
          <div className="flex w-full justify-end items-center">
            <button onClick={handleModal} className="bg-green-500 h-8 w-8 flex items-center justify-center rounded-full hover:bg-green-700">
              +
            </button>
          </div>
        </div>
      </div>
      <div
        id="feed-container"
        className="w-full h-full border-2 border-border overflow-y-auto rounded-lg md:mb-20"
      >
        {Object.entries(groupedTransactions).map(
          ([date, transactions], index) => (
            <React.Fragment key={index}>
              <div className="text-center p-2">{date}</div>
              {(transactions as Transaction[]).map(
                (transaction, innerIndex) => (
                  <FeedCard key={innerIndex} transaction={transaction} deleteTransaction={deleteTransaction} handleEditModal={handleEditModal} />
                )
              )}
            </React.Fragment>
          )
        )}
      </div>
      <Modal
        modalVisible={modal}
        handleModal={handleModal}
        updateTransaction={updateTransaction}
        update={update}
        transactionEdit={transactionEdit}
      />
    </div>
  );
};

export default Feed;
