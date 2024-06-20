using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.DTOS.Transactions;

namespace Kallum.Mappers
{
    public class GetTransactionHisotryMapper
    {
        public static TransactionHistoryDto ToTransactionHistoryDto(TransactionHistoryDto transactionHistoryDto)
        {
            return new TransactionHistoryDto
            {
                Amount = transactionHistoryDto.Amount,
                Currency = transactionHistoryDto.Currency,
                CurrencySymbol = transactionHistoryDto.CurrencySymbol,
                Date = transactionHistoryDto.Date,
                Reciever = transactionHistoryDto.Reciever,
                Sender = transactionHistoryDto.Sender,
                TransactionDescription = transactionHistoryDto.TransactionDescription,
                TransactionHistoryId = transactionHistoryDto.TransactionHistoryId,

            };
        }
    }
}