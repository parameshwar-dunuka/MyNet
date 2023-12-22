//deprecated function
function createPolicy() {
    var policyid = 0;
    var params = {
        policyid: policyid,
    }
    AjaxRequest('UDBuilder', 'Home', 'Policy', '', params, 'html', 'id=0', 'GET', function (data) {
        $('div.mymodal').addClass('active')
        $('div.mymodal').html(data)});
    //AjaxRequest('WizardStart', 'Home', 'Policy', '', params, 'html', 'id=0', "GET", function (data) { $('.body-content').html(data); });
}
function EditPolicy(trgt) {
    var rowdata = SelectRowDataByHyperlink(trgt);
    var params = {
        policyNum: rowdata.POLICYNUMBER
    }

    OpenPopup('Policy', '', params, 'id=0');
}

function AddAgent() {
    var AgentID = 0;
    var params = {
        AgentID: AgentID,
    }
    OpenPopup('Agent', '', params, 'id=0');
}

function EditAgent(trgt) {
    var rowdata = SelectRowDataByHyperlink(trgt);
    var AgentID = rowdata.ID;
    var params = {
        "AgentID": AgentID,
    }
    OpenPopup('Agent', '', params, 'id=0');
}

function DeleteAgent(param) {
    CloseDialogBox(false);
    if (param !== 'yes') {
        showDialogyesorNo("Do You want to Delete the record selected?", "DeleteAgent('yes')", "CloseDialogBox(false)");
        return
    }
    trgt = $('#section_AgentsGrid')
    var data = getRowRadioCheckedBySection(trgt);
    var params = {
        "AgentID": data.ID,
    }
    if (params.AgentID == undefined) {
        return
    }
    SectionAjaxRequest('GenericDelete', 'Home', 'AgentsGrid', '', params, 'application/json;', 'json', 'GET', function (data, trgt) { GridRefresh(trgt) });

}


function AddCustomer() {
    var CustomerId = 0;
    var params = {
        CustomerId: CustomerId,
    }
    OpenPopup('Customer', '', params, 'id=0');
}

function EditCustomer(trgt) {
    var rowdata = SelectRowDataByHyperlink(trgt);
    var CustomerId = rowdata.ID;
    var params = {
        "CustomerId": CustomerId,
    }
    OpenPopup('Customer', '', params, 'id=0');
}

function DeleteCustomer(param) {
    CloseDialogBox(false);
    if (param !== 'yes') {
        showDialogyesorNo("Do You want to Delete the record selected?", "DeleteCustomer('yes')", "CloseDialogBox(false)");
        return
    }
    trgt = $('#section_CustomersGrid')
    var data = getRowRadioCheckedBySection(trgt);
    var params = {
        "CustomerID": data.ID,
    }
    if (params.CustomerID == undefined) {
        return
    }
    SectionAjaxRequest('GenericDelete', 'Home', 'CustomersGrid', '', params, 'application/json;', 'json', 'GET', function (data, trgt) { GridRefresh(trgt) });

}

function AddHospital() {
    var HospitalID = 0;
    var params = {
        "HospitalID": HospitalID
    }
    OpenPopup('Hospital', '', params, 'id=0');
    
}

function EditHospital(trgt) {
    var rowdata = SelectRowDataByHyperlink(trgt);
    var HospitalID = rowdata.ID;
    var HospitalName = rowdata.HOSPITALNAME;
    var params = {
        "HospitalID": HospitalID,
        "HospitalName" :HospitalName
    }
    OpenPopup('Hospital', '', params, 'id=0');
}

function DeleteHospital(param) {
    CloseDialogBox(false);
    if (param !== 'yes') {
        showDialogyesorNo("Do You want to Delete the record selected?", "DeleteHospital('yes')", "CloseDialogBox(false)");
        return
    }
    trgt = $('#section_HospitalGrid')
    var data = getRowRadioCheckedBySection(trgt);
    var params = {
        "HospitalID": data.ID,
    }
    if (params.HospitalID == undefined) {
        return
    }
    SectionAjaxRequest('GenericDelete', 'Home', 'HospitalGrid', '', params, 'application/json;', 'json', 'GET', function (data, trgt) { GridRefresh(trgt) });

}

function GetData(pageName, Priority) {
    var params = {
        PageName: pageName,
        AdditionalParams: 'Priority=' + Priority
    }
    var url = resolveUrl('Home/UDBuilder');
    $.ajax({
        url: url,
        type: 'GET',
        data: params,
        dataType: 'html',
        success: function (data) {
            $('body').html(data)
        }
    });
}
function ShowCommissionData() {
    var params = {
        PageName: "OpenCommissions"
    }
    var url = resolveUrl('Home/UDBuilder');
    $.ajax({
        url: url,
        type: 'GET',
        data: params,
        dataType: 'html',
        success: function (data) {
            $('body').html(data)
        }
    });
}
function InititateWizardofPolicy() {
    params = {}
    AjaxRequest('WizardStart', 'Home', 'Policy', '', params, 'html', 'id=0', "GET", function (data) { $('.body-content').html(data); });
}

function EditPolicyData(trgt) {
    var rowdata = SelectRowDataByHyperlink(trgt);
    var PolicyDataID = rowdata.ID;
    var params = { PolicyDataID: PolicyDataID }
    AjaxRequest('WizardStart', 'Home', 'Policy', '', params, 'html', 'id=0', "GET", function (data) { $('.body-content').html(data); });
}
function AddTransaction() {
    var id = 0;
    var params = {
        "Id": id
    }
    OpenPopup('CreateTransaction', '', params, 'id=0');
}


function DeleteTransaction(param) {
    CloseDialogBox(false);
    if (param !== 'yes') {
        showDialogyesorNo("Do You want to Delete the record selected?", "DeleteTransaction('yes')", "CloseDialogBox(false)");
        return
    }
    trgt = $('#section_TransactionsGrid')
    var data = getRowRadioCheckedBySection(trgt);
    var params = {
        "TransactionID": data.ID,
    }
    if (params.TransactionID == undefined) {
        return
    }
    SectionAjaxRequest('GenericDelete', 'Home', 'TransactionsGrid', '', params, 'application/json;', 'json', 'GET', function (data, trgt) { GridRefresh(trgt) });

}

function RunNow() {
   
    showDialogyesorNo("Running Commission Cycle May take long time, Do you want to continue", "RunNowCycle('yes')", "CloseDialogBox(false)");
}

function RunNowCycle() {
    var params = {
    }
    CloseDialogBox(false);
    var url = resolveUrl('Home/RunCommission');
    $.ajax({
        url: url,
        type: 'GET',
        data: params,
        dataType: 'application/json',
        success: function (data) {
            CustomSuccessMsg("Commission Ran Successfully")
        }
    });
}