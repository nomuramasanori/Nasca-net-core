using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Dapper;

namespace Nasca.Models
{
    public class ElementDAO
    {
        //コンストラクタ
        public ElementDAO() { }

        public Element selectByID(String id)
        {
            Element result = null;

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = "Server=127.0.0.1;Port=5432;Database=nasca;Encoding=UTF-8;User Id=postgres;Password=nomura;";
                connection.Open();
                string sql =
                    " select"
                    + " 	obj.elmtid as id,"
                    + " 	obj.elmtnm as name,"
                    + " 	obj.elmttp as type,"
                    + " 	obj.remark as remark,"
                    + " 	otp.svgfle as svgFile"
                    + " from"
                    + " 	m_element obj LEFT OUTER JOIN"
                    + " 	m_elmttype otp ON"
                    + " 		obj.elmttp = otp.elmttp"
                    + " where"
                    + " 	obj.elmtid = @id";

                result = connection.Query<Element>(sql, new { id = id}).First();
            }

            return result;
        }


        public List<Element> selectAll()
        {
            List<Element> result = null;

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = "Server=127.0.0.1;Port=5432;Database=nasca;Encoding=UTF-8;User Id=postgres;Password=nomura;";
                connection.Open();
                string sql =
                    " select"
                    + " 	obj.elmtid as id,"
                    + " 	obj.elmtnm as name,"
                    + " 	obj.elmttp as type,"
                    + " 	obj.remark as remark,"
                    + " 	otp.svgfle as svgfile"
                    + " from"
                    + " 	m_element obj LEFT OUTER JOIN"
                    + " 	m_elmttype otp ON"
                    + " 		obj.elmttp = otp.elmttp";

                result = connection.Query<Element>(sql).ToList();
            }

            return result;
        }

        public List<Element> selectChild(String id)
        {
            List<Element> result = null;

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = "Server=127.0.0.1;Port=5432;Database=nasca;Encoding=UTF-8;User Id=postgres;Password=nomura;";
                connection.Open();
                string sql =
                    " select"
                    + " 	obj.elmtid as id,"
                    + " 	obj.elmtnm as name,"
                    + " 	obj.elmttp as type,"
                    + " 	obj.remark as remark,"
                    + " 	otp.svgfle as svgFile"
                    + " from"
                    + " 	m_element obj LEFT OUTER JOIN"
                    + " 	m_elmttype otp ON"
                    + " 		obj.elmttp = otp.elmttp"
                    + " where"
                    + " 	obj.elmtid like concat(@id, '.%')";

                result = connection.Query<Element>(sql, new { id = id}).ToList();
            }

            return result;
        }


        public void insert(String elementID, String elementName, String elementType, String remark)
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
                            "insert into m_element(elmtid,elmtnm,elmttp,remark) values (@elmtid,@elmtnm,@elmttp,@remark)",
                            new
                            {
                                elmtid = elementID,
                                elmtnm = elementName,
                                elmttp = elementType,
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
        public void insert(NpgsqlConnection conn, NpgsqlTransaction tran, String elementID, String elementName, String elementType, String remark)
        {
            conn.Execute(
                "insert into m_element(elmtid,elmtnm,elmttp,remark) values (@elmtid,@elmtnm,@elmttp,@remark)",
                new
                {
                    elmtid = elementID,
                    elmtnm = elementName,
                    elmttp = elementType,
                    remark = remark
                }, tran);
        }

        public void update(String originalElementID, String elementID, String elementName, String elementType, String remark)
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
                            " update m_element"
                            + " set"
                            + " 	elmtid = @elmtid,"
                            + " elmtnm = @elmtnm,"
                            + " elmttp = @elmttp,"
                            + " remark = @remark"
                            + " where"
                            + " elmtid = @originalelmtid",
                            new
                            {
                                elmtid = elementID,
                                elmtnm = elementName,
                                elmttp = elementType,
                                remark = remark,
                                originalelmtid = originalElementID
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

        public void update(NpgsqlConnection conn, NpgsqlTransaction tran, String originalElementID, String elementID, String elementName, String elementType, String remark)
        {
            conn.Execute(
                " update m_element"
                + " set"
                + " 	elmtid = @elmtid,"
                + " elmtnm = @elmtnm,"
                + " elmttp = @elmttp,"
                + " remark = @remark"
                + " where"
                + " elmtid = @originalelmtid",
                new
                {
                    elmtid = elementID,
                    elmtnm = elementName,
                    elmttp = elementType,
                    remark = remark,
                    originalelmtid = originalElementID
                }, tran);
        }

        public void delete(String elementID)
        {
            using (var conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=nasca;Encoding=UTF-8;User Id=postgres;Password=nomura;"))
            {
                // コネクション確立.
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        this.delete(conn, tran, elementID);

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
        }
        public void delete(NpgsqlConnection conn, NpgsqlTransaction tran, String elementID)
        {
            conn.Execute(
                " delete from m_element"
                + " where"
                + " elmtid = @elmtid",
                new
                {
                    elmtid = elementID
                }, tran);
        }
    }
}
