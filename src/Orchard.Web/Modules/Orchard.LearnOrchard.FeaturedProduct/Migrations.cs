using System;
using System.Collections.Generic;
using System.Data;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.LearnOrchard.FeaturedProduct.Models;
using Orchard.Core.Common.Models;
using Orchard.Widgets.Models;

namespace Orchard.LearnOrchard.FeaturedProduct {
    public class Migrations : DataMigrationImpl
    { 
        public int Create()
        {
            ContentDefinitionManager.AlterTypeDefinition(
              "FeaturedProductWidget", cfg => cfg
                .WithSetting("Stereotype", "Widget")
                .WithPart(typeof(FeaturedProductPart).Name)
                .WithPart(typeof(CommonPart).Name)
                .WithPart(typeof(WidgetPart).Name));
            return 1;
        }
    }
}