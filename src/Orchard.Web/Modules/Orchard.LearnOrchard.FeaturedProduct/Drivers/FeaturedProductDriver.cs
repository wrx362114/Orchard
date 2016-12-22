using Orchard.ContentManagement.Drivers;
using Orchard.LearnOrchard.FeaturedProduct.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.LearnOrchard.FeaturedProduct.Drivers
{
    public class FeaturedProductDriver : ContentPartDriver<FeaturedProductPart>
    {
        protected override DriverResult Display(FeaturedProductPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_FeaturedProduct", () =>
              shapeHelper.Parts_FeaturedProduct());
        }
    }
}