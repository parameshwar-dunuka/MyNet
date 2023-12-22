using MyNet.Models;
using MyNetModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetServices.IServices
{
    public interface ICommonServices
    {
        string LoginService(LoginModel loginModel);

        string Pagexml(string pageName);
        GenericModel UDService(Section section,GenericModel genericData);

        GenericModel SetDefaultValues(GenericModel GenericModel);
        GenericModel SetGridValues(GenericModel GenericModel);


        void SavePolicy(PolicyWizard policy, GenericRequest request);
        CascadingModel getCascadingvalues(CascadingModel cm);
        List<WizardModel> WizardPagexml(string pageName);

        RuleSetModel GetRules(string RuleCode);
        void GenericDelete(GenericRequest request);
        dynamic SaveGenericUDField(GenericRequest request,dynamic entity);
        dynamic RunProcesses(dynamic entity);
        SalesData Sales(Section section);
    }


}
