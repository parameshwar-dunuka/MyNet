using MyNet.Models;
using MyNetData.Data;
using MyNetServices.IServices;
using MyNetServices.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace MyNet.Helpers
{
    public static class Helpers
    {

        public static HtmlString RuleBuilder (this HtmlHelper html,string Rulecode)
        {
            //Rulecode = "AgentRules";
            if (string.IsNullOrEmpty(Rulecode)){
                return new HtmlString("");
            }
            string scriptTags = "";
            string MandatoryList = "";
            CommonServices commonServices = new CommonServices ();

            RuleSetModel ruleModel = commonServices.GetRules(Rulecode);
            var rules = JsonConvert.DeserializeObject<RuleModel>( ruleModel.RuleString);

            foreach (var item in rules.Mandatory)
            {
                MandatoryList = MandatoryList +"'"+ item + "',";
            }
            
            if (!string.IsNullOrEmpty(MandatoryList))
            {
                MandatoryList = "[" + MandatoryList.Trim(',') + "]";
            }
            else
            {
                return new HtmlString(scriptTags);
            }
            string RuleScript = $"function {Rulecode}()";
            RuleScript = RuleScript + $"{{ var RuleList={MandatoryList}; \n return RuleList;}}";
            scriptTags = $"<script>{RuleScript}</script>";

            return new HtmlString(scriptTags);
        }
        

        public static HtmlString UDPropertiesBuilder(this HtmlHelper html, List<UDFieldProperty> udproperties,string LayoutName)
        {
            Helper helper = new Helper ();
            string property = LayoutName + "_" + udproperties[0].EntityName;
            string script = "document.getElementById('" + property + "').onchange = function(e) {";
            foreach (var item in udproperties)
            {
                switch (item.PropertyName) 
                {
                    case "AdvancedSettings":
                        script+=helper.AdvancedSettings(item, LayoutName);
                        break;
                    case "Cascading":
                        script += " Cascading(e)";
                        break;
                    default:
                        break;
                }
                script += "\n";
            }
            string scriptTags = $"<script>{script+ $"}}\n$('#{property}').trigger('change')"}</script>";
            return new HtmlString(scriptTags);
        }

    }

    public class Helper
    {
        public static List<string> ConvertDataTableToList(DataTable table,string columnData)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                list.Add(table.Rows[i][columnData].ToString());
            }
            return list;
        }
        public string AdvancedSettings(UDFieldProperty uDField,string layoutName)
        {
            string property = layoutName+"_"+ uDField.EntityName;
            string script = "";
            var propVal = JsonConvert.DeserializeObject<List<UDFieldProperty>>(uDField.PropertyValue);
            foreach (var item in propVal)
            {
                var value = item.Value;

                script += $" if($('#{property}').val()=='{value}'){{";
                if (item.FieldsToHide?.Count > 0)
                {
                    script += $"$('#{layoutName+"_"}{string.Join(",#" + layoutName + "_", item.FieldsToHide)}').closest('.field-ele').hide()" +
                                $" \n";
                }
                if (item.FieldsToShow?.Count > 0)
                {
                    script += $"$('#{layoutName + "_"}{string.Join(",#" + layoutName + "_", item.FieldsToShow)}').closest('.field-ele').show()" +
                                $" \n";
                }
                script += "}\n";
            }
             
            return script;
        }
        public static T ExecuteRules<T>(T model,string xml)
        {
            string customExecSpRules =string.Empty;
            List<string>  Rules = new List<string>();
            dynamic modelEntity=model;
            var ruleCode=modelEntity.RuleCode;
            CommonServices commonServices = new CommonServices();
            if (string.IsNullOrEmpty(ruleCode))
            {
                return modelEntity;
            }
            RuleSetModel ruleModel = commonServices.GetRules(ruleCode);
            var rules = JsonConvert.DeserializeObject<RuleModel>(ruleModel.RuleString);
            if (rules.CustomRulesSql != null)
            {
                foreach (var item in rules.CustomRulesSql)
                {
                    var qry = item.SqlQuery;
                    foreach (var item1 in modelEntity.GetType().GetProperties())
                    {
                        if (Regex.IsMatch(qry, "{" + item1.Name + "}"))
                        {
                            var assemb = item1.GetValue(modelEntity);
                            item.SqlQuery = Regex.Replace(qry, "{" + item1.Name + "}", "'" + assemb + "'");
                            break;
                        }
                    }
                }
                var customEntityRules = Helper.GetXml(rules.CustomRulesSql);
                if (rules.ExecSpRules != null)
                {
                    customExecSpRules = Helper.GetXml(rules.ExecSpRules);

                }

                CommonData commonData = new CommonData();

                DataSet customrules = commonData.ExecuteRules(customEntityRules, xml, ruleCode, customExecSpRules);
                var custrules = customrules.Tables.Count == 0 ? null : customrules.Tables[0];
                if (custrules != null)
                {
                    for (int i = 0; i < custrules.Rows.Count; i++)
                    {
                        Rules.Add(Convert.ToString(custrules.Rows[i]["rulemsg"]));
                    }
                }
            }
                        
            foreach (var item in rules?.Mandatory)
            {
                if (string.IsNullOrEmpty(modelEntity.GetType().GetProperty(item)?.GetValue(modelEntity)))
                {
                    Rules.Add(item + "*");
                }
            }
            modelEntity.Rules= Rules;
            return modelEntity;
        }

        public static List<Dict> AdditionalParameters(string additonalParams)
        {
            var additionalParams = JsonConvert.DeserializeObject(additonalParams);
            var b = ((JObject)additionalParams);

            var data= JObject.FromObject(b).ToObject<Dictionary<string, string>>().Select(x => new Dict() { Key = x.Key, Value = x.Value }).ToList();
            return data;
        }
        public static dynamic setFormData(string entityName,string formData,List<Dict> AdditionalParams)
        {

            var instantiatedObject = Activator.CreateInstance("MyNetModels", entityName).Unwrap();
            try
            {
                var ModelEntity = JsonConvert.DeserializeObject(formData);
                var data = JObject.FromObject(ModelEntity).ToObject<Dictionary<string, dynamic>>().Select(x => new Dict() { Key = x.Key, Value = x.Value }).ToList();
                for (int i = 0; i < data.Count(); i++)
                {
                    if (instantiatedObject.GetType().GetProperty(data[i].Key) == null)
                    {
                        AdditionalParams.Add(data[i]);
                    }
                    else
                    {
                        if ((instantiatedObject.GetType().GetProperty(data[i].Key).PropertyType.Name == "Int32" ||
                            instantiatedObject.GetType().GetProperty(data[i].Key).PropertyType.Name == "Decimal")
                            && string.IsNullOrEmpty(data[i].Value))
                            data[i].Value = 0;
                        //instantiatedObject.GetType().GetProperty(data[i].Key).SetValue(instantiatedObject, data[i].Value);
                        instantiatedObject.GetType().GetProperty(data[i].Key).SetValue(instantiatedObject, Convert.ChangeType(data[i].Value, instantiatedObject.GetType().GetProperty(data[i].Key).PropertyType));

                    }
                }
            }
            catch(Exception ex) 
            { 

            }
            
            return instantiatedObject;
        }

        public static string GetXml(dynamic entity)
        {
            string xml = "";
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(entity.GetType());
                    x.Serialize(streamWriter, entity);
                    xml = Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
            return xml;
        }
    }

}