using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.Core;
using Talabat.Core.Entities.Order;
using Talabat.Core.Service.Contract;
using TalabatEx.DTOs;
using TalabatEx.Errors;

namespace TalabatEx.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IOrderService service, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _orderService = service;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<OrderToReturnDto?>> CreateOrder(OrderDto orderDto)
        {
            var buyrEmail = User.FindFirstValue(ClaimTypes.Email);
            var mappedAddress = _mapper.Map<AddressDto, Address>(orderDto.ShippingAddress);

            var userOrder = await _orderService.CreateOrderAsync(buyrEmail, orderDto.BasketId, orderDto.DeliveryMethodId ?? 0, mappedAddress);

            if (userOrder == null)
            {
                return BadRequest(new ApiResponse(400, "There is a Problem With Your Order"));
            }
            else
            {
                return Ok(_mapper.Map<OrderAg, OrderToReturnDto>(userOrder));
            }
        }

        [ProducesResponseType(typeof(IReadOnlyList<OrderToReturnDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var buyrEmail = User.FindFirstValue(ClaimTypes.Email);
            var name = User.FindFirstValue(ClaimTypes.Name);
            var orders = await _orderService.GetOrdersForSpecificUserAsync(buyrEmail);
            if (orders.Count() == 0)
            {
                return NotFound(new ApiResponse(404, $"There is no orders for {name}"));
            }
            else
            {
                var mappedOrders = _mapper.Map<IReadOnlyList<OrderAg>, IReadOnlyList<OrderToReturnDto>>(orders);
                return Ok(mappedOrders);
            }
        }

        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderForUser(int id)
        {
            var buyrEmail = User.FindFirstValue(ClaimTypes.Email);
            var name = User.FindFirstValue(ClaimTypes.Name);
            var order = await _orderService.GetOrderByIdForSpecificUserAsync(buyrEmail, id);
            if (order is null)
            {
                return NotFound(new ApiResponse(404, $"There is no order for {name} with OrderId: {id}"));
            }
            else
            {
                var mappedOrder = _mapper.Map<OrderAg, OrderToReturnDto>(order);
                return Ok(mappedOrder);
            }
        }

        [ProducesResponseType(typeof(DeliveryMethod), StatusCodes.Status200OK)]
        [HttpGet("DeliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var DeliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            return Ok(DeliveryMethods);
        }

    }
}
