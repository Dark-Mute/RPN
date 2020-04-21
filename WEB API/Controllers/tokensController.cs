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
    public class tokensController : ControllerBase
    {
        [HttpGet]
        [Produces("application/json")]
        public Models.Imodel Get1(string formula)
        {
            RPN rPN = new RPN();
            return rPN.formula(formula);
        }

       



    }
}