jQuery.fn.extend({
    hiteSlide: function (options) {
        var obj = $(this);
        var n = 0;
        var h = options.height || 200;
        var w = options.width || 200;
        var timer = options.timer || 3000;
        obj.css({ width: w + "px", height: h + "px" });

        var imgLiObj = $("ol li", obj);
        var count = imgLiObj.size();

        var text = '<ul>';
        for (var i = 0; i < count; i++) {
            text += '<li>' + (i + 1) + '</li>';
        }
        text += '</ul>';

        var textObj = $("ul", obj.append(text));
        var textLiObj = $("li", textObj);

        if (count > 0) { imgLiObj.eq(0).show(); textLiObj.eq(0).addClass("on"); }

        if (count === 1) { return; }

        textLiObj.mouseover(function () {
            i = $(this).text() - 1;
            n = i;
            if (n >= count) return;
            imgLiObj.filter(":visible").fadeOut(200, function () { imgLiObj.eq(n).fadeIn(300); });
            $(this).addClass("on").siblings().removeClass("on");
        });
        imgLiObj.hover(function () { window.clearInterval(_autoTimer); }, function () { _autoTimer = setInterval(go, timer); });


        _autoTimer = setInterval(go, timer);
        function go() {
            n = n >= (count - 1) ? 0 : ++n;
            textLiObj.eq(n).trigger('mouseover');
        }
    }
});