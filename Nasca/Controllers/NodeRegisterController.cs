using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nasca.Models;
using Newtonsoft.Json;
using Npgsql;
using System.Text.RegularExpressions;

namespace Nasca.Controllers
{
    [Route("NodeRegister/[action]")]
    [ApiController]
    public class NodeRegisterController : ControllerBase
    {
        [HttpPost]
        public ActionResult<string> insert([FromForm]InsertedNode parameter)
        {
            ElementDAO elementDAO = new ElementDAO();

            String parent = (parameter.parentid.Equals(Element.getRoot().getId()) ? "" : parameter.parentid + ".");
            using (var conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=nasca;Encoding=UTF-8;User Id=postgres;Password=nomura;"))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        elementDAO.insert(
                            conn,
                            tran,
                            parent + parameter.id,
                            parameter.name,
                            parameter.type,
                            parameter.remark
                        );

                        this.updateDependencyID(conn, tran, parameter.parentid, parent + parameter.id);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }

            return JsonConvert.SerializeObject(new { });
        }

        [HttpPost]
        public ActionResult<string> update([FromForm]InsertedNode parameter)
        {
            ElementDAO elementDAO = new ElementDAO();

            String parent = (parameter.parentid.Equals(Element.getRoot().getId()) ? "" : parameter.parentid + ".");
            using (var conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=nasca;Encoding=UTF-8;User Id=postgres;Password=nomura;"))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        //更新対象とその子エレメントを取得します。
                        Element target = elementDAO.selectByID(parameter.originalid);
                        List<Element> children = target.getChild();

                        //ID変更の場合
                        if (parameter.originalid != parent + parameter.id)
                        {
                            var re = new Regex(parameter.originalid);

                            //子エレメントとその依存を更新します。
                            foreach (Element child in children)
                            {
                                this.updateDependencyID(conn, tran, child.getId(), re.Replace(child.getId(), parent + parameter.id, 1));

                                //子エレメントの更新
                                elementDAO.update(
                                        conn,
                                        tran,
                                        child.getId(),
                                        re.Replace(child.getId(), parent + parameter.id, 1),
                                        child.getName(),
                                        child.getType(),
                                        child.getRemark());
                            }
                        }

                        this.updateDependencyID(conn, tran, parameter.originalid, parent + parameter.id);

                        elementDAO.update(
                            conn,
                            tran,
                            parameter.originalid,
                            parent + parameter.id,
                            parameter.name,
                            parameter.type,
                            parameter.remark
                        );

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }

            return JsonConvert.SerializeObject(new { });
        }

        [HttpPost]
        public ActionResult<string> delete([FromForm]InsertedNode parameter)
        {
            ElementDAO elementDAO = new ElementDAO();

            String parent = (parameter.parentid.Equals(Element.getRoot().getId()) ? "" : parameter.parentid + ".");
            using (var conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=nasca;Encoding=UTF-8;User Id=postgres;Password=nomura;"))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    //更新対象とその子エレメントを取得します。
                    Element target = elementDAO.selectByID(parent + parameter.id);
                    List<Element> children = target.getChild();

                    //子エレメントとその依存を更新します。
                    foreach (Element child in children)
                    {
                        this.deleteDependency(conn, tran, child.getId());

                        //子エレメントの更新
                        elementDAO.delete(conn, tran, child.getId());
                    }

                    this.deleteDependency(conn, tran, parent + parameter.id);

                    elementDAO.delete(conn, tran, parent + parameter.id);

                    tran.Commit();
                }
            }

            return JsonConvert.SerializeObject(new { });
        }

        private void updateDependencyID(NpgsqlConnection conn, NpgsqlTransaction tran, String originalID, String newID)
        {
            if (originalID.Equals(Element.getRoot().getId())) return;

            DependencyDAO dependencyDAO = new DependencyDAO();

            List<Dependency> dependencies = dependencyDAO.selectByElementID(originalID);
            foreach (Dependency dependency in dependencies)
            {
                dependencyDAO.update(
                    conn,
                    tran,
                    dependency.element.getId(),
                    dependency.dependencyElement.getId(),
                    newID,
                    dependency.dependencyElement.getId(),
                    dependency.dependencyTypeCreate,
                    dependency.dependencyTypeRead,
                    dependency.dependencyTypeUpdate,
                    dependency.dependencyTypeDelete,
                    dependency.remark);
            }

            dependencies = dependencyDAO.selectByDependencyElementID(originalID);
            foreach (Dependency dependency in dependencies)
            {
                dependencyDAO.update(
                    conn,
                    tran,
                    dependency.element.getId(),
                    dependency.dependencyElement.getId(),
                    dependency.element.getId(),
                    newID,
                    dependency.dependencyTypeCreate,
                    dependency.dependencyTypeRead,
                    dependency.dependencyTypeUpdate,
                    dependency.dependencyTypeDelete,
                    dependency.remark);
            }
        }

        private void deleteDependency(NpgsqlConnection conn, NpgsqlTransaction tran, String id)
        {
            DependencyDAO dependencyDAO = new DependencyDAO();

            List<Dependency> dependencies = dependencyDAO.selectByElementID(id);
            foreach (Dependency dependency in dependencies)
            {
                dependencyDAO.delete(conn, tran, dependency.element.getId(), dependency.dependencyElement.getId());
            }

            dependencies = dependencyDAO.selectByDependencyElementID(id);
            foreach (Dependency dependency in dependencies)
            {
                dependencyDAO.delete(conn, tran, dependency.element.getId(), dependency.dependencyElement.getId());
            }
        }
    }
}