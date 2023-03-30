
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Infrastructure.Repositories.PayPlaza
{

    public interface IState
    {
        Task payment(int amount);
        Task paymentPartialAccept();
        Task paymentCancel();
        Task nextState(StateContext context);
    }

    public class InitState : IState
    {
        public async Task payment(int amount)
        {
            Console.WriteLine("Payment: {0}", amount);
        }

        public async Task paymentCancel()
        {
            Console.WriteLine("Payment cancelled");
        }

        public async Task paymentPartialAccept()
        {
            Console.WriteLine("Payment partially accepted");
        }

        public async Task nextState(StateContext context)
        {
            context.setState(new PaymentState());
        }
    }

    public class PaymentState : IState
    {
        public async Task payment(int amount)
        {
            Console.WriteLine("Payment: {0}", amount);
        }

        public async Task paymentCancel()
        {
            Console.WriteLine("Payment cancelled");
        }

        public async Task paymentPartialAccept()
        {
            Console.WriteLine("Payment partially accepted");
        }

        public async Task nextState(StateContext context)
        {
            context.setState(new InitState());
        }
    }

    public class StateContext
    {
        private IState _currentState;
        private readonly string _publicFolder = "public";

        public StateContext()
        {
            setState(new InitState());
        }

        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //{
        //    app.UseStaticFiles();
        //    app.UseRouting();

        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapGet("/payment/{amount:int}", async context =>
        //        {
        //            int amount = Convert.ToInt32(context.Request.RouteValues["amount"]);
        //            await _currentState.payment(amount);
        //            await context.Response.WriteAsync(JsonConvert.SerializeObject(new { amount }));
        //        });

        //        endpoints.MapGet("/payment/partial_accept", async context =>
        //        {
        //            await _currentState.paymentPartialAccept();
        //            await context.Response.WriteAsync(JsonConvert.SerializeObject(new { success = true }));
        //        });

        //        endpoints.MapGet("/payment/cancel", async context =>
        //        {
        //            await _currentState.paymentCancel();
        //            await context.Response.WriteAsync(JsonConvert.SerializeObject(new { success = true }));
        //        });

        //        endpoints.MapFallback(context =>
        //        {
        //            string filePath = Path.Combine(Directory.GetCurrentDirectory(), _publicFolder, "index.html");
        //            return context.Response.SendFileAsync(filePath);
        //        });
        //    });
        //}

        public void setState(IState state)
        {
            _currentState = state;
            nextState();
        }

        public async Task nextState()
        {
            await _currentState.nextState(this);
        }
    }

    //public class Program
    //{
    //    public static void Main(string[] args)
    //    {
    //        CreateHostBuilder(args).Build().Run();
    //    }

    //    public static IHostBuilder CreateHostBuilder(string[] args) =>
    //        Host.CreateDefaultBuilder(args)
    //            .ConfigureWebHostDefaults(webBuilder =>
    //            {
    //                webBuilder.UseStartup<Startup>();
    //                webBuilder.UseUrls("http://*:8079");
    //            });
    //}

    //public class Startup
    //{
    //    public void ConfigureServices(IServiceCollection services)
    //    {
    //    }

    //    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    //    {
    //        StateContext stateContext = new StateContext();
    //        stateContext.Configure(app, env);
    //    }
    //}

}
