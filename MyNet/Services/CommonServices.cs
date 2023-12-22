using Microsoft.Ajax.Utilities;
using MyNet.ExecuteProcess;
using MyNet.Helpers;
using MyNet.Models;
using MyNetData.Data;
using MyNetData.IData;
using MyNetModels.Models;
using MyNetServices.IServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using WebGrease.Css.Ast;

namespace MyNetServices.Services
{
    public class CommonServices:ICommonServices
    {
        ICommonData IcommonData = null;
        public  CommonServices()
        {
            IcommonData = new CommonData();
        }
        public string LoginService(LoginModel loginModel)
        {
            string username = null;
            var dt=IcommonData.LoginData(loginModel);
            if (dt != null)
            {
                if(dt.Rows.Count==1)
                {
                   username=Convert.ToString( dt.Rows[0]["username"]) ;
                    if (!string.IsNullOrEmpty(username)){
                        return username;
                    }
                }
            }
            return null;
        }
        private string GetXml<T>(T objecttoxml)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(streamWriter, objecttoxml);
                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
        }
        public string Pagexml(string pageName)
        {
            return IcommonData.GetPageXml(pageName);
        }
        
        public GenericModel UDService(Section section,GenericModel genericdata)
        {

            string xmlstring;
            if(genericdata == null)
            {
                genericdata = new GenericModel();
            }

            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(Section));
                    xmlSerializer.Serialize(streamWriter, section);
                    xmlstring = Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
            var data= IcommonData.GetLayout(xmlstring);

            LayoutModel layout = new LayoutModel();
            var layoutdata=data.Tables[0].Rows;
            for (int i = 0; i < layoutdata.Count; i++)
            {
                layout.entity = layoutdata[i]["entity"].ToString();
                layout.layouttype = layoutdata[i]["layouttype"].ToString();
                layout.selectquery = Convert.ToString(layoutdata[i]["selectquery"]);
                layout.selectsp = Convert.ToString(layoutdata[i]["selectsp"]);
                layout.Editcolumn = Convert.ToString(layoutdata[i]["Editcolumn"]);
                layout.JSFunction = Convert.ToString(layoutdata[i]["JSFunction"]);
                layout.selectionmode = Convert.ToString(layoutdata[i]["selectionmode"]);
                layout.OrderBy = Convert.ToString(layoutdata[i]["OrderClause"]);


            }
            List<ElementModel> udfieldslist = new List<ElementModel>();
            genericdata.UDFieldProperty = new List<UDFieldProperty>();
            var udfieldsdata = data.Tables[1].Rows;
            for (int i = 0; i < udfieldsdata.Count; i++)
            {
                ElementModel ele = new ElementModel();

                ele.id = Convert.ToString(udfieldsdata[i]["id"]);
                ele.label = udfieldsdata[i]["label"].ToString();
                ele.type = udfieldsdata[i]["type"].ToString();
                ele.entityname = Convert.ToString(udfieldsdata[i]["entityname"]);
                ele.sortorder = Convert.ToInt32(udfieldsdata[i]["sortorder"]);
                ele.defaultvalue = Convert.ToString(udfieldsdata[i]["defaultvalue"]);
                ele.selectquery = Convert.ToString(udfieldsdata[i]["selectquery"]);
                ele.FnuLayoutName = Convert.ToString(udfieldsdata[i]["fnulayout"]);
                ele.IsHidden= Convert.ToBoolean(udfieldsdata[i]["Ishidden"]);
                ele.IsReadonly = Convert.ToBoolean(udfieldsdata[i]["readonly"]);
                ele.isSearchfield= Convert.ToBoolean(string.IsNullOrEmpty(Convert.ToString(udfieldsdata[i]["isSearchField"])) ?1: udfieldsdata[i]["isSearchField"]);
                if (!string.IsNullOrEmpty(Convert.ToString(udfieldsdata[i]["propertyname"])) && !string.IsNullOrEmpty(Convert.ToString(udfieldsdata[i]["propertyvalue"])))
                {
                    genericdata.UDFieldProperty.Add(new UDFieldProperty()
                    {
                        eleid = Convert.ToString(udfieldsdata[i]["id"]),
                        EntityName = Convert.ToString(udfieldsdata[i]["entityname"]),
                        PropertyName = Convert.ToString(udfieldsdata[i]["propertyname"]),
                        PropertyValue = Convert.ToString(udfieldsdata[i]["propertyvalue"])
                    });
                }
                udfieldslist.Add(ele);  
            }
            var finaludfieldslist = udfieldslist.GroupBy(x => x.id).Select(x => x.First()).ToList();


