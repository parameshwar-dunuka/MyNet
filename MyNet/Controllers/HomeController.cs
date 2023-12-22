using Microsoft.Ajax.Utilities;
using MyNet.Helpers;
using MyNet.Models;
using MyNetData.IData;
using MyNetModels.Models;
using MyNetServices.IServices;
using MyNetServices.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using static System.Collections.Specialized.BitVector32;
using Section = MyNet.Models.Section;

namespace MyNet.Controllers
{
    public class HomeController : Controller
    {
        ICommonServices ICommonService = null;
        public HomeController()
        {
            ICommonService = new CommonServices();
        }
        public ActionResult Index()
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Session["userrname"])))
            {
                return RedirectToAction("UDBuilder", "Home", new GenericModel() { PageName = "WelcomePage" });

            }
            else
            {
                return View();
            }
        }

        public ActionResult About()
        {
            Session["userrname"] = null;
            ViewBag.Message = "My Net is a Product Development Portal";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "My Net Designed by Parameshwar";

            return View();
        }

        public ActionResult Login(LoginModel loginModel)
        {
            if(ModelState.IsValid)
            {
                string userdata = Convert.ToString(Session["userrname"]);
                userdata = ICommonService.LoginService(loginModel);
                Session["userrname"] = userdata;

                if (userdata != null && userdata != "")
                {
                    return RedirectToAction("UDBuilder", "Home", new GenericModel() { PageName = "WelcomePage" });
                }
                else
                {
                    ModelState.AddModelError("invalidLogin", "Email or Password not valid");
                }
            }
            else
            {
                return View("Index", loginModel);
            }

            return View("Index",loginModel);
        }



        public ActionResult UDBuilder(GenericModel genericModel)
        {
            //Thread.Sleep(1000);
            string pagedata = string.Empty;
            GenericPage page = null;
            List<WizardModel> Wizardpages;
            try
            {
                if(genericModel.IsWizard==true)
                {
                    int i = 1;
                    Wizardpages = ICommonService.WizardPagexml(genericModel.PageName).OrderBy(x=>x.Sequence).ToList();
                    var current = genericModel.wizardModel.LastOrDefault(x => x.isCurrent == true);
                    pagedata = Wizardpages.FirstOrDefault(x=>x.ProcessName==current.ProcessName).PageXml;
                    Session["Wizardobj_" + genericModel.wizardModel[0].ActiveGuid] = genericModel;
                }
                else
                {
                    pagedata = ICommonService.Pagexml(genericModel.PageName);
                                        
                }
                XmlSerializer xms = new XmlSerializer(typeof(GenericPage));
                using (StringReader reader = new StringReader(pagedata))
                {
                    page = (GenericPage)xms.Deserialize(reader);
                }
                if (genericModel.IsWizard == true)
                {
                    page.IsWizard = true;
                }
                if (genericModel.AdditionalParams?.Contains("Priority")==true)
                {
                    var prior = genericModel.AdditionalParams.Split('=')[1];
                    List<Section> ordersections = new List<Section>();
                    ordersections.Add(page.Body.FirstOrDefault(x => x.SectionName == prior));
                    ordersections.AddRange(page.Body.Where(x=> x.SectionName != prior));
                    page.Body = ordersections;
                }
                TempData["genericData"] = genericModel;
                Session["genericPage"] = page;
                ViewBag.ActiveGuid = genericModel.ActiveGuid;
            }
            catch (Exception e)
            {

            }
            switch (page.Type)
            {
                case "Normal":
                    return PartialView("_Normal", page);
                case "PopUp":
                    return View("_PopUp", page);
                case "SectionPage":
                    return View("_SectionPage", page);
                default:
                    return View("_Normal", page);
            }

        }



        public ActionResult GenericUDBuilder(Section section)
        {
            GenericRequest request = new GenericRequest();
            setRequestData(section, request);
            GenericModel gm=ICommonService.UDService(section, TempData["genericData"] as GenericModel);
            ICommonService.SetDefaultValues(gm);
            setGeneralData(section, gm);
            gm.request = request;
            gm.SectionType = "udfield";
            request.ModelEntity = gm.layouts.entity;
            return PartialView("~/Views/Home/_Section.cshtml", gm);
        }
        public ActionResult GenericGridBuilder(Section section)
        {
            GenericModel gm = ICommonService.UDService(section, TempData["genericData"] as GenericModel);
            GenericRequest request = new GenericRequest();
            setRequestData(section, request);
            gm.request = request;
            ICommonService.SetGridValues(gm);
            setGeneralData(section, gm);
            gm.SectionType = "grid";
            return PartialView("~/Views/Home/_Section.cshtml", gm);
        }
        public ActionResult GenericGridRefresh(GenericRequest request)
        {
            Section section = new Section() { LayoutName=request.LayoutName};
            GenericModel gm = ICommonService.UDService(section, TempData["genericData"] as GenericModel);
            gm.request = request;
            ICommonService.SetGridValues(gm);
            return PartialView("~/Views/Shared/_Grid.cshtml", gm);
        }

        public ActionResult GenericSVBuilder(Section section)
        {
            GenericRequest request=new GenericRequest();
            setRequestData(section, request);
            GenericModel gm = ICommonService.UDService(section, TempData["genericData"] as GenericModel);
            
            ICommonService.SetGridValues(gm);
            setGeneralData(section, gm);
            gm.request= request;
            Section gms = new Section()
            {
                LayoutName=section.LayoutName,
            };
            Session["GenericSVData"]=gms;
            return PartialView("~/Views/Shared/_Smartview.cshtml", gm);
        }

        public void setGeneralData(Section section,GenericModel gm)
        {
            gm.HeaderText = section.Title;
            //gm.PageName = section.LayoutName;
            gm.isFindandUse = section.isFindandUse;
            gm.RuleCode = section.RuleCode;
        }

        private void setRequestData(Section section,GenericRequest request)
        {
            request.LayoutName= section.LayoutName;
            request.WhereQuery=section.WhereQuery;
            request.isFindandUse = section.isFindandUse;
            request.AdditionalParams = string.IsNullOrEmpty(section.AdditionalParams)==true?null: Helper.AdditionalParameters(section.AdditionalParams);
            request.ProcessName=section.ProcessName;
        }
        [HttpGet]
        public ActionResult GetCascading(CascadingModel cm)
        {
            var data=ICommonService.getCascadingvalues(cm);
            var jsonstring =JsonConvert.SerializeObject(data);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GridSearch(Section section)
        {
            var data = Session["GenericSVData"] as Section;
            GenericRequest request = new GenericRequest();
            section.LayoutName = data.LayoutName;
            setRequestData(section, request);
            GenericModel gm = ICommonService.UDService(section, TempData["genericData"] as GenericModel);
            
            gm.request = request;
            ICommonService.SetGridValues(gm);
            setGeneralData(section, gm);

            return PartialView("~/Views/Shared/_Grid.cshtml", gm);
        }

        public ActionResult WizardStart(GenericModel genericModel)
        {
            try
            {
                int i = 1;
                string ActiveGuid = Guid.NewGuid().ToString();
                List<WizardModel> Wizardpages = null;
                if (genericModel.wizardModel == null)
                {
                    Wizardpages = ICommonService.WizardPagexml(genericModel.PageName);
                }
                Wizardpages= Wizardpages.OrderBy(x=>x.Sequence).ToList();
                Wizardpages.ForEach(x =>
                {
                    x.InternalCount = i;
                    if(x.InternalCount == 1)
                    {
                        x.isInitial = true;
                        x.isCurrent = true;
                    }
                    if(Wizardpages.Count==x.InternalCount)
                    {
                        x.isEnd = true;
                    }
                    x.ActiveGuid = ActiveGuid;
                    ++i;
                });
                genericModel.IsWizard = true;
                genericModel.wizardModel = Wizardpages;
            }
            catch (Exception ex)
            {
                var exception = ex;
            }
            return View("Wizard", genericModel);
        }
        public string GetCurrentWizardProcess(string Guid)
        {
            var genericModel = Session["Wizardobj_" + Guid] as GenericModel;
            var Wizardpages = ICommonService.WizardPagexml(genericModel.PageName);
            var current = genericModel.wizardModel.LastOrDefault(x => x.isCurrent == true);
            return current.ProcessName;
        }
        public ActionResult WizardNext(WizardModel WizardGuid)
        {
            var i = 1;
            var genericModel = Session["Wizardobj_" + WizardGuid.ActiveGuid.ToString()] as GenericModel;
            var Wizardpages = ICommonService.WizardPagexml(genericModel.wizardModel[0].PageName);
            var current = genericModel.wizardModel.LastOrDefault(x => x.isCurrent == true);
            current.isCurrent = false;
            if (current.isEnd == true)
            {
                return Json("{\"Type\":\"Success\",\"Msg\":\"Submitted Succedfully\"}", JsonRequestBehavior.AllowGet);
            }
            Wizardpages.ForEach(x =>
            {
                x.InternalCount = i;
                if (x.InternalCount == 1)
                {
                    x.isInitial = true;
                }
                if (Wizardpages.Count == x.InternalCount)
                {
                    x.isEnd = true;
                }
                if(current.InternalCount == i-1)
                {
                    x.isCurrent = true;
                }
                ++i;
            });
            genericModel.wizardModel = Wizardpages;
            return PartialView("_Wizard", genericModel);
        }


        public ActionResult WizardPrevious(WizardModel WizardGuid)
        {
            var i = 1;
            var genericModel = Session["Wizardobj_" + WizardGuid.ActiveGuid.ToString()] as GenericModel;
            var Wizardpages = ICommonService.WizardPagexml(genericModel.PageName);
            var current = genericModel.wizardModel.LastOrDefault(x => x.isCurrent == true);
            current.isCurrent = false;
            Wizardpages.ForEach(x =>
            {
                x.InternalCount = i;
                if (x.InternalCount == 1)
                {
                    x.isInitial = true;
                }
                if (Wizardpages.Count == x.InternalCount)
                {
                    x.isEnd = true;
                }
                if (current.InternalCount == i + 1)
                {
                    x.isCurrent = true;
                }
                ++i;
            });
            genericModel.wizardModel = Wizardpages;
            return PartialView("_Wizard", genericModel);
        }
        [HttpPost]
        public ActionResult GenericSaveUDFields(FormCollection formCollection,GenericRequest request)
        {
            request.AdditionalParams = new List<Dict>();
            var entity=Helper.setFormData(request.ModelEntity, formCollection[0], request.AdditionalParams);

            var root=ICommonService.SaveGenericUDField(request,entity);
            return Json(root, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenericDelete(Section section)
        {
            GenericRequest request = new GenericRequest();

            try
            {
                setRequestData(section, request);
                ICommonService.GenericDelete(request);
            }
            catch (Exception ex)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
            return Json(request,JsonRequestBehavior.AllowGet);
        }


        //////////////////////////////////////////////***********      NON GENERIC           *****************////////////////////////////////////////////////////    
        [HttpPost]
        public ActionResult SavePolicy(PolicyWizard policy,WizardModel WizardGuid, GenericRequest request)
        {
            policy.ActiveGuid = WizardGuid.ActiveGuid;
            policy.ProcessName = GetCurrentWizardProcess(WizardGuid.ActiveGuid);
            ICommonService.SavePolicy(policy,request);
            if(policy.Rules != null && policy.Rules.Count > 0)
            {
                return Json(policy, JsonRequestBehavior.AllowGet);
            }
             return RedirectToAction("WizardNext", WizardGuid);
            
        }
        public ActionResult RunCommission()
        {
            Run run=new Run() { StartTime = DateTime.Now };
            ICommonService.RunProcesses(run);

            return Json("Commission Run Successful",JsonRequestBehavior.AllowGet);

        }
        public ActionResult Sales(Section section)
        {

            SalesData salesData= ICommonService.Sales(section);
            
            return PartialView("_Sales", salesData);

        }
    }
}