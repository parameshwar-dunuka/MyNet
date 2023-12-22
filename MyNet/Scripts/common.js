base = {
    url: window.location.origin,
    isLoading:false
}
opertor = { "=": "EQUALTO", "like": "LIKE", "!=": "NOTEQUAL", "<": "LESSERTHAN", "<=": "LESSERTHANEQUALTO", ">": "GREATERTHAN", ">=": "GRATERTHANEQUAL" }

function AjaxRequest(action, controller, pagename, headertext, params, returnType, whereQry, Type, SucessFunction) {
    if (Type == 'undefined') {
        Type="GET"
    }
    var data = {};
    data['AdditionalParams'] = JSON.stringify(params);
    data['PageName'] = pagename;
    data['HeaderText'] = headertext;
    data['WhereQuery'] = whereQry;
    var url = resolveUrl(controller + '/' + action)
    $.ajax({
        url: url,
        type: Type,
        data: data,
        dataType: returnType,
        success: function (data) {
            SucessFunction(data)
        }
    });
}

function SectionAjaxRequest(action, controller, LayoutName, RuleCode, params, contentType,returnType, Type, SucessFunction) {
    if (Type == 'undefined') {
        Type = "GET"
    }
    var data = {};
    data['AdditionalParams'] = JSON.stringify(params);
    data['LayoutName'] = LayoutName;
    data['RuleCode'] = RuleCode;

    $.ajax({
        url: resolveUrl(controller + '/' + action),
        type: Type,
        data: data,
        contentType: contentType,
        dataType: returnType,
        async: false,
        success: function (data) {
            SucessFunction(data, $('#section_' + LayoutName))
        }
    });
}

