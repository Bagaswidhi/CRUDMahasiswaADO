using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CRUDMahasiswaADO
{
    internal class DAL
    {
        static string connectionString = "Data Source=MSI\\BAGAS;Initial Catalog=DBAkademikADO;Integrated Security=True";
        SqlConnection conn = new SqlConnection(GetConnectionString());
        DataTable dtMahasiswa;
        DataTable dtProdi;
        SqlDataAdapter da;

        public static string GetConnectionString()
        {
            string connectionstring = $"Data Source={GetLocalIPAddress()};Initial Catalog=DBAkademikADO;User ID =sa; Password=bagas3005;";
            return connectionString;
        }
        
    }
}