using EDDY.IS.Base;
using EDDY.IS.Core.Logging;
using EDDY.IS.EmsLeadEngine.Core.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Core
{
    public class LogManager
    {
        public static void LogJournalInfo(Guid transactionId, Guid? subTransactionId, string message, params object[] arguments)
        {
            if (Settings.Default.LogInformationMessages)
            {
                new TransactionJournal(transactionId,
                    subTransactionId,
                    ISApplication.EMSLeadService,
                    TransactionJournalMessageLevel.Log,
                    message,
                    arguments
                    ).Save();
            }
        }

        public static void LogJournalInfo(Guid transactionId, string message, params object[] arguments)
        {
            LogJournalInfo(transactionId, null, message, arguments);
        }

        public static void LogJournalWarning(Guid transactionId, Guid? subTransactionId, string message, string formattedMessage, string additionalInformation)
        {
            new TransactionJournal(transactionId,
                subTransactionId,
                ISApplication.EMSLeadService,
                TransactionJournalMessageLevel.Warning,
                message,
                formattedMessage,
                additionalInformation
                ).Save();
        }

        public static void LogJournalWarning(Guid transactionId, string message, string formattedMessage, string additionalInformation)
        {
            LogJournalWarning(transactionId, null, message, formattedMessage, additionalInformation);
        }

        public static void LogJournalException(Guid transactionId, Guid? subTransactionId, Exception ex, params object[] arguments)
        {
            new TransactionJournal(transactionId,
                subTransactionId,
                ISApplication.EMSLeadService,
                ex,
                arguments).Save();
        }

        public static void LogJournalException(Guid transactionId, Exception ex, params object[] arguments)
        {
            LogJournalException(transactionId, null, ex, arguments);
        }

        public static void LogException(Exception ex)
        {
            new ISException(ISApplication.EMSLeadService, ex).Save();
        }

        public static void LogException(Exception ex, string additionalInfo, params object[] arguments)
        {
            new ISException(ISApplication.EMSLeadService, ex, additionalInfo, arguments).Save();
        }
    }
}
