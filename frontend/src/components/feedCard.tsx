const feedCard = ({ transaction }) => {
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
      </div>
    );
  };
  
  export default feedCard;
  