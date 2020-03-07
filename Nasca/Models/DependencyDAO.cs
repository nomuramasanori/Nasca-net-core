using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Dapper;

namespace Nasca.Models
{
    public class DependencyDAO
    {
        //コンストラクタ
        public DependencyDAO() { }

        public List<Dependency> selectByElementID(String id)
        {
            List<Dependency> result = null;

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = "Server=127.0.0.1;Port=5432;Database=nasca;Encoding=UTF-8;User Id=postgres;Password=nomura;";
                connection.Open();
                //string sql =
                //        " select"
                //        + " 	concat(elm1.elmtid, '-', elm2.elmtid) id,"
                //        + " 	elm1.elmtid,"
                //        + " 	elm1.elmtnm,"
                //        + " 	elm1.elmttp,"
                //        + " 	elm1.remark,"
                //        + " 	etp1.svgfle,"
                //        + " 	elm2.elmtid dpdelmtid,"
                //        + " 	elm2.elmtnm dpdelmtnm,"
                //        + " 	elm2.elmttp dpdelmttp,"
                //        + " 	elm2.remark dpdremark,"
                //        + " 	etp2.svgfle dpdsvgfle,"
                //        + " 	dpd.remark dpdrmk,"
                //        + " 	dpd.dpdtpc,"
                //        + " 	dpd.dpdtpr,"
                //        + " 	dpd.dpdtpu,"
                //        + " 	dpd.dpdtpd"
                //        + " from"
                //        + " 	t_depndncy dpd"
                //        + " 	inner join m_element elm1 on"
                //        + " 		dpd.elmtid = elm1.elmtid"
                //        + " 	inner join m_element elm2 on"
                //        + " 		dpd.dpdeid = elm2.elmtid"
                //        + " 	left outer join m_elmttype etp1 on"
                //        + "     	elm1.elmttp = etp1.elmttp"
                //        + " 	left outer join m_elmttype etp2 on"
                //        + "     	elm2.elmttp = etp2.elmttp"
                //        + " where"
                //        + " 	dpd.elmtid = @id";

                string sql =
                        " select"
                        + " 	concat(dpd.elmtid, '-', dpd.dpdeid) as id,"
                        + " 	dpd.remark as remark,"
                        + " 	dpd.dpdtpc as dependencyTypeCreate,"
                        + " 	dpd.dpdtpr as dependencyTypeRead,"
                        + " 	dpd.dpdtpu as dependencyTypeUpdate,"
                        + " 	dpd.dpdtpd as dependencyTypeDelete"
                        + " from"
                        + " 	t_depndncy dpd"
                        + " where"
                        + " 	dpd.elmtid = @id";

                result = connection.Query<Dependency>(sql, new { id = id}).ToList();

                result.ForEach(dependency => {
                    var ids = dependency.id.Split('-');
                    var elementDAO = new ElementDAO();
                    dependency.element = elementDAO.selectByID(ids[0]);
                    dependency.dependencyElement = elementDAO.selectByID(ids[1]);
                });
            }

            return result;
        }


        public List<Dependency> selectByDependencyElementID(String id)
        {
            List<Dependency> result = null;

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = "Server=127.0.0.1;Port=5432;Database=nasca;Encoding=UTF-8;User Id=postgres;Password=nomura;";
                connection.Open();
                //string sql =
                //    " select"
                //    + " 	concat(elm1.elmtid, '-', elm2.elmtid) id,"
                //    + " 	elm1.elmtid,"
                //    + " 	elm1.elmtnm,"
                //    + " 	elm1.elmttp,"
                //    + " 	elm1.remark,"
                //    + " 	etp1.svgfle,"
                //    + " 	elm2.elmtid dpdelmtid,"
                //    + " 	elm2.elmtnm dpdelmtnm,"
                //    + " 	elm2.elmttp dpdelmttp,"
                //    + " 	elm2.remark dpdremark,"
                //    + " 	etp2.svgfle dpdsvgfle,"
                //    + " 	dpd.remark dpdrmk,"
                //    + " 	dpd.dpdtpc,"
                //    + " 	dpd.dpdtpr,"
                //    + " 	dpd.dpdtpu,"
                //    + " 	dpd.dpdtpd"
                //    + " from"
                //    + " 	t_depndncy dpd"
                //    + " 	inner join m_element elm1 on"
                //    + " 		dpd.elmtid = elm1.elmtid"
                //    + " 	inner join m_element elm2 on"
                //    + " 		dpd.dpdeid = elm2.elmtid"
                //    + " 	left outer join m_elmttype etp1 on"
                //    + "     	elm1.elmttp = etp1.elmttp"
                //    + " 	left outer join m_elmttype etp2 on"
                //    + "     	elm2.elmttp = etp2.elmttp"
                //    + " where"
                //    + " 	dpd.dpdeid = @id";

                string sql =
                    " select"
                    + " 	concat(dpd.elmtid, '-', dpd.dpdeid) as id,"
                    + " 	dpd.remark as remark,"
                    + " 	dpd.dpdtpc as dependencyTypeCreate,"
                    + " 	dpd.dpdtpr as dependencyTypeRead,"
                    + " 	dpd.dpdtpu as dependencyTypeUpdate,"
                    + " 	dpd.dpdtpd as dependencyTypeDelete"
                    + " from"
                    + " 	t_depndncy dpd"
                    + " where"
                    + " 	dpd.dpdeid = @id";

                result = connection.Query<Dependency>(sql, new { id = id }).ToList();

                result.ForEach(dependency =>
                {
                    var ids = dependency.id.Split('-');
                    var elementDAO = new ElementDAO();
                    dependency.element = elementDAO.selectByID(ids[0]);
                    dependency.dependencyElement = elementDAO.selectByID(ids[1]);
                });
            }

