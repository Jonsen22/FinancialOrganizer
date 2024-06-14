"use client";

import React, { useEffect, useState } from "react";
import { getBankAccounts, getCategories, postTransaction } from "../hooks/Transactionsdata";

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

const Modal = ({ modalVisible, handleModal, updateTransaction }) => {
  const [isVisible, setIsVisible] = useState(false);
  const [bankAccounts, setBankAccounts] = useState(null);
  const [categories, setCategories] = useState(null);
  const [name, setName] = useState("");
  const [value, setValue] = useState(0);
  const [date, setDate] = useState("");
  const [description, setDescription] = useState("");
  const [recurring, setRecurring] = useState("");
  const [type, setType] = useState("");
  const [bankAccountId, setBankAccountId] = useState<number>();
  const [categoryId, setCategoryId] = useState<number>();

  useEffect(() => {
    setIsVisible(modalVisible);
  }, [modalVisible]);

  const handleCloseModal = () => {
    setIsVisible(false);
    handleModal();
  };

  useEffect(() => {
    if (localStorage.getItem("AccessToken")) {
      if (!bankAccounts) {
        fetchBankAccounts();
      }
    }
  }, [bankAccounts]);

  useEffect(() => {
    if (localStorage.getItem("AccessToken")) {
      if (!categories) {
        fetchCategories();
      }
    }
  }, [categories]);


  const fetchBankAccounts = async () => {
    if (localStorage.getItem("AccessToken")) {
      const bankAccountsData = await getBankAccounts(
        localStorage.getItem("AccessToken")
      );
      setBankAccounts(bankAccountsData);
    } else {
      console.error("User is not authenticated");
    }
  };

  const fetchCategories = async () => {
    if (localStorage.getItem("AccessToken")) {
      const categoriesData = await getCategories(
        localStorage.getItem("AccessToken")
      );
      setCategories(categoriesData);
    } else {
      console.error("User is not authenticated");
    }
  };

  if (!isVisible) {
    return null;
  }

  const handleSubmit = async() => {
    var updatedValue = value;
    if(type == "Expense")
      updatedValue = value*-1

    const transaction:Transaction = {
      bankAccountId: bankAccountId,
      categoryId: categoryId,
      name: name,
      value: updatedValue,
      date: date,
      description: description,
      recurring: recurring,
      type: type
    }

    const response = await postTransaction(localStorage.getItem("AccessToken"), transaction)
    console.log(response);
    updateTransaction();
  }


  const handleNameChange = (event: React.SetStateAction<string>) => {
    setName(event);
  };

  const handleDescriptionChange = (event: React.SetStateAction<string>) => {
    setDescription(event);
  };

  const handleValueChange = (event: React.SetStateAction<number>) => {
    setValue(event);
  };

  const handleValueDate = (event: React.SetStateAction<string>) => {
    setDate(event);
  };

  const handleRecurringChange = (
    event: React.ChangeEvent<HTMLSelectElement>
  ) => {
    const recurring = event.target.value;
    setRecurring(recurring);
  };

  const handleTypeChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
    const type = event.target.value;
    setType(type);
  };

  const handleBankAccountChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
    const bankAccount = parseInt(event.target.value);

    setBankAccountId(bankAccount);
  };

  const handleCategoryChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
    const category = parseInt(event.target.value);
    console.log(category)
    setCategoryId(category);
  };

  return (
    <div className="fixed flex items-center justify-center z-40 top-0 left-0 w-full h-full bg-gray-800 bg-opacity-80">
      <div className="bg-card rounded-lg border-2 border-border w-full max-w-md mx-auto md:max-w-lg  p-4">
        <div className="flex flex-col justify-start h-full">
          <div className="flex items-end justify-end mr-2">
            <button onClick={handleCloseModal}>Close</button>
          </div>
          <div className="flex flex-col justify-center items-start w-full space-y-2">
            <label >Name:</label>
            <input className="rounded p-1" onChange={(e) => handleNameChange(e.target.value)}></input>
            <label>Value:</label>
            <input
                className="rounded p-1"
              type="number"
              onChange={(e) => handleValueChange(parseInt(e.target.value))}
            ></input>
            <label>Date:</label>
            <input
            className="rounded p-1"
              onChange={(e) => handleValueDate(e.target.value)}
              type="datetime-local"
            ></input>
            <label >Description:</label>
            <input className="rounded p-1" onChange={(e) => handleDescriptionChange(e.target.value)}></input>
            <label>Recurring:</label>
            <select onChange={handleRecurringChange} className="rounded p-1">
              <option value={"Y"}>Yes</option>
              <option value={"N"}>No</option>
            </select>
            <label>Type:</label>
            <select onChange={handleTypeChange} className="rounded p-1">
              <option value={"Income"}>Income</option>
              <option value={"Expense"}>Expense</option>
            </select>
            <div className="flex w-full flex-row items-center justify-between">
              <label>Bank Account:</label>
              <button className="rounded-full p-[4px] bg-grape text-text">Edit Bank Account</button>
            </div>
            <select onChange={handleBankAccountChange} className="rounded p-1">
              {bankAccounts.map((bankAccount, index) => (
                <option key={index} value={bankAccount.bankAccountId}>
                  {bankAccount.name}
                </option>
              ))}
            </select>
            <div className="flex w-full flex-row items-center justify-between">
              <label>Category:</label>
              <button className="rounded-full p-[4px] bg-grape text-text">Edit category</button>
            </div>
            <select onChange={handleCategoryChange} className="rounded p-1">
              {categories.map((category, index) => (
                <option key={index} value={parseInt(category.categoryId)}>{category.name}</option>
              ))}
            </select>
          </div>
        </div>
        <div className="flex items-center justify-center">
              <button onClick={handleSubmit} className="rounded-full p-[5px] bg-grape text-text">Submit</button>
        </div>
      </div>
    </div>
  );
};

export default Modal;
