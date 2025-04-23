using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order;


namespace Talabat.Core.Service.Contract
{
    public interface IOrderService
    {
        Task<OrderAg> CreateOrderAsync(string buyerEmail, string basketId, int DeliveryMethod, Address address);
        //Task<OrderToReturnDto> CreateOrderAsyncc(OrderDto);
        Task<IReadOnlyList<OrderAg>> GetOrdersForSpecificUserAsync(string buyerEmail);

        Task<OrderAg> GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId);

        //Task<IReadOnlyCollection<DeliveryMethod>> GetDeliveryMethodAsync();

    }
}
