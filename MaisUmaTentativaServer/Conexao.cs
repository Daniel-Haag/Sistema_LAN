using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace OABLanServer
{
    class Conexao
    {
        SqlConnection con = new SqlConnection();

        public Conexao()
        {
            //con.ConnectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=OABLAN;Integrated Security=True";
            //con.ConnectionString = @"Data Source=192.168.254.231\BDSQL-01;Initial Catalog=OABLAN;Integrated Security=True;User id=sa; Password=sdja@esdFJ*!2; Connection Timeout=0 ";
            con.ConnectionString = @"Data Source=192.168.254.231;Initial Catalog=OABLAN;User id=desenvolvimento;Password=sdja@esdFJ*!2";
            //con.ConnectionString = @"User id=Desenvolvimento; Password=sdja@esdFJ*!2;Persist Security Info=True;Initial Catalog=OABLAN;Data Source=192.168.254.231";
            
        }

        public SqlConnection conectar()
        {
            if(con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }

            return con;
        }

        public void Desconectar()
        {
            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Close();
            }
        }
    }
}
