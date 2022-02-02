using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicalConundrums2019.Controllers
{
    public class ProgramDetailController : Controller
    {
        // GET: Program
        public ActionResult Index(int ProgramID)
        {
            Session["ProgramID"] = ProgramID;
            return View();
        }
    }
}