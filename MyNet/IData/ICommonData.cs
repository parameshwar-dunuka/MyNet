using MyNet.Models;
using MyNetModels.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetData.IData
{
    public interface ICommonData
    {
        DataTable LoginData(LoginModel loginModel);
        string GetPageXml(string pageName);
        DataSet GetLayout(string xmlstring);

        DataTable GetDatabyselectquery(string query);
        void SavePolicy(Policy policy);

        DataTable GetDatabySP(string spName, string xmlstring);
        DataSet getCascadingvalues(string xmlstring);
        DataSet getWizardPages(string pageName);
        DataSet GetRules(string ruleCode);
        DataTable GetGenericDatabyselectquery(string query);
        void GenericDelete(string xml);
        void SaveGenericUDField(string xml);
        DataTable GetProcessData(string xml);
        DataSet ExecuteSp(string spName, string xmlstring);
        DataTable GetGridProperties(GenericRequest request);
    }
}
