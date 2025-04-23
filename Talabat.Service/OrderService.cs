using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Service.Contract;
using Talabat.Core.Specifications.OrderSpecification;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }
        public async Task<OrderAg> CreateOrderAsync(string buyerEmail, string basketId, int DeliveryMethodId, Address address)
        {
            //1.Get Basket From Basket Repo
            var Basket = await _basketRepository.GetBasketAsync(basketId);

            //2.Get Selected Items at Basket From Product Repo
            var orderItems = new List<OrderItem>();
            if(Basket?.Items?.Count > 0)
            {
                foreach (var item in Basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetAsync(item.Id);

                    var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);

                    var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);

                    orderItems.Add(orderItem);
                }
            }

            //3.Calculate SubTotal
            var subTotal = orderItems.Sum(item => item.Price * item.Quantity);

            //4.Get Delivery Method From DeliveryMethod Repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(DeliveryMethodId);

            var orderSpec = new OrderWithPaymentIntentSpecification(Basket.PaymentIntentId);
            var exOrder = await _unitOfWork.Repository<OrderAg>().GetWithSpecAsync(orderSpec);

            if(exOrder is not null)
            {
                _unitOfWork.Repository<OrderAg>().Delete(exOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            }

            //5.Create Order
            var order = new OrderAg(buyerEmail, address, deliveryMethod, orderItems, subTotal, Basket.PaymentIntentId);

            //6.Add Order Locally
            await _unitOfWork.Repository<OrderAg>().AddAsync(order);

            //7.Save Order To Database[ToDo]
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0) return null;


            return order;
        }



        public async Task<OrderAg> GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId)
        {
            var spec = new OrderSpecifications(buyerEmail, orderId);
            var order = await _unitOfWork.Repository<OrderAg>().GetWithSpecAsync(spec);
            return order;
        }

        public async Task<IReadOnlyList<OrderAg>> GetOrdersForSpecificUserAsync(string buyerEmail)
        {
            var spec = new OrderSpecifications(buyerEmail);
            var orders = await _unitOfWork.Repository<OrderAg>().GetAllWithSpecAsync(spec);
            return (IReadOnlyList<OrderAg>)orders;
        }
    }
}
