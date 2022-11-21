// 設定 JavaScript Timer 自動淡出 Bootstrap Alert automatically。
$(document).ready(function () {
    window.setTimeout(function () {
        $(".alert").fadeTo(1500, 0).slideUp(500, function () {
            $(this).remove();
        });
    }, 3000);
});

(function ($) {
    jQuery.fn.setfocus = function () {
        return this.each(function () {
            var dom = this;
            setTimeout(function () {
                try { dom.focus(); } catch (e) { }
            }, 0);
        });
    };
})(jQuery);

// 顯示 Bootstrap Modal
function openBootstrapModal(modalId) {
    if (Page_ClientValidate()) {
        $("#" + modalId).modal('show');//toggle
    }
    else {
        RequiredFieldValidator_CheckAllValidControl(Page_Validators);
        Page_BlockSubmit = false;
        //$("#" + modalId).modal('hide');
    }
}

// 點選[Save]按鈕時，驗證所有的 ASP.NET 控制項。
function RequiredFieldValidator_CheckAllValidControl(val) {
    if (val != undefined) {
        for (i = 0; i < val.length; i++) {
            RequiredFieldValidator_CheckValidControl(val[i].id, val[i].controltovalidate);
        }
    }
}

// 驗証未通過之控制項動態加 is-invalid Css Class，驗證通過則動態移除 is-invalid Css Class
function RequiredFieldValidator_CheckValidControl(rfvId, ctlId) {
    if (rfvId != undefined && ctlId != undefined) {
        var rfv = document.getElementById(rfvId);
        SetControlRequiredMark(ctlId, rfv.isvalid.toString());
        //SetControlRequiredMark(ctlId, rfv.attributes["isvalid"].value);
    }
}

function SetControlRequiredMark(ctlId, isShow) {
    if (isShow != undefined) {

        if (isShow == "false") {
            $("#" + ctlId + "").removeClass("is-valid");
            $("#" + ctlId + "").addClass("is-invalid");
        }
        else {
            $("#" + ctlId + "").removeClass("is-invalid");
            $("#" + ctlId + "").addClass("is-valid");
        }
    }
}

function addClass(elements, className) {
    if (elements != undefined) {
        if (typeof (elements) == "string") {
            elements = document.querySelectorAll(elements);
        }
        else if (elements.tagName) {
            elements = [elements];
        }

        for (var i = 0; i < elements.length; i++) {
            elements[i].classlist.add(className);
        }
    }
}

function removeClass(element, className) {
    if (elements != undefined) {
        if (typeof (elements) == "string") {
            elements = document.querySelectorAll(elements);
        }
        else if (elements.tagName) {
            elements = [elements];
        }

        for (var i = 0; i < elements.length; i++) {
            elements[i].classlist.remove(className);
        }
    }
}
