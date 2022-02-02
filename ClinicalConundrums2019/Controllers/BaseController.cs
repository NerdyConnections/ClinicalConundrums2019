using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ClinicalConundrums2019.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base

        protected override void Initialize(RequestContext requestContext)
        {
          
            if (Session["ProgramID"] != null)
            {

                int ProgramID = Convert.ToInt32(Session["ProgramID"]);
               
            }
            else
            {

                Response.Redirect("/Program/Index");
            }
        }

    }
}