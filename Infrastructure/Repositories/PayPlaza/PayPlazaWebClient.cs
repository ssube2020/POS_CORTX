using System.Net;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using YourNamespace;
using Newtonsoft.Json;
using System.Security;
using System.Security.Authentication;
using System.Runtime.ConstrainedExecution;
using System.Net.Http;

namespace Infrastructure.Repositories.PayPlaza
{
    public class PayPlazaWebClient
    {
        private X509Certificate2 _certificate = null;
        private SslStream _securityContext;
        private SslStream _sslStream;
        private bool _sslStreamInitialized = false;

        private StateContext _context;
        private bool _securityContextInitialized = false;

        private PayPlazaConfig _payplazaconfig;

        private int _amount;
        private bool _activeTransaction = false;
        private readonly PayPlazaLongPollingClient _client;
        private readonly string _ecrId = "ECR_STIBBONSBT";
        private string _terminalWakeUpIpAddress;
        private int _terminalWakeUpIpPort;
        public PayPlazaWebClient(PayPlazaLongPollingClient client, PayPlazaConfig payplazaconfig)
        {
            _client = client;
            _payplazaconfig = payplazaconfig;
        }

        public SslStream SecurityContext()
        {
            if (_securityContext == null)
            {
                X509Certificate2 cert = Certificate;
                X509Chain chain = new X509Chain();
                chain.Build(cert);

                TcpClient tcpClient = new TcpClient("example.com", 443);
                _securityContext = new SslStream(tcpClient.GetStream(), false);
                _securityContext.AuthenticateAsClient("example.com", new X509Certificate2Collection(cert), SslProtocols.Tls, false);
            }
            return _securityContext;
        }

        private SslStream SecurityContext1()
        {
            if (!_sslStreamInitialized)
            {
                string p12pass = _payplazaconfig.P12Pass;

                X509Certificate2 certificate = new X509Certificate2("payplaza-testpms-client-2022b.p12", p12pass);

                TcpClient client = new TcpClient("localhost", 443);
                _securityContext = new SslStream(client.GetStream(), false);
                _securityContext.AuthenticateAsClient("localhost", new X509Certificate2Collection(new X509Certificate2[] { certificate }), SslProtocols.Tls, false);

                _sslStreamInitialized = true;
            }

            return _securityContext;
        }

        public X509Certificate2 Certificate
        {
            get
            {
                if (!_securityContextInitialized)
                {
                    var p12pass = PayPlazaConfig.Instance.P12Pass;

                    var certificatePath = "payplaza-testpms-client-2022b.p12";
                    _certificate = new X509Certificate2(certificatePath, p12pass);

                    _securityContextInitialized = true;
                }

                return _certificate;
            }
        }

        public string ecrId()
        {
            return _ecrId;
        }

        public string baseUri()
        {
            return $"https://cert.payplaza.com:32001/ecr-ws-dev4/rest/ecr/4.0/{ecrId}";
        }

        public bool ActiveTransaction
        {
            get { return _activeTransaction; }
        }

        private bool ValidateCertificate(HttpRequestMessage request, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public async Task PrintRequestMessage()
        {
            Console.WriteLine("printRequestMessage");
            using var client = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            });

            var request = new HttpRequestMessage(HttpMethod.Post, baseUri());
            request.Content = new StringContent($"{{\"printRequestMessage\":{{\"ecrId\":\"{ecrId}\"}}}}", Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);

            var decodedResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            PayPlazaLongPollingClient.Instance.HandleMessage(decodedResponse);
        }

        public async Task TransactionStartMessage()
        {
            Console.WriteLine("transactionStartMessage");
            _activeTransaction = true;

            using var client = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            });

            var request = new HttpRequestMessage(HttpMethod.Post, baseUri());
            request.Content = new StringContent("{\"transactionStartMessage\":{\"type\":\"PURCHASE\",\"amount\":{\"units\":" + _amount + ",\"currencyCode\":\"EUR\"},\"orderRef\":\"151102-00240\"}}", Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);

            var decodedResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            PayPlazaLongPollingClient.Instance.HandleMessage(decodedResponse);
        }

        public async Task StartPayment(StateContext context, int amount)
        {
            _context = context;
            _amount = amount;

            Console.WriteLine($"Starting PayPlaza ECRWS transaction for {_context} amount: {_amount}");

            var handler = new HttpClientHandler();
            var client = new HttpClient(handler);
            var content = new StringContent($"{{\"connectRequestMessage\":{{\"ecrId\":\"{ecrId}\"}}}}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync(baseUri(), content);
            var connectResultCode = string.Empty;
            var responseContent = await response.Content.ReadAsStringAsync();

            var decodedResponse = JObject.Parse(responseContent);
            if (decodedResponse.ContainsKey("connectCompleteMessage"))
            {
                var message = decodedResponse["connectCompleteMessage"];

                var connectResultCodeToken = message.SelectToken("connectResultCode");
                var terminalWakeUpIpAddress = message.SelectToken("terminalWakeUpIpAddress");
                var terminalWakeUpIpPort = message.SelectToken("terminalWakeUpIpPort");

                if (connectResultCodeToken != null)
                {
                    connectResultCode = connectResultCodeToken.ToString();
                }
                if (terminalWakeUpIpAddress != null)
                {
                    _terminalWakeUpIpAddress = terminalWakeUpIpAddress.ToString();
                }
                if (terminalWakeUpIpPort != null)
                {
                    _terminalWakeUpIpPort = (int)terminalWakeUpIpPort;
                }
            }

            if (connectResultCode == "PRINT_RECEIPT")
            {
                await PrintRequestMessage();
            }
            else
            {
                WakeUpTerminal();
                await TransactionStartMessage();
                if (_activeTransaction)
                {
                    _client.StartPolling(_context);
                }
            }
        }

        public async Task WakeUpTerminal()
        {
            if (null != _terminalWakeUpIpAddress && _terminalWakeUpIpPort != 0)
            {
                Console.WriteLine($"Current terminal wakeup address: {_terminalWakeUpIpAddress}:{_terminalWakeUpIpPort}");

                var ipAddress = IPAddress.Parse(_terminalWakeUpIpAddress);
                var endpoint = new IPEndPoint(ipAddress, _terminalWakeUpIpPort);

                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(endpoint.Address, endpoint.Port);
                }
            }
        }

        public async Task PrintResultMessage()
        {
            Console.WriteLine("printResultMessage");

            var clientHandler = new HttpClientHandler();
            var client = new HttpClient(clientHandler);

            try
            {
                var content = new StringContent("{\"printResultMessage\":{\"resultCode\":\"SUCCESS\"}}");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(baseUri(), content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);

                var decodedResponse = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseString);
                _client.HandleMessage(decodedResponse);
            }
            finally
            {
                client.Dispose();
                clientHandler.Dispose();
            }
        }

        public async Task DisconnectMessage()
        {
            if (!_activeTransaction)
            {
                return;
            }

            Console.WriteLine("disconnectMessage");
            _activeTransaction = false;

            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var client = new HttpClient(clientHandler);
            try
            {
                client.BaseAddress = new Uri(baseUri());
                var request = new HttpRequestMessage(HttpMethod.Post, "");
                request.Content = new StringContent("{\"disconnectMessage\":{\"ecrId\":\"" + ecrId + "\"}}",
                    Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);

                    var decodedResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);
                    _client.HandleMessage(decodedResponse);
                }
            }
            finally
            {
                client.Dispose();
            }
        }

    }
}