            genericdata.Elements = finaludfieldslist;
            genericdata.layouts = layout;

            return genericdata;
        }

        public GenericModel SetDefaultValues(GenericModel genericModel)
        {
            string xmlstring;

            GenericRequest request = new GenericRequest();
            if (!string.IsNullOrEmpty(genericModel.WhereQuery))
            {
                request.WhereQuery = genericModel.WhereQuery.ToLower().Replace("where", "");
            }
            if (!string.IsNullOrEmpty(genericModel.AdditionalParams))
            {
                var additionalParams = JsonConvert.DeserializeObject(genericModel.AdditionalParams);
                var b = ((JObject)additionalParams);
                   
                request.AdditionalParams   = JObject.FromObject(b).ToObject<Dictionary<string, string>>().Select(x=>new Dict() { Key=x.Key,Value=x.Value}).ToList();
            }
            if (!string.IsNullOrEmpty(genericModel.ActiveGuid))
            {
                if (request.AdditionalParams == null)
                {
                    request.AdditionalParams = new List<Dict>();
                }
                request.AdditionalParams.Add(new Dict() { Key="ActiveGuid",Value=genericModel.ActiveGuid});
            }
            if(string.IsNullOrEmpty(request.WhereQuery)  && request.AdditionalParams!=null && request.AdditionalParams.Count > 0)
            {
                request.WhereQuery = "where ";
                request.AdditionalParams.ForEach(x => {
                    if (request.WhereQuery!= "where ")
                    {
                        request.WhereQuery += " and ";
                    }
                    
                    request.WhereQuery+=x.Key+" = '"+x.Value+"'";
                });
                
            }
            //var instantiatedObject = Activator.CreateInstance("MyNetModels", genericModel.layouts.entity).Unwrap();  // to get object from string
            ////Assembly assembly = Assembly.LoadFrom("C:\\Users\\dunukapa\\source\\repos\\MyNet\\MyNetModels\\bin\\Debug\\MyNetModels.dll");

            //var typeofEntity= Type.GetType(genericModel.layouts.entity+ ",MyNetModels");        // to get type from string
            //foreach (var item in instantiatedObject.GetType().GetProperties())
            //{
            //    if (item.Name == genericModel.AdditionalParams.Split('=')[0])
            //    {
            //        item.SetValue(instantiatedObject, genericModel.AdditionalParams.Split('=')[1]);  
            //    }
            //    else
            //    {
            //        item.SetValue(instantiatedObject, string.Empty);
            //    }
            //}
            //genericModel.ModelEntity = instantiatedObject;

            DataTable fielddata =new DataTable();
            if (!string.IsNullOrEmpty(genericModel.layouts.selectsp))
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (TextWriter streamWriter = new StreamWriter(memoryStream))
                    {
                        var xmlSerializer = new XmlSerializer(typeof(GenericRequest));    
                        xmlSerializer.Serialize(streamWriter, request);
                        xmlstring = Encoding.UTF8.GetString(memoryStream.ToArray());
                    }
                }
                fielddata = IcommonData.GetDatabySP(genericModel.layouts.selectsp,xmlstring);
            }
            else if(!string.IsNullOrEmpty(genericModel.layouts.selectquery))
            {
                if(request.WhereQuery!=null)
                {
                    fielddata = IcommonData.GetDatabyselectquery(genericModel.layouts.selectquery + " Where " + request.WhereQuery);
                }
                else
                {
                    fielddata = IcommonData.GetDatabyselectquery(genericModel.layouts.selectquery);
                }
            }

            for (int i = 0; i < fielddata.Rows.Count; i++)
            {
                foreach (var x in genericModel.Elements)
                {
                    if (fielddata.Columns.Contains(x.entityname.ToLower()))
                    x.value = Convert.ToString(fielddata.Rows[i][x.entityname.ToLower()]);
                }

                
            }

            foreach (var x in genericModel.Elements)
            {
                x.value = x.value ?? x.defaultvalue;
                switch (x.type)
                    {
                        case "dropdown":
                            List<SelectListItem> selectListItems = new List<SelectListItem>();
                            var values = IcommonData.GetDatabyselectquery(x.selectquery);
                            for (int k = 0; k < values.Rows.Count; k++)
                            {
                                SelectListItem sli = new SelectListItem()
                                {
                                    Value = Convert.ToString(values.Rows[k]["KEY"]),
                                    Text = Convert.ToString(values.Rows[k]["VALUE"])
                                };
                                if (sli.Value == x.value)
                                    sli.Selected = true;
                                selectListItems.Add(sli);
                            }
                            x.dropdownval = selectListItems;
                            break;
                        case "radiobutton":
                            var radiovalues = IcommonData.GetDatabyselectquery(x.selectquery);
                        for (int k = 0; k < radiovalues.Rows.Count; k++)
                        {
                            x.Collection.Add(Convert.ToString(radiovalues.Rows[k]["KEY"]),Convert.ToString(radiovalues.Rows[k]["VALUE"]));
                        }
                        break;
                        case "checkbox":
                             var checkboxvalues = IcommonData.GetDatabyselectquery(x.selectquery);
                        for (int k = 0; k < checkboxvalues.Rows.Count; k++)
                        {
                            x.Collection.Add(Convert.ToString(checkboxvalues.Rows[k]["KEY"]), Convert.ToString(checkboxvalues.Rows[k]["VALUE"]));
                        }
                        break;
                    }

                }
            
            return genericModel;
        }

        public GenericModel SetGridValues(GenericModel genericModel)
        {
            DataTable fielddata = new DataTable();
            string xmlstring = "";
            if (genericModel.request == null)
            {
                genericModel.request = new GenericRequest();
            }

            if (!string.IsNullOrEmpty(genericModel.layouts.selectsp))
            {
                xmlstring = GetXml<GenericRequest>(genericModel.request);
                fielddata = IcommonData.GetDatabySP(genericModel.layouts.selectsp, xmlstring);
            }
            else if (!string.IsNullOrEmpty(genericModel.layouts.selectquery))
            {
                genericModel.request.Query = genericModel.layouts.selectquery;
                if ( !string.IsNullOrEmpty(genericModel.WhereQuery ) && string.IsNullOrEmpty(genericModel.request.WhereQuery))
                {
                    genericModel.request.WhereQuery = genericModel.WhereQuery;
                }
                var xml = GetXml<GenericRequest>(genericModel.request);
               fielddata = IcommonData.GetGenericDatabyselectquery(xml);
            }
            if(genericModel.Elements.Count > 0)
            {

                var nonSearchfields = genericModel.Elements.Where(x => x.isSearchfield == false);
                List<string> strings = new List<string>();
                foreach (var item in fielddata.Columns)
                {
                    string colname = ((DataColumn)item).ColumnName;
                    if (nonSearchfields.Where(x => x.entityname == colname).Count() == 0)
                    {
                        strings.Add(colname);
                    }
                    strings.Remove("count_count");
                    strings.Remove("id");
                }
                strings.ForEach(x =>
                {
                    fielddata.Columns.Remove(x);
                });

                var result = fielddata.Select().Select(x => x.ItemArray.Select((a, i) => new GridModel { Header = fielddata.Columns[i].ColumnName, Value = Convert.ToString(a) }));
                genericModel.GridElements = result;
            }
            else
            {
                var result = fielddata.Select().Select(x => x.ItemArray.Select((a, i) => new GridModel{ Header = fielddata.Columns[i].ColumnName, Value = Convert.ToString(a) }));
                genericModel.GridElements = result;
                foreach (var item in fielddata.Columns)
                {
                    string colname = ((DataColumn)item).ColumnName;
                    genericModel.Elements.Add(new ElementModel()
                    {
                        entityname = colname.ToUpper(),
                        isSearchfield=false
                    });

                }

            }
            if (genericModel.request.LayoutName != null)
            {
                var gridFieldProperties=new List<UDFieldProperty>();
                DataTable dataGrid = IcommonData.GetGridProperties(genericModel.request);
                dataGrid.Select().ForEach(x => {
                    gridFieldProperties.Add(new UDFieldProperty()
                    {
                        PropertyName = x["propertyname"].ToString(),
                        PropertyValue = x["propertyvalue"].ToString(),
                        EntityName = x["gridfield"].ToString()
                    });
                    
                    });
                genericModel.GridFieldProperties = gridFieldProperties;     
            }

            return genericModel;
        }

        public CascadingModel getCascadingvalues(CascadingModel cm)
        {
            string xmlstring;
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(CascadingModel));
                    xmlSerializer.Serialize(streamWriter, cm);
                    xmlstring = Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
            var data=IcommonData.getCascadingvalues(xmlstring);
            var dataTable=data.Tables[0];
            var dataTable2 = data.Tables[1];
            List<SelectListItem> selectlist = new List<SelectListItem>();   
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                SelectListItem sli = new SelectListItem()
                {
                    Value = dataTable.Rows[i]["KEY"].ToString(),
                    Text = dataTable.Rows[i]["VALUE"].ToString()
                };
                selectlist.Add(sli);    
            }
            cm.targetElement=Convert.ToString(dataTable2.Rows[0]["targetElement"]);
            cm.targetValue = selectlist;
            return cm;
        }

        public RuleSetModel GetRules(string RuleCode)
        {
            DataSet data = IcommonData.GetRules(RuleCode);
            var dataTable = data.Tables[0];
            RuleSetModel rm = new RuleSetModel()
            {
                RuleString = dataTable.Rows[0]["jsonstr"].ToString(),
                RuleSet = dataTable.Rows[0]["ruleset"].ToString()
            };

            return rm;
        }

        public List<WizardModel> WizardPagexml(string pageName)
        {
            List<WizardModel> list = new List<WizardModel>();
            DataSet data = IcommonData.getWizardPages(pageName);
            var dataTable = data.Tables[0];
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                list.Add(new WizardModel()
                {
                    ProcessName = dataTable.Rows[i]["processname"].ToString(),
                    PageXml = dataTable.Rows[i]["xmlstr"].ToString(),
                    PageName = dataTable.Rows[i]["pagename"].ToString()
                });
            }
            return list;
        }
        public void GenericDelete(GenericRequest request)
        {
            string xml= GetXml<GenericRequest>(request);
            IcommonData.GenericDelete(xml);
        }
        public dynamic SaveGenericUDField(GenericRequest request,dynamic entity)
        {
            entity.AdditionalParams = request.AdditionalParams;
            entity.RuleCode= request.RuleCode;

            string xml = "";
            try
            {
                
                xml = Helper.GetXml(entity);
                entity=Helper.ExecuteRules(entity,xml);
                if(entity!=null && entity.Rules!=null && entity.Rules.Count > 0)
                {

                }
                else
                {  
                    ExecuteProcess executeProcess=new ExecuteProcess(IcommonData);
                    executeProcess.Execute(request,entity);
                    //IcommonData.SaveGenericUDField(xml);
                }
            }
            catch(Exception ex) 
            {

            }
            return entity;

        }
        public dynamic RunProcesses(dynamic entity)
        {
            GenericRequest request=new GenericRequest();
            request.ProcessName = "RunCalculationProcess";
            string xml = "";
            try
            {
                xml = Helper.GetXml(entity);

                ExecuteProcess executeProcess = new ExecuteProcess(IcommonData);
                executeProcess.Execute(request, entity);
            }
            catch (Exception ex)
            {

            }
            return entity;

        }

        //////////////////////////////////////////////***********      NON GENERIC           *****************////////////////////////////////////////////////////    

        public void SavePolicy(PolicyWizard policy, GenericRequest request)
        {
            request.ProcessName = "policywizardSave";
            string xml = Helper.GetXml(policy);
            policy.AdditionalParams = request.AdditionalParams;
            policy.RuleCode = request.RuleCode;
            var entity = Helper.ExecuteRules(policy, xml);
            if (entity != null && entity.Rules != null && entity.Rules.Count > 0)
            {

            }
            else
            {
                ExecuteProcess executeProcess = new ExecuteProcess(IcommonData);
                executeProcess.Execute(request, entity);
            }
            //IcommonData.SavePolicy(xmlstring);
        }

        public void SaveHospital(Hospital Hospital)
        {
            string xml=Helper.GetXml(Hospital);
            IcommonData.SaveGenericUDField(xml);
        }

        public SalesData Sales(Section section)
        {
            GenericRequest request=new GenericRequest() { ProcessName="SalesDATAProcess"};
            SalesData sales = new SalesData();
            ExecuteProcess executeProcess = new ExecuteProcess(IcommonData);
            DataSet dt= executeProcess.Execute(request, section);
            var datatbl = dt.Tables[0];
            for (int i = 0; i < datatbl.Columns.Count; i++)
            {
                sales.GetType().GetProperty(Convert.ToString(datatbl.Columns[i])).SetValue(sales,Convert.ToString(datatbl.Rows[0][datatbl.Columns[i].ToString()]));
            }
            return sales;
        }
    }

}
