/*Create 30/8/55 ; Update 5/11/55 (BoSS)*/

/*#region Member */

//ใช้บอก Index ของ Grid ที่ใช้งาน
var g_no = 0;
//ใช้สำหรับ Method ในการแสดง Result หลังจากเปลี่ยนแปลงเช่น บึนทึก หรือแก้ไข
var g_success = "GridSuccess";
//ใช้สำหรับแสดงผลการทำงาน Result ที่เกี่ยวข้องกับ Grid
var g_result = ".gridresult";
//ใช้นับ Index ของ Grid
var g_length = 0;
//ใช้บอกตำแหน่งของ Class ที่ต้องการให้ค่าที่ List มาไปแสดงที่ต้องการ
var g_content = ".GridContent";
//หน้าที่เราอยู่
var g_pageindex = ".hidPageIndex";
//หน้าที่เราอยู่ปัจจุบันใช้สำหรับเอาไว้เทียบไม่ให้เปลี่ยนแปลงหน้าเป็นหน้าเดิมและอื่นๆ
var g_pagenow = ".hidPageNow";
//ค่า PageSize ที่ใช้สำหรับกำหนด PageSize จริงๆ สำหรับส่งค่าไปยัง Controller
var g_pagesize = ".hidPageSize";
//ค่า PageSize ของ DropDownList นำค่าของ DropDownList ไปเปลี่ยนที่ g_ddlpagesize
var g_ddlpagesize = ".ddlPageSize";
//ชื่อที่ซ่อนไว้สำหรับกด Submit ให้ง่ายขึ้น
var g_hidsubmit = ".hidBtnSubmit";
var g_totalpage = ".hidTotalPage";
//ใช้สำหรับต่อ String ในการ Sort ส่งไปยัง Controller
var g_keyworter = ".hidKeySorter";
//ใช้สำหรับต่อ String ในการ Order ส่งไปยัง Controller
var g_keyorder = ".hidKeyOrder";
//ใช้สำหรับบอกการเรียงจากน้อยไปมาก
var g_isortasc = "isortasc";
//ใช้สำหรับบอกการเรียงจากมากไปน้อย
var g_isortdesc = "isortdesc";
var KeywordGroup = "Start";

/*#endregion */

/*#region On Document Ready ( เมื่อโหลดหน้าเสร็จให้ทำ ) */
$(function () {
    //สำหรับเช็คว่าเรากำลังทำงานกับ Grid ไหน
    $("form[data-ajax-success='" + g_success + "']").live("mouseover", function () {
        g_no = $(this).index("form[data-ajax-success='" + g_success + "']");
    });
    $(".txtGoToPage").live('keypress', function (e) {
        if ((e.which >= 48 && e.which <= 57) || e.which == 8 || e.which ==13) {
            return true;
        } else {
            return false;
        }
    });
    $(".cbxItem").live('click', function () {
        if ($(".cbxItem:checked").length == $(".cbxItem").length) {
            $("#checkAll").attr('checked', true);
        }
        else {
            $("#checkAll").attr('checked', false);
        }
    });
    g_length = $("form[data-ajax-success='" + g_success + "']").length;
    for (i = 0; i < g_length; i++) {
        GridStartPage(i);
    }
    $(".btnsearch").live('click', function () {
        OpenLoading(true);
        $(g_pageindex).val(1);
        $("#KeywordGroup input[type=checkbox]:checked,#KeywordGroup input[type=radio]:checked").removeClass("checked");
        $("#KeywordGroup input[type=checkbox]:checked,#KeywordGroup input[type=radio]:checked").addClass("checked");
        $("#KeywordGroup select option").removeClass("selected");
        $("#KeywordGroup select option:selected").addClass("selected");
        if (KeywordGroup != "Start") { KeywordGroup = $("#KeywordGroup").html(); }

    });
    //KeywordGroup ต้องอยู่ด้านล่างสุดของ jQuery เท่านั้น

    $("#KeywordGroup input[type=text]").live('keyup', function () {
        $(this).addClass("val").attr("val", $(this).val());
    });
    KeywordGroup = $("#KeywordGroup").html();
});
/*#endregion */

//Function

/*#region//สั่งให้ .cbxitem เมื่อคลิก ให้เช็ค ทั้งหมด*/
function CheckAll(obj, strclass) {
    if (strclass == "" || strclass == null) {
        //ให้ Class Default เป็น cbxItem
        strclass = "cbxItem";
    }
    if (obj.attr('checked') == true || obj.attr('checked') == 'checked') {
        $("." + strclass).attr('checked', true);
    }
    else {
        $("." + strclass).attr('checked', false);
    }
}
/*#endregion*/

