using MyNet.Helpers;
using MyNet.Models;
using MyNetData.Data;
using MyNetData.IData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace MyNet.ExecuteProcess
{
    public class ExecuteProcess
    {
        ICommonData _commonData;
        public ExecuteProcess(ICommonData commonData)
        {
            _commonData = commonData;
        }
        public DataSet Execute(GenericRequest genericRequest,dynamic model)
        {
            DataSet dt=new DataSet();
            if (_commonData == null)
            {
                _commonData=new CommonData();
            }
            model.LayoutName=genericRequest.LayoutName;
            Dictionary<string,string> processDict= new Dictionary<string,string>();
            string processname=genericRequest.ProcessName;
            if (!string.IsNullOrEmpty(processname))
            {
                var processes = _commonData.GetProcessData(processname);

                for (int i = 0; i < processes.Rows.Count; i++)
                {
                    var processType = processes.Rows[i]["Executiontype"].ToString();
                    var processToExec = processes.Rows[i]["Execution"].ToString();
                    switch (processType.ToLower())
                    {
                        case "procedure":
                            dt=ExecuteProcedure(model, processToExec);
                            break;
                        case "method":
                            ExecuteMethod(model, processToExec);
                            break;
                        default:
                            break;
                    }
                }
            }
            return dt;
        }

        public DataSet ExecuteProcedure(dynamic model,string proc)
        {
            string xml=Helper.GetXml(model);
            var dataset=_commonData.ExecuteSp(proc, xml);
            return dataset;
        }

        public void ExecuteMethod(dynamic model, string methodName)
        {
            
            var instantiatedObject = Activator.CreateInstance("MyNet", methodName.Split('/')[0]).Unwrap();
            string xml = Helper.GetXml(model);
            methodName = methodName.Split('/')[1];
            if (instantiatedObject.GetType().GetMethod(methodName) != null)
            {
                MethodInfo method = instantiatedObject.GetType().GetMethod(methodName);
                method.Invoke(instantiatedObject, new[] { model });
            }

        }
    }
}