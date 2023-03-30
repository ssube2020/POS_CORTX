using Infrastructure.Repositories.PayPlaza;
using Newtonsoft.Json;

public class PayPlazaLongPollingClient
{
    private StateContext _context;
    private PayPlazaWebClient _payplazaClient;
    private bool _continuePolling = false;

    private static readonly PayPlazaLongPollingClient instance = new PayPlazaLongPollingClient();

    public static PayPlazaLongPollingClient Instance
    {
        get
        {
            return instance;
        }
    }

    public async Task StartPolling(StateContext context)
    {
        _context = context;
        Console.WriteLine($"Starting PayPlaza ECRWS long polling for {_context}");

        var httpClientHandler = new HttpClientHandler();
        var httpClient = new HttpClient(httpClientHandler);
        _continuePolling = true;

        try
        {
            while (_continuePolling)
            {
                Console.WriteLine("GET");
                var response = await httpClient.GetAsync(_payplazaClient.baseUri());
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);

                var decodedResponse = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseString);
                await HandleMessage(decodedResponse);
            }
        }
        finally
        {
            Console.WriteLine("Polling stopped");
            httpClient.Dispose();
            httpClientHandler.Dispose();
        }
    }

    public async Task HandleMessage(Dictionary<string, dynamic> message)
    {
        var messageId = message.Keys.FirstOrDefault();
        Console.WriteLine($"Message: {messageId}");

        switch (messageId)
        {
            case "rejectMessage":
                break;
            case "abortMessage":
                break;
            case "timeoutMessage":
                break;
            case "transactionUpdateMessage":
                //LincstationWebClient.CancelPayment();
                break;
            case "transactionCompleteMessage":
                if (message.TryGetValue("transactionCompleteMessage", out dynamic body))
                {
                    if (body.ContainsKey("resultCode"))
                    {
                        var resultCode = body["resultCode"].Value<string>();
                        if (resultCode == "SUCCESS")
                        {
                            Console.WriteLine("PayPlaza ECRWS Transaction Success!");
                        }
                    }
                    if (body.ContainsKey("nextAction"))
                    {
                        var nextAction = body["nextAction"].Value<string>();
                        if (nextAction == "sendDisconnectMessage")
                        {
                            await CancelPolling();
                            await _payplazaClient.DisconnectMessage();
                        }
                    }
                }
                break;
            case "disconnectMessage":
                await CancelPolling();
                break;
            case "printCommandMessage":
                await CancelPolling();
                await _payplazaClient.PrintResultMessage();
                break;
            case "printCompleteMessage":
                if (message.TryGetValue("printCompleteMessage", out dynamic printCompleteMessage))
                {
                    if (printCompleteMessage.ContainsKey("nextAction"))
                    {
                        var nextAction = printCompleteMessage["nextAction"].Value<string>();
                        if (nextAction == "sendDisconnectMessage")
                        {
                            await CancelPolling();
                            await _payplazaClient.DisconnectMessage();
                        }
                    }
                }
                break;
        }
    }

    public async Task CancelPolling()
    {
        Console.WriteLine("Cancelling ECRWS long polling");
        _continuePolling = false;
    }
}