function SetKeywordGroup() {
    if ($("#KeywordGroup").length > 0 && KeywordGroup != "Start") {
        $("#KeywordGroup").html(KeywordGroup);
        $("#KeywordGroup option.selected").attr("selected", "selected");
        $("#KeywordGroup input.checked").attr("checked", "checked");
        for (x = 0; x < $("#KeywordGroup input.val").length; x++) {
            $("#KeywordGroup input.val").eq(x).val($("#KeywordGroup input.val").eq(x).attr("val"));
        }
    }
}

/*#region About Pagging*/
function submitPaging(PageIndex) {
    OpenLoading(true);
    $(".gridrefresh").eq(g_no).addClass("gridloading");
    $(g_pageindex).eq(g_no).val(PageIndex);
    //ตัวช่วย KeywordGroup
    SetKeywordGroup();
    $(g_hidsubmit).eq(g_no).click();
    $(g_result).eq(g_no).hide();
}


function NumPage(PageIndex, PageSize) {
    if (PageIndex != $(g_pagenow).eq(g_no).val())
        submitPaging(PageIndex, PageSize);
}

function ChangePageSize() {
    $(g_pagesize).eq(g_no).val($(g_ddlpagesize).eq(g_no).val());
    submitPaging(1);
    $("#autoHeight").slideDown(function () {
        $("#sidebar").height($("#autoHeight").height());
        $("#main").height($("#autoHeight").height());
    });
}

function keyPageIndex(obj) {
    $(g_pageindex).eq(g_no).val(obj.val());
}

function GoToPage() {
    PageSize = parseInt($(g_pagesize).eq(g_no).val());
    PageIndex = parseInt($(g_pageindex).eq(g_no).val());
    totalPages = parseInt($(g_totalpage).eq(g_no).val());
    if (PageIndex <= totalPages && PageIndex > 0) {
        if (PageIndex != parseInt($(g_pagenow).eq(g_no).val())) {
            submitPaging(PageIndex, PageSize);
            return false;
        }
        else {
            return false;
        }
    } else {
        //if (PageIndex < 1) {
            submitPaging(1, PageSize);
        //} else {
         //   submitPaging(totalPages, PageSize);
        //}
        return false;
    }
}

function GridRefresh() {
    window.location.reload();
}

function GridSuccess(content) {

    $(g_content).eq(g_no).html(content);
    //SetAddUrl();
    SorterIcon();
    //    GridLoaded();
    CheckGrid();
    //CloseLoading();
    $("input[type=submit]").removeAttr('disabled');
    OpenLoading(false);
    $("#autoHeight").slideDown(function () {
        $("#sidebar").height($("#autoHeight").height());
        $("#main").height($("#autoHeight").height());
    });

    //เฉพาะ message center
    var checked = getCookie("checkedContact");
    if (checked != "" && checked != null) {
        var arr = checked.split(",");
        for (var i = 0; i < arr.length; i++) {
            $(".c-" + arr[i]).attr("checked", true);
        }
    }
}

function TableSort(obj) {
    //OpenLoading();
    var KeySort = obj.closest('th').attr('sort');
    if (obj.hasClass(g_isortdesc) == true) {
        $(g_keyworter).eq(g_no).val(KeySort);
        $(g_keyorder).eq(g_no).val('ASC');
    } else if (obj.hasClass(g_isortasc) == true) {
        $(g_keyworter).eq(g_no).val(KeySort);
        $(g_keyorder).eq(g_no).val('DESC');
    }
    $(g_hidsubmit).eq(g_no).click();
}

function SorterIcon() {
    $('.grid').eq(g_no).find('thead tr.header th').each(function (index) {
        if ($(this).attr("sort") == $(g_keyworter).eq(g_no).val()) {
            if ($(g_keyorder).eq(g_no).val() == 'ASC') {
                $(this).find('div.Isort').addClass(g_isortasc).removeClass(g_isortdesc);
            }
            else if ($(g_keyorder).eq(g_no).val() == 'DESC') {
                $(this).find('div.Isort').addClass(g_isortdesc).removeClass(g_isortasc);
            }
        }
    });
}
/*#endregion*/

/*#region ส่วนของ Result ที่ได้จากการทำงานต่างๆของ Grid เช่น ChangeStatus เป็นต้น */
function SuccessBox(msgBox) {
    msgBox = (msgBox == 'undefined' || msgBox == null || msgBox == "" || msgBox == '[object Object]') ? label.vldedit_success : msgBox;
    CaseBox(msgBox);
}

function WarningBox(msgBox) {
    msgBox = (msgBox == 'undefined' || msgBox == null || msgBox == "") ? label.vldplease_select : msgBox;
    CaseBox(msgBox, "iwarning");
}

function ErrorBox(msgBox) {
    msgBox = (msgBox == 'undefined' || msgBox == null || msgBox == "") ? label.vldedit_unsuccess : msgBox;
    CaseBox(msgBox, "ierror");
}

