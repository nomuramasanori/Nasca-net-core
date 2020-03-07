using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Dapper;

namespace Nasca.Models
{
    public class ElementTypeDAO
    {
        //コンストラクタ
        public ElementTypeDAO() { }

        public List<ElementType> selectAll()
        {
            List<ElementType> result = null;

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = "Server=127.0.0.1;Port=5432;Database=nasca;Encoding=UTF-8;User Id=postgres;Password=nomura;";
                connection.Open();
                string sql =
                    "select"
                    + "	elmttp as elementType,"
                    + "	svgfle as svgFile"
                    + " from"
                    + "	m_elmttype";

                result = connection.Query<ElementType>(sql).ToList();
            }

            return result;
        }
    }
}
