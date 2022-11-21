$(function () {
    initSwitchGrid();
});

function initSwitchGrid() {
    var $window = $(window);
    var $body = $("body");
    var ssScrollSetting = {
        height: 'calc(100vh - 22.5rem)', //可滾動區域高度
        size: '12px', //元件寬度
        color: '#23569d', //滾動條顏色
        opacity: 1, //滾動條透明度
        alwaysVisible: true, //是否 始終顯示元件
        disableFadeOut: false, //是否 滑鼠經過可滾動區域時顯示元件，離開時隱藏元件
        railVisible: true, //是否 顯示軌道
        railColor: '#333', //軌道顏色
        railOpacity: .35, //軌道透明度
        railDraggable: true, //是否 滾動條可拖動
        railClass: 'slimScrollRail', //軌道div類名 
        barClass: 'slimScrollBar', //滾動條div類名
        wrapperClass: 'slimScrollDiv', //外包div類名
        allowPageScroll: true, //是否 使用滾輪到達頂端/底端時，滾動視窗
        wheelStep: 20, //滾輪滾動量
        touchScrollStep: 200, //滾動量當使用者使用手勢
        borderRadius: '7px', //滾動條圓角
        railBorderRadius: '7px' //軌道圓角
    }


    var AuditssScrollSetting = {
        height: 'calc(100vh - 25rem)', //可滾動區域高度
        size: '12px', //元件寬度
        color: '#23569d', //滾動條顏色
        opacity: 1, //滾動條透明度
        alwaysVisible: true, //是否 始終顯示元件
        disableFadeOut: false, //是否 滑鼠經過可滾動區域時顯示元件，離開時隱藏元件
        railVisible: true, //是否 顯示軌道
        railColor: '#333', //軌道顏色
        railOpacity: .35, //軌道透明度
        railDraggable: true, //是否 滾動條可拖動
        railClass: 'slimScrollRail', //軌道div類名 
        barClass: 'slimScrollBar', //滾動條div類名
        wrapperClass: 'slimScrollDiv', //外包div類名
        allowPageScroll: true, //是否 使用滾輪到達頂端/底端時，滾動視窗
        wheelStep: 20, //滾輪滾動量
        touchScrollStep: 200, //滾動量當使用者使用手勢
        borderRadius: '7px', //滾動條圓角
        railBorderRadius: '7px' //軌道圓角
    }


    //電腦版可產生於班表交換區產生捲軸。行動版不產生。
    if ($window.width() > 768) {
        $(".ss-main").slimScroll(ssScrollSetting);
    }
    //"審核申請單" 頁的班表選擇區產生捲軸
    if ($window.width() > 768 && $("#content-body").hasClass("page-audit")) {
        $(".ss-main").slimScroll(AuditssScrollSetting);
    }


    //視窗檢視RWD時會切換捲軸是否產生
    $window.resize(function () {
        if ($window.width() <= 768) {
            $(".ss-main").slimScroll({ destroy: true }).css("height", "auto");
        } else {
            $(".ss-main").slimScroll(ssScrollSetting);
        }

        if ($window.width() <= 768 && $("#content-body").hasClass("page-audit")) {
            $(".ss-main").slimScroll({ destroy: true }).css("height", "auto");
        } else {
            $(".ss-main").slimScroll(AuditssScrollSetting);
        }
    });




    // // Get the header
    // var header = document.getElementById("footer");

    // // Get the offset position of the navbar
    // var sticky = header.offsetTop;
    // alert("sticky = " + sticky)



    var ss_header_posi = $(".ss-header").offset().top - 65;

    //行動版往上捲動捲軸可固定班表Header
    $window.scroll(function () {
        if ($body.hasClass("body-small")) {

            if ($window.scrollTop() > ss_header_posi) {
                $(".ss-header").addClass("ss-header-sticky");
            } else {
                $(".ss-header").removeClass("ss-header-sticky");
            }
        }
    });


    //同一組航班滑入後，顯示為同一組的樣式
    $(".ss-selectable").hover(
        function () {
            //滑入
            if ($(this).attr("data-selectablegroup") != undefined) {
                var groupNo = $(this).attr("data-selectablegroup");

                $("[data-selectablegroup = " + groupNo + "]").each(function (i) {
                    $(this).addClass("ss-js-group-hover");
                });
            }
        },
        function () {
            //滑出
            if ($(this).hasClass("ss-js-group-hover")) {
                var groupNo = $(this).attr("data-selectablegroup");

                $("[data-selectablegroup = " + groupNo + "]").each(function (i) {
                    $(this).removeClass("ss-js-group-hover");
                });
            }
        }
    );


    //可選航班點擊時(分為單日與多日群組)
    $(".ss-selectable").click(function () {
        //如果可選的班表有分組(連續多日為相關聯的航班班表)，例如: selectable-group01
        if ($(this).attr("data-selectablegroup") != undefined) {

            var groupNo = $(this).attr("data-selectablegroup");
            if (!$(this).hasClass("ss-selected")) {
                $("[data-selectablegroup = " + groupNo + "]").each(function (i) {
                    $(this).addClass("ss-selected").find(".ss-pickdate  :checkbox").prop("checked", true);;
                });
            } else {
                $("[data-selectablegroup = " + groupNo + "]").each(function (i) {
                    $(this).removeClass("ss-selected").find(".ss-pickdate  :checkbox").prop("checked", false);
                });
            }

        } else {
            //如果為一般單日可選航班班表
            if (!$(this).hasClass("ss-selected")) {
                $(this).addClass("ss-selected").find(".ss-pickdate  :checkbox").prop("checked", true);
            } else {
                $(this).removeClass("ss-selected").find(".ss-pickdate  :checkbox").prop("checked", false);
            }
        }

    });


    // label綁定checkbox，沒按到checkbox但按到label時仍可選取以方便使用，但需取消此功能，因與按整塊ss-1pdata衝突。
    $(".ss-pickdate label").click(function (e) {
        e.preventDefault();
    });


    $(".page-audit .ss-selectable").click(function () {

    })

}