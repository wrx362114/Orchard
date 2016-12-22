using System;
using System.Web;
using System.Web.Mvc;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Localization.Services;
using Orchard.Mvc.Html;
using Orchard.Services;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Diagnostics;
using HashidsNet;
using System.Collections.Generic;

namespace Orchard.LearnOrchard.FeaturedProduct.Shapes
{
    public class ArticleShapes : IDependency
    {
        private readonly IClock _clock;
        private readonly IDateLocalizationServices _dateLocalizationServices;
        private readonly IDateTimeFormatProvider _dateTimeLocalization;

        public ArticleShapes(
            IClock clock,
            IDateLocalizationServices dateLocalizationServices,
            IDateTimeFormatProvider dateTimeLocalization
            )
        {
            _clock = clock;
            _dateLocalizationServices = dateLocalizationServices;
            _dateTimeLocalization = dateTimeLocalization;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }


        [Shape("spec-article")]
        public IHtmlString SpecArticle(dynamic Display)
        {
            List<ContentManagement.ContentPart> parts = Display.ViewDataContainer.Model.ContentItem.Parts;

            return new MvcHtmlString("作品标签需添加ID属性.并且ID格式为:Article-{Id}");
        }

    }
}
