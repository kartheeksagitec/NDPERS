$(function () {
 
    var iframebody = $('iframe').filter(":visible").contents().find("body");
  
    //$(document).on("contextmenu", "label,input,select,span", function (e) {
    //    if ($(e.target).hasClass("GridProp")) {
    //        SetVal($(e.target).prev("table")[0].id, e);
    //        e.preventDefault();
    //    }
    //    else if ($(e.target).hasClass("WizProp")) {
    //        SetVal($(e.target).prev(".swMain")[0].id, e);
    //        e.preventDefault();
    //    }
    //    else {
    //        SetVal(e.target.id, e);
    //        e.preventDefault();
    //    }
    //});

    //$(document).on("drop", "label, input, select, span", function (e) {
    //    if (e.target.id != "") {
    //        SetVal(e.target.id, e);
    //    }
    //});

    iframebody.on("keydown", function (e) {
        if (e.ctrlKey && e.keyCode == 83) {
            if ((e.target.localName == "input" && $(e.target).attr("type") == "text") || (e.target.localName == "select")) {
                $(e.target).blur();
            }
            $("#btnSave").click();
            return false;
        }
    });

    //function SetVal(value, e) {
    //    var y = $(e.currentTarget).offset().top;
    //    $("#ControlID").css("top", y + "px");
    //    $("#ControlID2").css("top", y + "px");
    //    $("#ControlID").val(value);
    //    $("#ControlID").focus();
    //    $("#ControlID").trigger("blur");
    //    $("#ControlID2").focus();
    //}
    //$(document).on("contextmenu", "th", function (e) {
    //    //console.log(e.target.tagName);
    //    var tableid = $(this).closest("table")[0].id;
    //    var thtext = $(this).text();
    //    var thindex = $(this).index();
    //    SetVal(tableid + "~" + thtext + "~" + thindex, e);
    //    e.preventDefault();
    //});


    //$(document).on("contextmenu", "td", function (e) {
    //    //console.log(e.target.tagName);
    //    var table = $(this).closest("table");
    //    var tableid = table[0].id;
    //    var tdindex = $(this).index();
    //    var thtext = table.find("th:eq(" + tdindex + ")").text();;
    //    var controlIndex = $(e.target).index();
    //    SetVal(tableid + "~" + thtext + "~" + tdindex + "~" + controlIndex, e);
    //    e.preventDefault();
    //});

    //$(document).on("contextmenu", ".k-link", function (e) {
    //    //console.log(e.target.tagName);
    //    var panel = $(this).closest(".k-widget");
    //    SetVal(panel[0].id, e);
    //    e.preventDefault();
    //});

    //$(document).on("contextmenu", "li[role=tab]", function (e) {
    //    //console.log(e.target.tagName);
    //    var tab = $(this).closest("li");
    //    SetVal(tab[0].id.replace("_Header", ""), e);
    //    e.preventDefault();
    //});

    //$(document).on("contextmenu", "li[class='steplink']", function (e) {
    //    //console.log(e.target.tagName);
    //    SetVal($(this).attr("linkfor"), e);
    //    e.preventDefault();
    //});

    setTimeout(function () {
      //  createWizard();

        var iframeBody = $('iframe').filter(":visible").contents().find("body");

        //iframeBody.find(".tabstrip").each(function () {
        //    $(this).kendoTabStrip({
        //        animation: false
        //    });
        //});

        //iframeBody.find("ul[controltype='panelbar']").each(function (e) {
        //    $(this).kendoPanelBar({
        //        expandMode: "single",
        //        animation: false
        //    });
        //    var panelbar = $(this).data("kendoPanelBar");
        //    panelbar.expand($("li", panelbar.element));
        //});

       

        //var inputElements = iframeBody.find("input");
        //inputElements.prop("disabled", true);

        //var dropdowns = iframeBody.find("select");
        //dropdowns.prop("disabled", true);

        //var hyperLinks = iframeBody.find("a");
        //hyperLinks.prop("disabled", true);

        var $emptyLabels = iframeBody.find("label");
        $emptyLabels.each(function () {
            if (this.textContent == "") {
                if (this.id == "") {
                    this.textContent = "Empty Label";
                    $(this).css("Color", "Gray");
                    $(this).css("font-style", "italic");
                }
                else {
                    this.textContent = this.id;
                }
                if ($(this).css('display') && $(this).css('display').toLowerCase() == "none") {
                    $(this).addClass("invisible-label");
                }
            }
        });

        iframebody.find("table").each(function (e) {
            if ((this.id != "" || this.id != undefined) && $(this).find("tr").length == 2) {
                $(this).css("float", "left");
                $(this).after("<label class='GridProp'> </label>");
            }
            $(this).css({ "border-width": "4px" });
        });

        iframebody.find("td>span").each(function (e) {
            if ($(this).id != "" && $(this).text() == "") {
                $(this).text('#' + this.id);
            }
        });
        iframebody.find("td>a").each(function (e) {
            if (this.id == "") {
                this.textContent = "Empty Label";
                $(this).css("Color", "Gray");
                $(this).css("font-style", "italic");
                $(this).css("text-decoration", "underline");
            }
            else {
                this.textContent = this.id;
            }
             $(this).prop("disabled", true);
        
        });

        iframebody.find("span[id]").each(function (e) {
            if ($(this).id != "" && $(this).text() == "") {
                $(this).text('#' + this.id);
            }
        });


        iframebody.find("td>div").each(function (e) {
            if ($(this).id != "" && $(this).text() == "") {
                $(this).text('#' + this.id);
            }
        });

        iframebody.find("div[id]").each(function (e) {
            if ($(this).id != "" && $.trim($(this).html()) == "") {
                $(this).text('#' + this.id);
            }
        });

        //iframebody.find("button").each(function () {
        //    $(this).prop("disabled", true);
        //});

        //iframebody.find("input[type='submit']").each(function () {
        //    $(this).prop("type", "button");
        //});

        iframebody.find("input[type='image']").each(function () {
            $(this).prop("alt", "Image Button");
        });
        iframebody.find("*").each(function () {
            if (this.id) {
               this.title = this.id;
            }
            if ($(this).hasClass("hideControl")) {
                $(this).removeClass("hideControl");
                if ($(this).is(":button") && !$(this).hasClass("button") && $(this).attr("class") == "") {
                    $(this).addClass("button");
                }
            }
        });

    }, 1000);

    //function createWizard() {
    //    //var UL = $("<ul></ul>");

    //    var $swMain = $('iframe').filter(":visible").contents().find("body").find(".swMain");

    //    //$swMain.find("[controltype='stepDiv']").each(function () {
    //    //    var item = $("<li class='steplink' linkfor=" + this.id + "></li>");
    //    //    item.append($("<a isdone='1' href=#" + this.id + "></a>").append($("<h4>" + $(this).attr("title") + "<h4>")));
    //    //    UL.append(item);
    //    //});
    //    //$swMain.append(UL);
    //    $swMain.smartWizard({ transitionEffect: 'slide' });
    //    $swMain.after("<label class='WizProp'> </label>");
    //    $swMain.find(".actionBar").hide();
    //}

    //$("[id]").each(function (e) {
    //   // $(this).attr("title", this.id);
    //});

    iframebody.find("table").each(function () {
        var current = $(this);
        if (current) {
            current.removeClass("s-grid-container");
            current.addClass("s-grid fluid-table");
            current.parent('div').addClass("s-gridparent s-grid-container");
        }
    });
});