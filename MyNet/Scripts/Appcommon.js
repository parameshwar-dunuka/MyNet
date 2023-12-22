function SavePosthooks(data, request) {
    if (data != undefined && data.Rules != undefined && data.Rules.length>0) {
        DisplayRules(data.Rules)
        return
    } 
    SaveSuccessMsg();

    switch (request.LayoutName) {
        case "Hospital":
            var trgt = $('#section_HospitalGrid')
            GridRefresh(trgt)
            closePopup()
        case "Agent":
            var trgt = $('#section_AgentsGrid')
            GridRefresh(trgt)
            closePopup()
        case "Customer":
            var trgt = $('#section_CustomersGrid')
            GridRefresh(trgt)
            closePopup()
        default:
            closePopup()
    }

}
function SavePrehooks() {

}

function SelectLookup(target) {
    var data = SelectLookupData(target);
    var layoutName = $(target).closest('.fnupopup').attr('data-layoutName')

    switch (layoutName) {
        case 'Policy SV':
            $('[name="Policy"]').val(data.POLICYNUMBER)
            $('[name="PolicyName"]').val(data.POLICYNAME)
            closeFnU();
            break;
        case 'PolicyData SV':
            $('[name="Policy"]').val(data.POLICYIDENTIFIER)
            closeFnU();
            break;
        default:
    }
}