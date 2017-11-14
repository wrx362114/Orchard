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
using MySql.Data.MySqlClient;

namespace Orchard.LearnOrchard.FeaturedProduct.Shapes
{
    public class ArtworkShapes : IDependency
    {
        private readonly IClock _clock;
        private readonly IDateLocalizationServices _dateLocalizationServices;
        private readonly IDateTimeFormatProvider _dateTimeLocalization;

        public ArtworkShapes(
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

        [Shape("artwork")]
        public IHtmlString Artwork(dynamic Display)
        {
            try
            {
                string div = Display.ViewContext.Writer.ToString();
                //Display.ViewContext.Writer = "<div class='article'>";
                Debug.Print(div);
                var articleId = int.Parse(Regex.Match(div, "[0-9]+").Value);
                if (Regex.IsMatch(div, "artwork-([0-9]+)"))
                {
                    var idstr = Regex.Match(div, "(?<=artwork-)[0-9]+").Value;
                    int id = int.Parse(idstr);
                    return new MvcHtmlString(ArticleText(articleId));
                }
            }
            catch (Exception ex)
            {
                return new MvcHtmlString("服务发生异常" + ex.ToString());
            }
            return new MvcHtmlString("作品标签需添加ID属性.并且ID格式为:Artwork-{Id}");
        }
        string ArticleText(int id)
        {
            var model = new ArticleModel();
            using (var db = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["YWDatabase"].ConnectionString))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
	t1.`id` AS `Id`,
    t1.`artwork_name`AS `Name`,
    t1.`mainimg_size_height` AS `Height`,
    t1.`mainimg_size_width` AS `Width`,
    t1.`price_discount` AS `Price`,
    t1.`mainimg_filename` AS `ImgName`,
    t1.`artwork_material` AS `Type`,
    t1.`recommend` AS `Story`,
    t2.`id` AS `AuthorId`,
    t2.`artist_name` AS `AuthorName`,
    t1.`is_sold` AS `Sold`,
    t1.`price_sale` AS `OPrice`,
    t1.`is_auction_goods` AS `IsAuctionGoods`,
    t1.`is_collect` AS `IsCollect`
FROM artwork AS t1
	LEFT JOIN artist AS t2 on t2.Id=t1.artist_id
WHERE t1.id=@id
LIMIT 0,1".Replace("@id", id.ToString());
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return "找不到作品";
                        }
                        model.Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                        model.Name = reader.IsDBNull(1) ? "" : reader.GetString(1);
                        model.Height = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2);
                        model.Width = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3);
                        model.Price = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4);
                        var img = reader.IsDBNull(5) ? "" : reader.GetString(5);
                        model.PreviewImgUrl = "https://cdn.ywart.com/yw/" + img + "_test02";
                        model.Type = reader.IsDBNull(6) ? "" : reader.GetString(6);
                        model.Story = reader.IsDBNull(7) ? "" : reader.GetString(7);
                        model.AuthorId = reader.IsDBNull(8) ? 0 : reader.GetInt32(8);
                        model.AuthorName = reader.IsDBNull(9) ? "" : reader.GetString(9);
                        model.Sold = !(reader.IsDBNull(10) || reader.GetInt32(10) == 0);
                        model.OriginPrice = reader.IsDBNull(11) ? 0 : reader.GetDecimal(11);
                        model.IsAuctionGoods = reader.IsDBNull(12) ? false : reader.GetBoolean(12);
                        model.IsCollect = reader.GetBoolean(13);


                        model.ArticleUrl = "https://www.ywart.com/artworks/" + HashHelper.GetArtworkHashByID(model.Id);
                        model.AuthorUrl = "https://www.ywart.com/artist/" + HashHelper.GetArtistHashByID(model.AuthorId);
                    }
                }
            }
            var template =
@"<article class='article'>
    <div class='topstorytext'><div class='diamond'></div>@story</div>
    <div class='preview'>
        <div class='sold @sold'>已售</div>
        <a href='@articleUrl'>
            <img src='@previewUrl' alt='艺术品预览图' />
            <div class='story'>
                <div class='text'>@story</div>
            </div>
        </a>
    </div>
    <div class='info'>
        <div class='author'>
            <a href='@authorUrl'>@authorName</a>
        </div>
        <div class='main'>
            <a href='@articleUrl'>
                <span class='name'>@articleName</span>
                <span class='type'>@articleType</span>
                <span class='size'>@articleSize</span>
            </a>
        </div>
        <div class='price'><strong  class='@hasred'>￥@articlePrice </strong><span class='@hidden'>￥@originPrice</span></div>
    </div>
