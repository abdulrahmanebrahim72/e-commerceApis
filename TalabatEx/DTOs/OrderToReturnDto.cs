using Talabat.Core.Entities.Order;

namespace TalabatEx.DTOs
{
    public class OrderToReturnDto
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public AddressDto ShipToAddress { get; set; }
        public string DeliveryMethod { get; set; }
        public decimal DeliveryCost { get; set; }
        public List<OrderItemDto> Items { get; set; } 
        public string PaymentIntentId { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
    }
}
