using AutoMapper;
using StackExchange.Redis;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;
using TalabatEx.DTOs;

namespace TalabatEx.Helpers
{
    public class MappingProfiles : Profile
    {

        public MappingProfiles() 
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand.Name))
                .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.Name))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductPictureUrlResolver>()).ReverseMap();
            //.ForMember(d => d.PictureUrl, o => o.MapFrom(s => $"{_config["ApiBaseUrl"]}/{s.PictureUrl}")).ReverseMap();

            CreateMap<CustomerBasketDto, CustomerBasket>().ReverseMap();
            CreateMap<BasketItemDto, BasketItem>().ReverseMap();

            CreateMap<Talabat.Core.Entities.Identity.Address, AddressDto>().ReverseMap();
            CreateMap<Talabat.Core.Entities.Order.Address, AddressDto>().ReverseMap();

            CreateMap<OrderAg, OrderToReturnDto>()
                .ForMember(d => d.DeliveryMethod, O => O.MapFrom(S => S.DeliveryMethod.ShortName))
                .ForMember(d => d.DeliveryCost, O => O.MapFrom(S => S.DeliveryMethod.Cost))
                .ForMember(d => d.Total, O => O.MapFrom(S => S.GetTotal()))
                .ForMember(d => d.ShipToAddress, O => O.MapFrom(S => S.ShippingAddress));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, O => O.MapFrom(S => S.Product.ProductId))
                .ForMember(d => d.ProductName, O => O.MapFrom(S => S.Product.ProductName))
                .ForMember(d => d.PictureUrl, O => O.MapFrom(S => S.Product.PictureUrl))
                .ForMember(d => d.PictureUrl, O => O.MapFrom<OrderItemPictureUrlResolver>());

        }
    }
}
