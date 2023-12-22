using MyNet.Models;
using MyNetData.IData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using WebGrease.Css.Ast;
using MyNetModels.Models;
using MyNet.Helpers;

namespace MyNetData.Data
{
    public class CommonData : BaseClass,ICommonData
    {
       
        public DataTable LoginData(LoginModel loginModel)
        {
            DataTable data = new DataTable();
            try
            {
                BaseClass baseClass = new BaseClass();
                string query=string.Format("select username from dbo.login_details where email='{0}' and password='{1}' ", loginModel.Email,loginModel.Password);
                data=baseClass.ExecuteReader(query);
                if (data.Rows.Count !=1)
                {
                    return null;

                }
            }
            catch (Exception e)
            {
                
            }
            return data;
        }

        public string GetPageXml(string pageName)
        {
            string val=null;
            try
            {
                BaseClass baseClass = new BaseClass();
                SqlCommand sqlCommand = baseClass.GetProcedure("PageXMLProcedure");
                sqlCommand.Parameters.AddWithValue("@PageName", pageName);

                SqlParameter sqlParameter = new SqlParameter("@PageXml", SqlDbType.NVarChar,int.MaxValue);
                sqlParameter.Direction = ParameterDirection.Output;
                sqlCommand.Parameters.Add(sqlParameter);
                baseClass.ExecuteProcedure(sqlCommand);
                val = sqlParameter.Value.ToString();

            }
            catch (Exception e)
            {

            }
            return val;
        }

        public DataSet GetLayout(string xmlstring)
        {
            BaseClass baseClass = new BaseClass();
            SqlCommand sqlCommand = baseClass.GetProcedure("GetPageLayout");
            sqlCommand.Parameters.AddWithValue("@xmlstring", xmlstring);


            DataSet val =ExecuteProcedureReader(sqlCommand);
            return val;
        }

        public DataTable GetDatabyselectquery(string query)
        {
            BaseClass baseClass = new BaseClass();
            var dt=baseClass.ExecuteReader(query);
            return dt;

        }
        public DataTable GetGenericDatabyselectquery(string xmlstring)
        {
            BaseClass baseClass = new BaseClass();
            SqlCommand sqlCommand = baseClass.GetProcedure("picg_GenericSpQuery");
            if (!string.IsNullOrEmpty(xmlstring))
            {
                sqlCommand.Parameters.AddWithValue("@xmlstring", xmlstring);
            }

            DataSet val = ExecuteProcedureReader(sqlCommand);
            return val.Tables[0];

        }
        public DataTable GetGridProperties(GenericRequest request)
        {
            string xmlstring=Helper.GetXml(request);
            BaseClass baseClass = new BaseClass();
            SqlCommand sqlCommand = baseClass.GetProcedure("picg_GridProperties");
            if (!string.IsNullOrEmpty(xmlstring))
            {
                sqlCommand.Parameters.AddWithValue("@xmlstring", xmlstring);
            }

            DataSet val = ExecuteProcedureReader(sqlCommand);
            return val.Tables[0];

        }

        public DataTable GetProcessData(string xmlstring)
        {
            BaseClass baseClass = new BaseClass();
            SqlCommand sqlCommand = baseClass.GetProcedure("picg_process");
            if (!string.IsNullOrEmpty(xmlstring))
            {
                sqlCommand.Parameters.AddWithValue("@processname", xmlstring);
            }

            DataSet val = ExecuteProcedureReader(sqlCommand);
            return val.Tables[0];

        }

        public DataTable GetDatabySP(string spName,string xmlstring)
        {
            BaseClass baseClass = new BaseClass();
            SqlCommand sqlCommand = baseClass.GetProcedure(spName);
            if(!string.IsNullOrEmpty(xmlstring))
            {
                sqlCommand.Parameters.AddWithValue("@xmlstring", xmlstring);
            }

            DataSet val = ExecuteProcedureReader(sqlCommand);
            return val.Tables[0];
        }