function CaseBox(msgBox, Case) {
    var b_header = "Success : ";
    var b_class = "success";


    if (Case == "iwarning") {
        var b_header = "Warning! ";
        var b_class = "block";
    }
    else if (Case == "ierror") {
        var b_header = "Error! ";
        var b_class = "error";
    }

    if ($(g_result).length > 0) {
        $("#information").addClass(' alert-' + b_class);
        $("#information > strong").text(b_header);
        $("#information").text(msgBox);
        $("#information").fadeIn();
    } else {
        $("#information").addClass(' alert-' + b_class);
        $("#information > strong").text(b_header);
        $("#information > p").text(msgBox);
        $("#information").fadeIn();
    }
   // CloseLoading();
    $("#information").delay(3000).fadeOut(500);
}

function ClearAjaxResult() {
    $(g_result).html("").hide();
}
/*#endregion */

function GridBegin() {
    GridLoading();
}

function GridStartPage(ind) {
    try {
        OpenLoading(true);
        var obj = $("form[data-ajax-success='" + g_success + "']").eq(ind);
        var path = obj.attr("action");
        var method = obj.attr("data-ajax-method");

        var Form = {};
        obj.find('input,textarea,select').each(function () {
            Form[$(this).attr('name')] = $(this).val();
        });
        if (method.toLowerCase() == "post") {
            //bootbox.alert(url + path.substring(1, path.length));
            $.ajax({
                url: "../../../../../../../" + path.substring(1, path.length),
                traditional: true,
                data:Form,
                success: function (data) {
                    $(g_content).eq(ind).html(data).find("table").addClass("StartGrid");
                    // $(".PagingUC").disableSelection();
                    CheckGrid();
                    OpenLoading(false);
                     $("#autoHeight").slideDown(function () {
                        $("#sidebar").height($("#autoHeight").height());
                        $("#main").height($("#autoHeight").height());
                    });
                },
                type: "POST"
            });
        }
    }
    catch (Error) { console.log(Error); }
}

/*#region Function สำหรับเช็คค่าต่างๆใน Gird เช่น เช็คข้อมูลว่าครบตาม Rows ที่ต้องการหรือไม่, ใส่สีสลับ Column,ให้หน้าสุดท้ายหรือหน้าแรกไม่สามารถกดปุ่มได้ เป็นต้น*/
function CheckGrid() {
    var grid = $("table.grid").eq(g_no);
    if ($(g_content).find("table.StartGrid").length > 0) { grid = $(g_content).find("table.StartGrid").removeClass("StartGrid"); }
    var size = grid.next().find("input[name=hidPageSize]").val();
    if (grid.find(".nodata").hasClass("nodata")) {
        var len = grid.find("th").length;
        //bootbox.alert(len);
        grid.find(".nodata > td").attr("colspan", len);
    }
    var g_row = grid.next().find(g_pagesize).val();
    $("#totalList").text(parseInt(grid.find("tr.body").length));
    var cbxIndex = grid.find('thead > tr.header').find('th.cbx').index();
    var listnoIndex = grid.find('thead > tr.header').find('th.listno').index();
    var dateIndex = grid.find('thead > tr.header').find('th.date').index();
    var editbyIndex = grid.find('thead > tr.header').find('th.editby').index();
    var statusIndex = grid.find('thead > tr.header').find('th.status').index();
    var manageIndex = grid.find('thead > tr.header').find('th.manage').index(); 

    var gridObj = grid.find('tbody > tr.body');
    gridObj.find("td:eq(" + cbxIndex + ")").addClass("cbx");
    gridObj.find("td:eq(" + listnoIndex + ")").addClass("listno");
    gridObj.find("td:eq(" + dateIndex + ")").addClass("date");
    gridObj.find("td:eq(" + editbyIndex + ")").addClass("editby");
    gridObj.find("td:eq(" + statusIndex + ")").addClass("status");
    gridObj.find("td:eq(" + manageIndex + ")").addClass("manage");

    $(g_ddlpagesize).eq(g_no).find("option[value=\"" + size + "\"]").attr("selected", "selected");
    $(".paging li").disableSelection();
   

}
/*#endregion*/

//Table Grid & Paging
function SaveStatus(obj, Path, KeyValue) {
    Do_SaveStatus(obj, Path, KeyValue);
}

function Do_SaveStatus(obj, Path, KeyValue) {
    AjaxGridPost(obj, Path, KeyValue);
}

function DeleteData(obj, Path, PrimaryKeyName) {
    Do_DeleteData(obj, Path, PrimaryKeyName);
}

function Do_DeleteData(obj, Path, PrimaryKeyName) {
    AjaxGridPost(obj, Path, PrimaryKeyName);
}

