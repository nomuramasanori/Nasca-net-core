using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nasca.Models;
using Newtonsoft.Json;
using Npgsql;

namespace Nasca.Controllers
{
    [Route("LinkRegister/[action]")]
    [ApiController]
    public class LinkRegisterController : ControllerBase
    {
        [HttpPost]
        public ActionResult<string> insert([FromForm]InsertedLink parameter)
        {
            DependencyDAO dependnecyDAO = new DependencyDAO();

                dependnecyDAO.insert(
                    parameter.source,
                    parameter.target,
                    parameter.dependencyTypeC,
                    parameter.dependencyTypeR,
                    parameter.dependencyTypeU,
                    parameter.dependencyTypeD,
                    parameter.remark);

            return JsonConvert.SerializeObject(new { });
        }

        [HttpPost]
        public ActionResult<string> update([FromForm]InsertedLink parameter)
        {
            DependencyDAO dependnecyDAO = new DependencyDAO();

            dependnecyDAO.update(
                parameter.source,
                parameter.target,
                parameter.source,
                parameter.target,
                parameter.dependencyTypeC,
                parameter.dependencyTypeR,
                parameter.dependencyTypeU,
                parameter.dependencyTypeD,
                parameter.remark
            );

            return JsonConvert.SerializeObject(new { });
        }

        [HttpPost]
        public ActionResult<string> delete([FromForm]InsertedLink parameter)
        {
            DependencyDAO dependnecyDAO = new DependencyDAO();

            dependnecyDAO.delete(
                parameter.source,
                parameter.target
            );

            dependnecyDAO.update(
                parameter.source,
                parameter.target,
                parameter.source,
                parameter.target,
                parameter.dependencyTypeC,
                parameter.dependencyTypeR,
                parameter.dependencyTypeU,
                parameter.dependencyTypeD,
                parameter.remark
            );

            return JsonConvert.SerializeObject(new { });
        }
    }
}