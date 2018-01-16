$(document).ready(function () {
    document.domain = "ywart.com";
    if ($("#article").length > 0) {
        $(".container").css("max-width", 850);
        if ($("#Content article.page header:first p.tags").length > 0) {
            $("#taglist").append($("#Content article.page header:first p.tags"))
        }
    }
    if ($("#topic").length > 0) {
        if ($("#header-img img").length < 2) {
        } else {
            $("#topic-header-img-1").css("background-image", "url('" + $($("#header-img img")[0]).attr("src") + "')");
            $("#topic-header-img-2").css("background-image", "url('" + $($("#header-img img")[1]).attr("src") + "')");
        }
        $(window).resize(function () {
            $("#special-content .preview").height($("#special-content img").height());
        });
        $("#special-content .preview").height($("#special-content img").height());
        setTimeout(function () { $("#special-content .preview").height($("#special-content img").height()); }, 1000);
        setTimeout(function () { $("#special-content .preview").height($("#special-content img").height()); }, 3000);
        var fileref = document.createElement("link")
        fileref.rel = "stylesheet";
        fileref.type = "text/css";
        fileref.href = "/Themes/YwartTheme/Styles/topic.css";
        fileref.media = "screen";
        var headobj = document.getElementsByTagName('head')[0];
        headobj.appendChild(fileref);
    }
    if ($("#article").length > 0 || $("#topic").length > 0) {
        $(".text-field").remove();
        $(".media-library-picker-field").remove();
    }
    if ($("#spec-root").length > 0) {
        $("#spec-root").parent().prev().remove();
    }
    if ($("#free-layout-root").length > 0) {
        var fileref = document.createElement("link")
        fileref.rel = "stylesheet";
        fileref.type = "text/css";
        fileref.href = "/Themes/YwartTheme/Styles/free-layout-root.css";
        fileref.media = "screen";
        var headobj = document.getElementsByTagName('head')[0];
        headobj.appendChild(fileref);
        var bannerimgH = $('#free-layout-root>.banner_img>div>div>img').height();
        console.log(bannerimgH);
        $('article header').height(bannerimgH);
        $('button').click(function () {
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        });
    }
    if (/s=cm/.test(window.location.search)) {
        var logo4cmurl = "//pages.ywart.com/Media/Default/_Profiles/98c9a695/7b090ae0/logo4cm.png";
        var hb = $("header[role=banner]");
        hb.html("<a href='https://www.ywart.com'><img src='//pages.ywart.com/Media/Default/_Profiles/98c9a695/7b090ae0/logo4cm.png' style='width:212px;height:60px;'/></a>");
        hb.css("cssText", "background-color:#fff !important;text-align:center")
    }
});