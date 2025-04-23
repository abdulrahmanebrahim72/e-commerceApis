using Microsoft.AspNetCore.Mvc;
using Talabat.Core;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Service.Contract;
using Talabat.Repository;
using Talabat.Service;
using TalabatEx.Errors;
using TalabatEx.Helpers;

namespace TalabatEx.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
        {
            Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            Services.AddScoped(typeof(IOrderService), typeof(OrderService));

            Services.AddAutoMapper(typeof(MappingProfiles));

            Services.AddScoped<IPaymentService, PaymentService>();


            #region Validation Error
            Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actioncontext) =>
                {
                    var errors = actioncontext.ModelState.Where(p => p.Value.Errors.Count() > 0)
                                                                      .SelectMany(p => p.Value.Errors)
                                                                      .Select(e => e.ErrorMessage)
                                                                      .ToList();

                    var response = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);
                };
            });

            #endregion


            Services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));

            return Services;
        }

        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        public static WebApplication UseSwaggerMiddleware(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            return app;
        }

    }
}