function Do_SaveEx(data) { }

//กดแล้วคลุมเลือกข้อความใน TextBox 
function txtSelect(obj) {
    obj.select();
}
// alert confirm ให้ใส่เองในหน้า view ตอน onclick เพื่อความยืดหยุ่น
function AjaxGridPost(obj, Path, PrimaryKeyName) {
    var ID = [];
    var RowVersion = [];
    if (obj.parent().parent().is("td")) {
        ID[ID.length] = parseInt(obj.parents("tr.body").find(".hidPrimeID").val());
        RowVersion[RowVersion.length] = parseInt(obj.parents("tr.body").find(".hidRowVersion").val());
    } else {
        $('.cbxItem:checked').each(function () {
            ID[ID.length] = $(this).val();
            RowVersion[RowVersion.length] = $(this).parent().find(".hidRowVersion").val();
        });
    }
    if (ID.length > 0) {
        //OpenLoading();
        $.ajax({
            url: url + Path,
            data: { ID: ID, RowVersion: RowVersion, KeyValue: -1, PrimaryKeyName: PrimaryKeyName },
            success: function (data) {
                if (data == "True" || data == true || data == "Success") {
                    $(g_hidsubmit).eq(g_no).click();
                    //SuccessBox();
                    Do_SaveEx(data);
                    return true;
                } else {
                   // CloseLoading();
                    return false;
                }
            },
            error: function () { ErrorBox(); CloseLoading(); return false; },
            type: "POST", traditional: true
        });
    }
    else {
        WarningBox();
        return false;
    }
}


/*#region ListNo,CategoryListNo*/
function MoveDownListNo(obj, Path) {
    ChangeListNo(Path, "+", obj);
}

function MoveUpListNo(obj, Path) {
    ChangeListNo(Path, "-", obj);
}
function ChangeListNo(obj, Path, PrimaryKeyName) {
    var ListNo = [];
    var ID = [];
    var RowVersion = [];
    var Form = {};
        var Grid = $("table.grid").eq(g_no);
        for (x = 0; x < Grid.find(".ListNo").length; x++) {
            if (Grid.find(".ListNo").eq(x).val() != Grid.find(".OldListNo").eq(x).val()) {
                if (isNaN(Grid.find(".ListNo").eq(x).val()) || $.trim(Grid.find(".ListNo").eq(x).val())=="") {
                    Grid.find(".ListNo").eq(x).val(1);
                }
                ListNo[ListNo.length] = parseInt(Grid.find(".ListNo").eq(x).val());
                ID[ID.length] = Grid.find(".hidPrimeID").eq(x).val();
                RowVersion[RowVersion.length] = Grid.find(".hidRowVersion").eq(x).val();
            }
        }
    Form['KeyValue'] = ListNo;
    Form['ID'] = ID;
    Form['RowVersion'] = RowVersion;
    Form["PrimaryKeyName"] = PrimaryKeyName;
    if (ID.length < 1)
        return false;
    //OpenLoading();

    $.ajax({
        url: url + Path,
        data: Form,
        success: function (data) {
            if (data == "True" || data == true || data == "Success") {
                $(g_hidsubmit).eq(g_no).click();
                //SuccessBox();
                Do_SaveEx(data);
                bootbox.alert(label.vldsave_success);
            } else {
                //CloseLoading();
                bootbox.alert(label.vldsave_unsuccess);
            }
        },
        error: function () { ErrorBox(); CloseLoading(); },
        type: "POST", traditional: true
    });

}
/*#endregion*/
function Keywords(ind) {
    var IsBegin = false;
    if (ind == "" || ind == 'undefined' || ind == undefined) {
        ind = 0;
        IsBegin = true;
    }
    
    if ($("#KeywordGroup").length > 0 && IsBegin) {
        var obj = $("form[data-ajax-success='" + g_success + "']").eq(ind).find(".GridContent");
        var KeywordData = "";
        $("#KeywordGroup").find('input,textarea,select').each(function () {
            KeywordData += "<input type='hidden' name='"+$(this).attr('name')+"' value='"+$(this).val()+"'>";
        });
        obj.prepend(KeywordData);
    }
    if(IsBegin)
        $(g_hidsubmit).eq(ind).click();
}

function GetValue() {
    var id = new Array();
    var len = $(".cbxCompID").length;
    // console.log('len : ' + len);
    var j = 0;
    for (var i = 0; i < len; i++) {
        if ($('.cbxCompID').eq(i).attr("checked") == true || $('.cbxCompID').eq(i).attr("checked") == "checked") {
            var status = $(".cbxCompID").eq(i).attr('data-status');
            id[j] = parseInt($(".cbxCompID").eq(i).attr('data-id'), 10);
            j++;
        }
    }
    return id;
}
 