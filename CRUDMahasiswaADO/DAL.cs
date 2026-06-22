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
        public int CountMhs()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_CountMahasiswa", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter outputParam = new SqlParameter("@Total", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return Convert.ToInt32(outputParam.Value);
                }
            }
        }
        public DataTable GetMhs()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetMahasiswa", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        dtMahasiswa = new DataTable();
                        da.Fill(dtMahasiswa);
                        return dtMahasiswa;
                    }
                }
            }
        }

    }
}