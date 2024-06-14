const feedCard = ({ transaction, deleteTransaction, handleEditModal: putTransaction }) => {
    const color = transaction.category.colorhex;

    var value = transaction.value;

    if(value < 0)
        value = value*-1;
  
    const formatTransactionTime = (timeString) => {
      const date = new Date(timeString);
      const hours = date.getHours();
      const minutes = date.getMinutes();
      return `${hours}:${minutes < 10 ? '0' : ''}${minutes}`;
    };

    const handleDelete = () => {
      deleteTransaction(transaction.transactionId)
    }

    const handleUpdate = () => {
      const transactionUpdate = {
        transactionId: transaction.transactionId,
        bankAccountId: transaction.bankAccountId,
        categoryId: transaction.categoryId,
        name: transaction.name,
        value: transaction.value,
        date: transaction.date,
        description: transaction.description,
        recurring: transaction.recurring,
        type: transaction.type,
      }
      putTransaction(transactionUpdate)
    }
  
    return (
      <div className="flex items-center p-2">
        <div className="w-8 h-8 rounded-full mr-2 border-[1px] border-black" style={{ backgroundColor: color }}></div>
        <div className="flex flex-grow items-center">
          <div className="flex flex-col">
            <span>{transaction.name}</span>
            <span className="text-xs">{transaction.category.name} - {formatTransactionTime(transaction.date)} - {transaction.bankAccount.name}</span>
          </div>
        </div>
        <div className="text-base">US$ {value}</div>
        <button
          className="bg-red-500 ml-2 text-white h-6 w-6 flex items-center justify-center rounded-full hover:bg-red-700"
          onClick={handleDelete}
        >
          &times;
        </button>
        <button
          className="bg-yellow-500 ml-2 text-white h-6 w-6 flex items-center justify-center rounded-full hover:bg-yellow-700"
          onClick={handleUpdate}
        >
          E
        </button>
      </div>
    );
  };
  
  export default feedCard;
  