</article>";
            return template
                .Replace("@articleUrl", model.ArticleUrl)
                .Replace("@previewUrl", model.PreviewImgUrl)
                .Replace("@authorUrl", model.AuthorUrl)
                .Replace("@authorName", model.AuthorName)
                .Replace("@articleName", model.Name)
                .Replace("@articleType", model.Type)
                .Replace("@story", model.Story)
                .Replace("@articleSize", model.Width + "×" + model.Height + "cm")
                .Replace("@articlePrice", model.IsCollect ? "vip点击查看" : (model.IsAuctionGoods ? "暂无价格" : String.Format("{0:N}", model.Price).Replace(".00", "")))
                .Replace("@originPrice", model.IsCollect ? "vip点击查看" : (model.IsAuctionGoods ? "暂无价格" : String.Format("{0:N}", model.OriginPrice).Replace(".00", "")))
                .Replace("@hidden", model.Price == model.OriginPrice ? "hidden" : "originprice")
                .Replace("@hasred", model.Price != model.OriginPrice ? "redprice" : "")
                .Replace("@sold", model.Sold ? "" : "hidden");
        }
        private class ArticleModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ArticleUrl { get; set; }
            public string PreviewImgUrl { get; set; }
            public decimal Price { get; set; }
            public decimal OriginPrice { get; set; }
            public bool IsAuctionGoods { get; set; }
            public string Story { get; set; }
            public int AuthorId { get; set; }
            public string AuthorName { get; set; }
            public string AuthorUrl { get; set; }
            public decimal Height { get; set; }
            public decimal Width { get; set; }
            public string Type { get; set; }
            public bool Sold { get; set; }
            public bool IsCollect { get; set; }
        }
        public class HashHelper
        {
            //初始版本
            static Hashids hsArtwork_v0 = new Hashids("this is my yishupin 2015 salt");
            static Hashids hsArtist_v0 = new Hashids("this is my yiwang salt");

            //20170228修改为v1
            static Hashids hsArtwork_v1 = new Hashids("this is artwork 2017 salt", alphabet: "abcdefghijklmnopqrstuvwxyz1234567890");
            static Hashids hsArtist_v1 = new Hashids("this is artist2017 salt", alphabet: "abcdefghijklmnopqrstuvwxyz1234567890");

            static Hashids hsOrder = new Hashids("this is order2017 salt", alphabet: "abcdefghijklmnopqrstuvwxyz1234567890");

            public static string GetArtworkHashByID(long p)
            {
                return "1" + hsArtwork_v1.EncodeLong(p + 99999);
            }

            public static long GetArtworkIDByHash(string key)
            {
                char ver = key[0];
                long result = 0;
                switch (ver)
                {
                    case '0':
                        result = hsArtwork_v0.DecodeLong(key.Substring(1))[0] - 99999;
                        break;
                    case '1':
                        result = hsArtwork_v1.DecodeLong(key.Substring(1))[0] - 99999;
                        break;
                    default:
                        break;
                }
                return result;
            }

            /// <summary>
            /// 根据艺术家的ID获取艺术家ID的hash值
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static string GetArtistHashByID(long id)
            {
                return "1" + hsArtist_v1.EncodeLong(id + 100000);
            }

            /// <summary>
            /// 根据艺术家ID的Hash获取艺术家ID
            /// </summary>
            /// <param name="hash"></param>
            /// <returns></returns>
            public static long GetArtistIDByHash(string hash)
            {
                char ver = hash[0];
                long result = 0;
                switch (ver)
                {
                    case '0':
                        result = hsArtist_v0.DecodeLong(hash.Substring(1))[0] - 100000;
                        break;
                    case '1':
                        result = hsArtist_v1.DecodeLong(hash.Substring(1))[0] - 100000;
                        break;
                    default:
                        break;
                }
                return result;
            }

            /// <summary>
            /// 订单ID编码
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static string OrderIdEncode(long id)
            {
                return hsOrder.EncodeLong(id + 9999999);
            }

            /// <summary>
            /// 订单ID解码
            /// </summary>
            /// <param name="code"></param>
            /// <returns></returns>
            public static long OrderIdDecode(string code)
            {
                return hsOrder.DecodeLong(code)[0] - 9999999;
            }

        }

    }
}
