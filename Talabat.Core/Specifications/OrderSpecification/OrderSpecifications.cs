using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order;
using Talabat.Core.Spacifications;

namespace Talabat.Core.Specifications.OrderSpecification
{
    public class OrderSpecifications : BaseSpecification<OrderAg>
    {
        public OrderSpecifications(string email) : base(o => o.BuyerEmail == email)
        {
            Includes.Add(o => o.DeliveryMethod);
            Includes.Add(o => o.Items);

            AddOrderByDescending(o => o.OrderDate);
        }
        public OrderSpecifications(string email, int oId) : base(o => o.BuyerEmail == email && o.Id == oId)
        {
            Includes.Add(o => o.DeliveryMethod);
            Includes.Add(o => o.Items);
        }
    }
}
