using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Spacifications;

namespace Talabat.Core.Specifications.ProductSpecification
{
    public class ProductsWithFilterationForCountSpecification : BaseSpecification<Product>
    {
        public ProductsWithFilterationForCountSpecification(ProductSpecParams specParams)
            : base(p => (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search.ToLower())) &&
                        (!specParams.BrandId.HasValue || p.BrandId == specParams.BrandId.Value) &&
                        (!specParams.CategoryId.HasValue || p.CategoryId == specParams.CategoryId.Value)
                  )
        {

        }
    }
}
