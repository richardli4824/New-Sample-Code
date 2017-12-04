using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sample.Controllers
{   
    [RoutePrefix("admin/competency")]
    public class CompetencyController : BaseController
    {
        //GET: UserCompetency
        [Route("index")]
        public ActionResult Index_ng() 
        {
            return View();
        }
    }
}