            return result;
        }

        public void insert(String elementID, String dependencyElementID, bool isDependencyTypeCreate, bool isDependencyTypeRead, bool isDependencyTypeUpdate, bool isDependencyTypeDelete, String remark)
        {
            using (var conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=nasca;Encoding=UTF-8;User Id=postgres;Password=nomura;"))
            {
                // コネクション確立.
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        conn.Execute(
                            "insert into t_depndncy (elmtid,dpdeid,dpdtpc,dpdtpr,dpdtpu,dpdtpd,remark) values (@elmtid,@dpdeid,@dpdtpc,@dpdtpr,@dpdtpu,@dpdtpd,@remark)",
                            new
                            {
                                elmtid = elementID,
                                dpdeid = dependencyElementID,
                                dpdtpc = isDependencyTypeCreate,
                                dpdtpr = isDependencyTypeRead,
                                dpdtpu = isDependencyTypeUpdate,
                                dpdtpd = isDependencyTypeDelete,
                                remark = remark
                            }, tran);

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
        }


        public void update(String originalElementID, String originalDependencyElementID, String elementID, String dependencyElementID, bool isDependencyTypeCreate, bool isDependencyTypeRead, bool isDependencyTypeUpdate, bool isDependencyTypeDelete, String remark)
        {
            using (var conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=nasca;Encoding=UTF-8;User Id=postgres;Password=nomura;"))
            {
                // コネクション確立.
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        conn.Execute(
                                " update t_depndncy"
                                + " set"
                                + "   elmtid = @elmtid,"
                                + "   dpdeid = @dpdeid,"
                                + "   dpdtpc = @dpdtpc,"
                                + "   dpdtpr = @dpdtpr,"
                                + "   dpdtpu = @dpdtpu,"
                                + "   dpdtpd = @dpdtpd,"
                                + "   remark = @remark"
                                + " where"
                                + "   elmtid = @originalelmtid and"
                                + "   dpdeid = @originaldpdeid",
                            new
                            {
                                elmtid = elementID,
                                dpdeid = dependencyElementID,
                                dpdtpc = isDependencyTypeCreate,
                                dpdtpr = isDependencyTypeRead,
                                dpdtpu = isDependencyTypeUpdate,
                                dpdtpd = isDependencyTypeDelete,
                                remark = remark,
                                originalelmtid = originalElementID,
                                originaldpdeid = originalDependencyElementID
                            }, tran);

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
        }
        public void update(NpgsqlConnection conn, NpgsqlTransaction tran, String originalElementID, String originalDependencyElementID, String elementID, String dependencyElementID, bool isDependencyTypeCreate, bool isDependencyTypeRead, bool isDependencyTypeUpdate, bool isDependencyTypeDelete, String remark)
        {
            conn.Execute(
                    " update t_depndncy"
                    + " set"
                    + "   elmtid = @elmtid,"
                    + "   dpdeid = @dpdeid,"
                    + "   dpdtpc = @dpdtpc,"
                    + "   dpdtpr = @dpdtpr,"
                    + "   dpdtpu = @dpdtpu,"
                    + "   dpdtpd = @dpdtpd,"
                    + "   remark = @remark"
                    + " where"
                    + "   elmtid = @originalelmtid and"
                    + "   dpdeid = @originaldpdeid",
                new
                {
                    elmtid = elementID,
                    dpdeid = dependencyElementID,
                    dpdtpc = isDependencyTypeCreate,
                    dpdtpr = isDependencyTypeRead,
                    dpdtpu = isDependencyTypeUpdate,
                    dpdtpd = isDependencyTypeDelete,
                    remark = remark,
                    originalelmtid = originalElementID,
                    originaldpdeid = originalDependencyElementID
                }, tran);
        }

        public void delete(String elementID, String dependencyElementID)
        {
            using (var conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=nasca;Encoding=UTF-8;User Id=postgres;Password=nomura;"))
            {
                // コネクション確立.
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        conn.Execute(
                            " delete from t_depndncy"
                            + " where"
                            + " elmtid = @elmtid and"
                            + " dpdeid = @dpdeid",
                            new
                            {
                                elmtid = elementID,
                                dpdeid = dependencyElementID
                            }, tran);

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
        }

        public void delete(NpgsqlConnection conn, NpgsqlTransaction tran, String elementID, String dependencyElementID)
        {
            conn.Execute(
                " delete from t_depndncy"
                + " where"
                + " elmtid = @elmtid and"
                + " dpdeid = @dpdeid",
                new
                {
                    elmtid = elementID,
                    dpdeid = dependencyElementID
                }, tran);
        }
    }
}
