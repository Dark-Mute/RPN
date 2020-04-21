using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace serwer.Controllers
{
    [Route("api/[controller]")]

    [ApiController]
    public class calculateController : ControllerBase
    {
        [HttpGet]
        [Produces("application/json")]
        [Route("xy")]
        public Models.Imodel xy(string formula, double from,double to, int n)
        {
            RPN rPN = new RPN();
            return rPN.formula(formula,from,to,n);
        }

        [HttpGet]
        [Produces("application/json")]
        public Models.Imodel Get2(string formula, double x)
        {
            RPN rPN = new RPN();
            return rPN.formula(formula, x);
        }

       
    }
}