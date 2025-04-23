using AutoMapper;
using Talabat.Core.Entities.Order;
using TalabatEx.DTOs;

namespace TalabatEx.Helpers
{
    public class OrderItemPictureUrlResolver : IValueResolver<OrderItem, OrderItemDto, string>
    {
        public IConfiguration Configuration { get; }
        public OrderItemPictureUrlResolver(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Product.PictureUrl))
                return $"{Configuration["ApiBaseUrl"]}{source.Product.PictureUrl}";
            return null;
        }
    }
}
