"use client";

import React, { useEffect, useState } from "react";
import {
  BarChart,
  CartesianGrid,
  XAxis,
  YAxis,
  Tooltip,
  Legend,
  Bar,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
} from "recharts";
import Select, { MultiValue, components } from "react-select";

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

interface CategoryExpense {
  name: string;
  value: number;
  colorhex: string;
}

interface GraphsProps {
  data: Balance[];
  transactions: Transaction[];
}

type OptionType = {
  value: number;
  label: string;
  income: number;
  expense: number;
};


const CustomOption = (props) => {
  return (
    <components.Option {...props}>
      <input type="checkbox" checked={props.isSelected} onChange={() => null} />{" "}
      <label>{props.label}</label>
    </components.Option>
  );
};

// Custom MultiValue component to hide selected options from the select bar
const CustomMultiValue = (props) => {
  return null;
};

const Graphs: React.FC<GraphsProps> = ({ data, transactions=[] }) => {
  const [options, setOptions] = useState<OptionType[]>([]);
  const [selectedOptions, setSelectedOptions] = useState<OptionType[]>([]);
  const [categoryExpenses, setCategoryExpenses] = useState<CategoryExpense[]>(
    []
  );

  const dataToSelectOptions = (data: Balance[]) => {
    const tempOptions: OptionType[] = data.map((item) => ({
      value: item.id,
      label: item.name,
      income: item.income,
      expense: item.expense,
    }));
    setOptions(tempOptions);
  };

  if (!Array.isArray(transactions)) {
    transactions = [];
  }

  const calculateCategoryExpenses = (transactions: Transaction[]) => {
    const expenses = transactions.reduce((acc, transaction) => {
      if (transaction.type === "Expense") {
        const category = transaction.category.name;
        if (!acc[category]) {
          acc[category] = { value: 0, colorhex: transaction.category.colorhex };
        }
        acc[category].value += -1 * transaction.value;
      }
      return acc;
    }, {});

    const result = Object.keys(expenses).map((key) => ({
      name: key,
      value: expenses[key].value,
      colorhex: expenses[key].colorhex,
    }));

    setCategoryExpenses(result);
  };



  useEffect(() => {
    dataToSelectOptions(data);
  }, [data]);

  useEffect(() => {
    calculateCategoryExpenses(transactions);
  }, [transactions]);

  const handleChange = (selected: MultiValue<OptionType>) => {
    setSelectedOptions(selected as OptionType[]);
  };

  return (
    <div className=" w-full h-full md:h-full flex flex-col items-center justify-start">
      <div className="w-full h-[50%] bg-white p-4 rounded">
      <Select
        options={options}
        value={selectedOptions}
        isMulti
        onChange={handleChange}
        components={{ Option: CustomOption }}
        closeMenuOnSelect={false}
        hideSelectedOptions={false}
        placeholder="Please select options"
        className="w-full"
      />
        <div className="h-full w-full">
          <ResponsiveContainer width="100%" height="100%">
            <BarChart
              layout="vertical"
              width={500}
              height={300}
              data={selectedOptions.length ? selectedOptions : options}
              margin={{
                top: 5,
                right: 30,
                left: 20,
                bottom: 5,
              }}
            >
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis type="number" />
              <YAxis dataKey="label" type="category" />
              <Tooltip />
              <Legend />
              <Bar dataKey="income" fill="#8884d8" />
              <Bar dataKey="expense" fill="#82ca9d" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      <div className="w-full h-[50%] bg-white p-4 rounded">
        <div className="text-center">Expenses by Category</div>
        <ResponsiveContainer width="100%" height="100%">
          <PieChart>
            <Pie
              data={categoryExpenses}
              dataKey="value"
              nameKey="name"
              cx="50%"
              cy="50%"
              outerRadius={100}
              fill="#8884d8"
              label={({ name, percent }) => `${(percent * 100).toFixed(2)}%`}
            >
              {categoryExpenses.map((entry, index) => (
                <Cell key={`cell-${index}`} fill={entry.colorhex} />
              ))}
            </Pie>
            <Tooltip
              formatter={(value) => `$${value}`} // Add a dollar sign to the tooltip value
            />
          </PieChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
};

export default Graphs;
