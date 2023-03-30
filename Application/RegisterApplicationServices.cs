using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class RegisterApplicationServices
    {
        public static IServiceCollection RegisterApplication(this IServiceCollection services)
        {
            return services;
        }

    }
}
