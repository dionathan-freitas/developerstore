using Ambev.DeveloperEvaluation.Application.Interface.Sales;
using Ambev.DeveloperEvaluation.Application.Services.Sales;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<ISaleEventLogger, SaleEventLogger>();
            services.AddScoped<SaleService>();

            return services;
        }
    }
}