        public DataSet ExecuteSp(string spName, string xmlstring)
        {
            BaseClass baseClass = new BaseClass();
            SqlCommand sqlCommand = baseClass.GetProcedure(spName);
            if (!string.IsNullOrEmpty(xmlstring))
            {
                sqlCommand.Parameters.AddWithValue("@xmlstring", xmlstring);
            }

            DataSet val = ExecuteProcedureReader(sqlCommand);
            return val;
        }

        public DataSet getCascadingvalues(string xmlstring)
        {
            BaseClass baseClass=new BaseClass();
            SqlCommand sqlCommand = baseClass.GetProcedure("Cascadingprocedure");
            sqlCommand.Parameters.AddWithValue("@xmlString", xmlstring);

            DataSet val = ExecuteProcedureReader(sqlCommand);

            return val;
        }

        public DataSet GetRules(string RuleCode)
        {
            BaseClass baseClass = new BaseClass();
            SqlCommand sqlCommand = baseClass.GetProcedure("picg_rules");
            sqlCommand.Parameters.AddWithValue("@RuleCode", RuleCode);

            DataSet val = ExecuteProcedureReader(sqlCommand);

            return val;
        }

        public DataSet getWizardPages(string pageName)
        {
            DataSet data = null;
            try
            {
                BaseClass baseClass = new BaseClass();
                SqlCommand sqlCommand = baseClass.GetProcedure("picg_WizardPage");
                sqlCommand.Parameters.AddWithValue("@pageName", pageName);

                data = baseClass.ExecuteProcedureReader(sqlCommand);

            }
            catch (Exception e)
            {

            }

            return data;
        }

        public void GenericDelete(string xml)
        {
            BaseClass baseClass = new BaseClass();
            SqlCommand sqlCommand = baseClass.GetProcedure("picd_gridrecord");
            sqlCommand.Parameters.AddWithValue("@xmlString", xml);
            baseClass.ExecuteProcedure(sqlCommand);
            
        }
        public void SaveGenericUDField(string xml)
        {
            BaseClass baseClass = new BaseClass();
            SqlCommand sqlCommand = baseClass.GetProcedure("pics_SaveGenericData");
            sqlCommand.Parameters.AddWithValue("@xmlString", xml);
            baseClass.ExecuteProcedure(sqlCommand);
        }

        public DataSet ExecuteRules(dynamic modelEntity, string xml,string RuleCode,string ExecSpRules)
        {
            BaseClass baseClass = new BaseClass();
            SqlCommand sqlCommand = baseClass.GetProcedure("pic_RuleEngine");
            sqlCommand.Parameters.AddWithValue("@pageXml", xml);
            sqlCommand.Parameters.AddWithValue("@CustomRulesxml", modelEntity);
            sqlCommand.Parameters.AddWithValue("@RuleCode", RuleCode);
            sqlCommand.Parameters.AddWithValue("@ExecSpRules", ExecSpRules);


            var data = baseClass.ExecuteProcedureReader(sqlCommand);
            return data;
        }

        //////////////////////////////////////////////***********      NON GENERIC           *****************////////////////////////////////////////////////////    

        public void SavePolicy(Policy policy)
        {
            string policyXml=Helper.GetXml(policy);
            try
            {
                BaseClass baseClass = new BaseClass();
                SqlCommand sqlCommand = baseClass.GetProcedure("pics_savepolicy");
                sqlCommand.Parameters.AddWithValue("@xmlString", policyXml);

                baseClass.ExecuteProcedure(sqlCommand);

            }
            catch (Exception e)
            {

            }
        }
        public void SavePolicyWizard(PolicyWizard policy)
        {
            string policyXml = Helper.GetXml(policy);
            try
            {
                BaseClass baseClass = new BaseClass();
                SqlCommand sqlCommand = baseClass.GetProcedure("pics_savepolicyWizard");
                sqlCommand.Parameters.AddWithValue("@xmlString", policyXml);
                baseClass.ExecuteProcedure(sqlCommand);

            }
            catch (Exception e)
            {

            }
        }


    }
}
