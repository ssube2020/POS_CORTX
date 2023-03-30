using Core.Models;

namespace Core.Interfaces.PayPlaza
{
    public interface IPayPlaza
    {
        Task<ConnectCompleteMessage> ConnectRequestMessage(ConnectRequestMessage connectRequestMessage);
        Task<TransactionCompleteMessage> TransactionStartMessage(TransactionStartMessage transactionStartMessage);
        Task<PrintCommandMessage> PrintRequestMessage(PrintRequestMessage printResultMessage);
        Task<PrintCompleteMessage> PrintResultMessage(PrintResultMessage printResultMessage);
        Task<DisconnectMessage> DisconnectMessage(DisconnectMessage disconnectMessage);
    }
}