function resolveUrl(url) {
    if (location.href.includes('/Home/')) {
        url = url.replace('Home/', '')
        url = url.replace(/^\//,'')
    }
    else {
        url = base.url + '/'+url;
    }
    return url;
}

function Cascading(control) {
    var data = {
        sourceElement: control.target.name,
        sourceValue: control.target.value,
        layoutName: $(control.target).closest('.section').data('section-obj').LayoutName
    }
    if (data.sourceValue.toLowerCase() == 'select' || data.sourceValue == 0 || data.layoutName=='') {
        var target = $("[name='" + data.sourceElement + "']").find('span').attr('target')
        $("[name='"+target+"']").html('')
        return
    }
    var Url = resolveUrl('Home/GetCascading')
    $.ajax({
        url: Url,
        type: "GET",
        data: data,
        contentType: "application/json;",
        dataType: "json",
        async: false,
        success: function (data) {
            var response = data;
            $("[name='" + data.targetElement + "']").html('')
            $.each(data.targetValue, function () {
                $("[name='" + data.targetElement + "']").append($("<option></option>").val(this['Value']).html(this['Text']));
            });
            $("[name='" + data.sourceElement + "']").find('span').remove()
            $("[name='" + data.sourceElement + "']").append('<span target="' + data.targetElement +'"></span>')
        },
        error: function (xhr, status, error) {
            console.log(xhr.responseText);
        }
    });
}



function OnFailureLogin() {
    alert("Login Failed..");
}

function onCancel(trgt) {
    isDirty = $(trgt).closest('.section').find('.UDFields').children('#isdirty').length

    if (isDirty >= 1) {
        handlers = []
        handlers.push({ text: 'Yes', action: 'yes()' })
        handlers.push({ text: 'No', action: 'no()' })

        DialogBox('info', 'Changes will be lost. Do you want to continue?', handlers);
    }
    else {
        yes()
    }
}

function DialogBox(css, msg, handlers) {
    var imgurl = "https://cdn-icons-png.flaticon.com/512/189/189664.png";
    var btn = '';
    if (css = 'info') {
        imgurl = "https://cdn-icons-png.flaticon.com/512/189/189664.png"
    }
    for (var i = 0; i < handlers.length; i++) {
        btn=btn+`<button onclick="${handlers[i].action}">${handlers[i].text}</button>`
    }
    var html = `<div class='dialog-box'>
    <div class='dialog-content'>
        <img src=${imgurl} height='60' width='60'></img>
        <p>${msg}</p>
        <div>${btn}</div>
    </div>
    </div>`
    $('div.mymodal').addClass('active')
    $('div.mymodal').append(html)
}

function yes() {
    $('div.mymodal').removeClass('active')
    $('div.mymodal').html('')
}

function no() {
    $('.dialog-box').css('display','none')
}


function OpenPopup(pageName, Header, params, whereQuery) {
    params['AdditionalParams'] = JSON.stringify(params);
    params["PageName"] = pageName;
    
    urls = resolveUrl('Home/UDBuilder')
    $.ajax({
        url: urls,
        type: "GET",
        data: params,
        dataType: "html",
        success: function (data) {
            $('div.mymodal').addClass('active')
            $('div.mymodal').html(data);
        },
        error: function (xhr, status, error) {
            console.log(xhr.responseText);
        }
    });
}

function LookUp(event) {
    var wherequery = ''
    var isfromFnu = $(event).closest('.fnu').length > 0 ? true : false;
    for (let x of $('div.sv-search').find('input')) {
        if (x.value != '') {
            if (wherequery != "")
                wherequery += " and ";
            wherequery = wherequery + x.name + " like '%" + x.value + "%'";
        }
    }
    var params = {}
    params.WhereQuery = wherequery;
    var insertArea ='.sv-result'
    if (isfromFnu) {
        
        params.isFindandUse = true
    }
    var Url = resolveUrl('Home/GridSearch')
    $.ajax({
        url: Url,
        type: "POST",
        data: params,
        dataType: 'html',
        success: function (res) {
            if (isfromFnu) {
                
                $(event).closest('.fnu').find('.sv-result').html(res);
                $('.sv').addClass('fnu')
            }
            else {
                htmlnew = res
                $(event).closest('.sv').find('.sv-result').html(htmlnew);
            }
                
            //console.log(res)
        },
        error: function (request, status, errorThrown) {
            console.log('fail');
        }
    });
}


function SelectLookupData(target) {
    var row = {};
    var chosen = $(target).closest('.sv').find('.sv-result tr').find('[name="myradio"]:checked');
    var list = $(chosen).closest('tr').children()
    var header = $(target).closest('.sv').find('.sv-result th');

    for (let index = 1; index < list.length; index++) {
        
        row[header[index].textContent.replace(' ', '')] = list[index].textContent
    }
    return row;
}

function openFindandUse(entity) {
    var params = { LayoutName: entity, isFindandUse: true }
    var urlrs = resolveUrl("Home/GenericSVBuilder")
    $.ajax({
        url: urlrs,
        type: "GET",
        data: params,
        dataType: 'html',
        success: function (html) {
            $('div.mymodal').find('.popup').addClass('overlay');
            var htmlnew = `<div class="popup fnupopup" data-layoutName="${entity}">
            <div class="popup-content">
            <div class="popup-header">
            <button style="float:right;" onclick="closeFnU()">X</button>
            <div style="color:darkslateblue;padding:20px;">Lookup</div>
            </div>
                  ${html}    
             </div></div>`

            if ($('div.mymodal').hasClass('active')) {
                htmlnew = `<div class="fnupopupoverlay">
                <div class="fnupopupcontent">
                    ${htmlnew}
                 </div></div>
                `
            }
            $('div.mymodal').addClass('active')
            $('div.mymodal').append(htmlnew);
            $('.sv').addClass('fnu')
        }
    });

}

function closeFnU() {
    
    if ($('div.mymodal').find('.popup').hasClass('overlay')) {
        $('div.mymodal').find('.fnupopupoverlay').remove();
        $('div.mymodal').find('.popup').removeClass('overlay');
    }
    else {
        $('div.mymodal').removeClass('active')
        $('div.mymodal').html('');
    }
}

function WizardNext(pageName, presentPage) {

    var RuleCode = $('.wizard .section').data('section-obj').RuleCode

    if (window[RuleCode] != null) {
        if (ExecRules(window[RuleCode](), $('.wizard .form'))) {
            return
        }
    }
    params = {
        policy: Object.fromEntries(new FormData(document.forms[0]).entries()),
        WizardGuid: { ActiveGuid: $('.wizard').data('obj') },
        request: $('.wizard .section').data('section-obj')
    }
    var url = resolveUrl($('form').attr('action'));
    $.ajax({
        url: url,
        type: "POST",
        data: params,
        /*dataType: 'html',*/
        success: function (data) {
            var mydata = data;
            if (isJson(data)) {
                WizardSuccess();
            }
            else if (typeof mydata == 'object') {
                if (mydata.Rules.length > 0) {
                    DisplayWizardRules(mydata.Rules)
                }
            }
            else {
                $('.wizard').html(data);
            }
        }
    });

}

function WizardSuccess() {
    SaveSuccessMsg();
    params = { PageName: "WelcomePage" }
    $.ajax({
        url: 'UDBuilder',
        type: "GET",
        data: params,
        dataType: 'html',
        success: function (data) {
            $('.body-content').html(data);
        }
    });
}
function WizardPrev() {

    params = {
        WizardGuid: { ActiveGuid: $('.wizard').data('obj') }
    }
    var url = resolveUrl('Home/WizardPrevious')
    $.ajax({
        url: url,
        type: "POST",
        data: params,
        dataType: 'html',
        success: function (data) {
            $('.wizard').html(data);
        }
    });

}




function DisplayRules(ruleList) {
    var msg = '<ul>'
    for (let index = 0; index < ruleList.length; index++) {
        msg = msg + '<li>' + ruleList[index] + '</li>'
    }
    msg = msg + '</ul>'
    $('.RuleContent').remove()
    var script = '<script>' + `$('.myrulesholder').on('click', function () {
            $('.RuleContent').toggle()
            $('.myrulesholder.slide').toggle()
        })`+ '<\/script>'
    msg = `<div class="RuleContent">${msg}<button class='myrulesholder'>>></button>
        ${script}</div><button class='myrulesholder slide'><<</button>`
    $('div.mymodal').addClass('active');
    $('div.mymodal').append(msg);
}

function DisplayWizardRules(ruleList) {
    var msg = '<ul>'
    for (let index = 0; index < ruleList.length; index++) {
        msg = msg + '<li>' + ruleList[index] + '</li>'
    }
    msg = msg + '</ul>'
    $('.RuleContent').remove()
    var script = '<script>' + `$('.myrulesholder').on('click', function () {
            $('.RuleContent').toggle()
            $('.myrulesholder.slide').toggle()
        })`+ '<\/script>'
    msg = `<div class="RuleContent">${msg}<button type="button" class='myrulesholder'>>></button>
        ${script}</div><button type="button" class='myrulesholder slide'><<</button>`
    $('.section').append(msg)
}
function gridRecordsChange(targt) {
    var recordsLt = targt.value
    var url = resolveUrl('Home/GenericGridRefresh')
    var params = {
        "GridRecordLength": recordsLt, "LayoutName": $(targt).closest('.section').data('section-obj').LayoutName,
        "SetNumber": 1,
        "isFindandUse": $(targt).closest('.section').data('section-obj').isFindandUse
    }
    $.ajax({
        url: url,
        type: "Get",
        data: params,
        dataType: 'html',
        success: function (data) {
            $(targt).closest('.grid').find('.replaceable-grid').html($(data).find('.replaceable-grid').html())
        }
    });
}

function GridRefresh(targt) {
    var url = resolveUrl('Home/GenericGridRefresh')
    var params = {
        "LayoutName": $(targt).closest('.section').data('section-obj').LayoutName
    }
    $.ajax({
        url: url,
        type: "Get",
        data: params,
        dataType: 'html',
        success: function (data) {
            $(targt).find('.grid').find('.replaceable-grid').html($(data).find('.replaceable-grid').html())
        }
    });
}

function SectionGridRefresh(targt) {
    var url = resolveUrl('Home/GenericGridRefresh')
    var params = {
        "LayoutName": $(targt).closest('.sectionPage-content').find('.section').data('section-obj').LayoutName
    }
    $.ajax({
        url: url,
        type: "Get",
        data: params,
        dataType: 'html',
        success: function (data) {
            $(targt).closest('.sectionPage-content').find('.grid').find('.replaceable-grid').html($(data).find('.replaceable-grid').html())
        }
    });
}

function gridPageNext(val, trgt) {
    var recordsLt = $(trgt).closest('.grid-paging').find('#gridrecordsload').val()
    var setnum = parseInt($(trgt).closest('.grid-paging').find('#grid-setnum').val()) + val
    if (setnum > parseInt($(trgt).closest('.grid-paging').find('#pgcount').text()) || setnum <= 0) {
        return
    }

    var url = resolveUrl('Home/GenericGridRefresh')
    var params = { "GridRecordLength": recordsLt, "LayoutName": $(trgt).closest('.section').data('section-obj').LayoutName, SetNumber: setnum }
    $.ajax({
        url: url,
        type: "Get",
        data: params,
        dataType: 'html',
        success: function (data) {
            $(trgt).closest('.grid').find('.replaceable-grid').html($(data).find('.replaceable-grid').html())
        }
    });
}
function gridPageChange(val, trgt) {
    var recordsLt = $(trgt).closest('.grid-paging').find('#gridrecordsload').val()
    var setnum = parseInt($(trgt).closest('.grid-paging').find('#grid-setnum').val()) + val
    if (setnum > parseInt($(trgt).closest('.grid-paging').find('#pgcount').text())) {
        //$(trgt).closest('.grid-paging').find('#grid-setnum').val($(trgt).closest('.grid-paging').find('#pgcount').text())
        setnum = $(trgt).closest('.grid-paging').find('#pgcount').text()
    }
    if (setnum <= 0) {
        //$(trgt).closest('.grid-paging').find('#grid-setnum').val(1)
        setnum=1
    }
    var url = resolveUrl('Home/GenericGridRefresh')
    var params = { "GridRecordLength": recordsLt, "LayoutName": $(trgt).closest('.section').data('section-obj').LayoutName, SetNumber: setnum }
    $.ajax({
        url: url,
        type: "Get",
        data: params,
        dataType: 'html',
        success: function (data) {
            $(trgt).closest('.grid').find('.replaceable-grid').html($(data).find('.replaceable-grid').html())
        }
    });
}


function genericSave(target, RuleCode) {
    var form = $(target).closest('form')
    var items = Object.fromEntries(new FormData(form[0]).entries())//form.serialize()//
    var requestData = $(target).closest('.section').data('section-obj')
    requestData["RuleCode"] = RuleCode
    params = {
        formCollection: JSON.stringify(items),
        request: requestData
    }
    if (window[RuleCode] != null) {
        if (ExecRules(window[RuleCode](), form)) {
            return
        }
    }
    var url = resolveUrl('Home/GenericSaveUDFields')
    SavePrehooks()
    $.ajax({
        url: url,
        type: "POST",
        data: params,
        dataType: 'json',
        success: function (data) {
            SavePosthooks(data,params.request)
        }
    });
}

function SaveSuccessMsg() {
    var html = '<div class="success-show">Saved Successfully</div>'
    $(document.body).append(html)
    $('.success-show').fadeOut(2500)
}
function CustomSuccessMsg(msg) {
    var html = '<div class="success-show">'+msg+'</div>'
    $(document.body).append(html)
    $('.success-show').fadeOut(3500)
}
function ExecRules(rules, form) {
    var rulesList = []
    for (let index = 0; index < rules.length; index++) {
        if ($(form).find('[name="' + rules[index] + '"]').val() == '') {
            rulesList.push($(form).find('[name="' + rules[index] + '"]').closest('.field-ele').find('label').text() + ' is required')
        }
        else if ($(form).find('[name="' + rules[index] + '"]')[0].tagName == 'SELECT' && $(form).find('[name="' + rules[index] + '"]').val() == '0') {
            rulesList.push($(form).find('[name="' + rules[index] + '"]').closest('.field-ele').find('label').text() + ' is required')
        }
        else if (($(form).find('[name="' + rules[index] + '"]')[0].type == 'radio' || $(form).find('[name="' + rules[index] + '"]')[0].type == 'checkbox')
            && $(form).find('[name="' + rules[index] + '"]:checked').length==0) {
            rulesList.push($(form).find('[name="' + rules[index] + '"]').closest('.field-ele').find('label').text() + ' is required')
        }
    }
    if (rulesList.length > 0) {
        if ($('.wizard').length==0)
            DisplayRules(rulesList)
        else
            DisplayWizardRules(rulesList)
        return true;
    }
    else {
        $('.RuleContent').html('')
        $('.RuleContent').remove()
        return false;
    }
}


function SelectRowDataByHyperlink(target) {
    var row = {};
    var list = $(target).closest('tr').children()
    var header = $(target).closest('tbody').find('th');

    for (let index = 1; index < list.length; index++) {

        row[header[index].textContent.replace(' ', '')] = list[index].textContent
    }
    return row;
}

function getRowRadioCheckedBySection(target) {
    var row = {};
    var chosen = $(target).find('tr').find('[name="myradio"]:checked');
    var list = $(chosen).closest('tr').children()
    var header = $(target).find('tbody').find('th');

    for (let index = 1; index < list.length; index++) {

        row[header[index].textContent.replace(' ','')] = list[index].textContent
    }
    return row;
}


function showDialogyesorNo(msg,functionYes,functionNo) {
    isDirty = true;
    if (isDirty == true) {
        handlers = []
        handlers.push({ text: 'Yes', action: functionYes })
        handlers.push({ text: 'No', action: functionNo })

        DialogBox('info',msg, handlers);
    }
}

function CloseDialogBox(isfrompopup) {
    if (isfrompopup) {
        $('.dialog-box').css('display', 'none')
    }
    else {
        $('div.mymodal').removeClass('active')
        $('div.mymodal').html('')
    }
}

function BuildFilterObject(whereQuery) {
    var FilterQry = []
    var finalSplit = whereQuery.split(/and|or/)
    var i = -1;
    for (let name of finalSplit) {
        key = name.split(/=|>=|<=|>|<|!=/)[0].trim()
        value = name.split(/=|>=|<=|>|<|!=/)[1].trim()
        operator = name.match(/=|>=|<=|>|<|!=/)[0]
        FilterQry.push({ key: key, Value: value, Operator: opertor[operator] })
        i++;
    }
    return JSON.stringify({ Filter: FilterQry })
}

function isJson(str) {
    try {
        JSON.parse(str);
    } catch (e) {
        return false;
    }
    return true;
}