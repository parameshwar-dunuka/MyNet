using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyNet.Models
{
    public class CommonModels
    {
    }
    public class GenericRequest
    {
        public List<Dict> AdditionalParams { get; set; }
        public string WhereQuery { get; set; }
        public string RuleCode { get; set; }
        public int GridRecordLength { get; set; } = 10;
        public int SetNumber { get; set; }=1;
        public string LayoutName { get; set; }
        public string ModelEntity { get; set; }
        public string Query { get; set; }
        public bool isFindandUse { get; set; }
        public string ProcessName { get; set; }
        public string ActiveGuid { get; set; }
    }
    public class Section
    {
        public string SectionName { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string LayoutName { get; set; }
        public string Title { get; set; }
        public string AdditionalParams { get; set; }
        public string WhereQuery { get; set; }

        public bool isFindandUse { get; set; }
        public Properties Properties { get; set; }
        public string RuleCode { get; set; }
        public string ProcessName { get; set; }
        public List<SectionAction> SectionActions { get; set; }
        public string ModelEntity { get; set; }

    }
    public class GenericModel
    {

        public string HeaderText { get; set; }
        public string PageName { get; set; }

        public string AdditionalParams { get; set; }
        public string WhereQuery { get; set; }

        public dynamic ModelEntity { get; set; }
        public List<ElementModel> Elements { get; set; }

        public IEnumerable<IEnumerable<GridModel>> GridElements { get; set; }

        public LayoutModel  layouts { get; set; }
        public string SectionType { get; set; }
        public string GlobalEntity { get; set; }
        public string DataSource { get; set; }
        public string RuleCode { get; set; }
        public bool Editable { get; set; }
        public int DisplayRecordCount { get; set; }
        public bool Searchable { get; set; }
        public bool Sortable { get; set; }
        public GenericRequest request { get; set; }

        public bool IsWizard { get; set; } = false;
        public bool IsFromPopup { get; set; } = false;
        
        public bool ActiveWizard { get; set; }
        public string ActiveGuid { get; set; }

        public bool isFindandUse { get; set; }
        public List<WizardModel> wizardModel { get; set; }
        public List<UDFieldProperty> UDFieldProperty { get; set; }
        public List<UDFieldProperty> GridFieldProperties { get; set; }

    }
    public class ElementModel
    {
        public string id { get; set; }
        public int sortorder { get; set; }
        public string desc { get; set; }
        public string entityname { get; set; }
        public string type { get; set; }
        public string label { get; set; }
        public string defaultvalue { get; set; }
        public string value { get; set; }
        public dynamic dropdownval { get; set; }

        public bool isrequired { get; set; }

        public string selectquery { get; set; }
        public bool isSearchfield { get; set; }

        public string FnuLayoutName { get; set; }
        public bool IsHidden { get; set; }
        public bool IsReadonly { get; set; }

        //public List<string> Collection { get; set; } = new List<string>();
        public Dictionary<string, string> Collection { get; set; } = new Dictionary<string, string>();


    }

    public class GridModel
    {
        public string Header { get; set; }
        public string Value { get; set; }

    }

    public class GenericPage: GenericRequest
    {
        public string Header { get; set; }
        public string Type { get; set; }
        public string AdditionalParameters { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public List<Section> Body { get; set; }
        public bool IsWizard { get; set; } = false;

    }

    public class Properties
    {
        public string SelectionMode { get; set; }
    }

    

    public class SectionAction
    {
        public string Title { get; set; }

        public string Onclick { get; set; }
        public string Icon { get; set; }
    }

    public class LayoutModel
    {
        public string layouttype { get; set; }
        public string entity { get; set; }
        public string selectquery { get; set; }
        public string selectsp { get; set; }
        public string Editcolumn { get;set; }
        public string JSFunction { get; set; }
        public string selectionmode { get; set; }
        public string OrderBy { get; set; }

    }



    public class Dict
    {
        public string Key { get; set; }
        public dynamic Value { get; set; }

    }

    public class Root
    {
        public string RuleCode { get; set; }
        public string LayoutName { get; set; }
        public List<Dict> AdditionalParams { get; set; }
        public List<string> Rules { get; set; }
        public string ActiveGuid { get; set; }
        public string ProcessName { get; set; }


    }

    public class WizardModel
    {
        public string PageName { get; set; }
        public string PageXml { get; set; }
        public int isSkippable { get; set; }
        public int Sequence { get; set; }
        public string ProcessName { get; set; }
        public bool isInitial { get; set; }
        public bool isEnd { get; set; }
        public int InternalCount { get; set; }
        public bool isVisited { get; set; }
        public bool isSkipped { get; set; }
        public string ActiveGuid { get; set; }
        public bool isCurrent { get; set; }
    }

    public class CascadingModel
    {
        public object targetValue { get; set; }
        public string targetElement { get; set; }
        public string sourceValue { get; set; }
        public string sourceElement { get; set; }
        public string layoutname { get; set; }
    }

    

    public class RuleModel
    {
        public List<string> Mandatory { get; set; }
        public List<Rules> CustomRulesSql { get; set; }
        public List<string> ExecSpRules { get; set; }


    }

    public class Rules
    {
        public string Key { get; set; }
        public string RuleMessage { get; set; }
        public string SqlQuery { get; set; }

    }

    public class RuleSetModel
    {
        public string RuleString { get; set; }
        public string RuleSet { get; set; }
    }
    public class UDFieldProperty
    {
        public string eleid { get; set; }
        public string EntityName { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
        public string Value { get; set; }
        public List<string> FieldsToHide { get; set;}
        public List<string> FieldsReadonly { get; set; }
        public List<string> FieldsToShow { get; set; }


    }


    //////////////////////////////////////////////***********      NON GENERIC           *****************////////////////////////////////////////////////////    


    public class SalesData
    {
        public string Policyssold { get; set; }
        public string PremiumsReceived { get; set; }
        public string AgentsCommission { get; set; }
        public string GrossProfit { get; set; }
        public string TotalDisbursment { get; set; }
        public string Totalexpenditure { get; set; }
        public string NetProfit  { get; set; }
        public string customers { get; set; }
        public string Agents { get; set; }
        public string Hospitals { get; set; }

    }
}