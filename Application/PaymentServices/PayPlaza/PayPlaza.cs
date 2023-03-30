using Core.Interfaces.PayPlaza;
using Core.Models;
using System.Net.Http.Headers;
using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace Application.PaymentServices.PayPlaza
{
    public class PayPlaza : IPayPlaza
    {

        public Task<ConnectCompleteMessage> ConnectRequestMessage(ConnectRequestMessage connectRequestMessage)
        {
            throw new NotImplementedException();
        }

        public Task<DisconnectMessage> DisconnectMessage(DisconnectMessage disconnectMessage)
        {
            throw new NotImplementedException();
        }

        public Task<PrintCommandMessage> PrintRequestMessage(PrintRequestMessage printResultMessage)
        {
            throw new NotImplementedException();
        }

        public async Task PrintResultMessage()
        {
            throw new NotImplementedException();
        }


        public Task<PrintCompleteMessage> PrintResultMessage(PrintResultMessage printResultMessage)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionCompleteMessage> TransactionStartMessage(TransactionStartMessage transactionStartMessage)
        {
            throw new NotImplementedException();
        }
    }
}
