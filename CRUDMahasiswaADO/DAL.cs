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
        public void InsertMhs(string nim, string nama, string alamat, string jeniskelamin, DateTime tanggallahir, string kodeProdi, byte[] FOTO)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlTransaction trans = conn.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_InsertMahasiswa", conn, trans);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pNIM", nim);
                cmd.Parameters.AddWithValue("@pNama", nama);
                cmd.Parameters.AddWithValue("@pJenisKelamin", jeniskelamin);
                cmd.Parameters.AddWithValue("@pTanggalLahir", tanggallahir);
                cmd.Parameters.AddWithValue("@pAlamat", alamat);
                cmd.Parameters.AddWithValue("@pKodeProdi", kodeProdi);
                cmd.Parameters.AddWithValue("@pTanggalDaftar", DateTime.Now);
                cmd.Parameters.AddWithValue("@pFoto", FOTO);


                cmd.ExecuteNonQuery();

                SqlCommand cmdLog = new SqlCommand(@"INSERT INTO LogAktivitas
                                                        (aktivitas, waktu)
                                                        VALUES
                                                        (@aktivitas, GETDATE())",
                                                    conn,
                                                    trans);
                cmdLog.Parameters.AddWithValue("@aktivitas", "INSERT MAHASISWA : " + nim);
                cmdLog.ExecuteNonQuery();
                trans.Commit();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                trans.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
        }
        public void UpdateMhs(string nim, string nama, string alamat, string jeniskelamin, DateTime tanggallahir, string kodeProdi, byte[] FOTO)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            using (SqlCommand cmd = new SqlCommand("sp_UpdateMahasiswa", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@pNIM", nim);
                cmd.Parameters.AddWithValue("@pNama", nama);
                cmd.Parameters.AddWithValue("@pJenisKelamin", jeniskelamin);
                cmd.Parameters.AddWithValue("@pTanggalLahir", tanggallahir);
                cmd.Parameters.AddWithValue("@pAlamat", alamat);
                cmd.Parameters.AddWithValue("@pKodeProdi", kodeProdi);
                cmd.Parameters.AddWithValue("@pFoto", FOTO);

                conn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                {
                    MessageBox.Show("Data berhasil diubah.");
                }

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        public void DeleteMhs(string nim)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            using (SqlCommand cmd = new SqlCommand("sp_DeleteMahasiswa", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pNIM", nim);
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                {
                    MessageBox.Show("Data berhasil dihapus.");
                }
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        public void ResetData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                string query = @"
                        IF OBJECT_ID('dbo.Mahasiswa_Backup') IS NOT NULL
                        BEGIN
                            DELETE FROM dbo.Mahasiswa;
                            INSERT INTO dbo.Mahasiswa
                            SELECT * FROM dbo.Mahasiswa_Backup;
                        END";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void testInject(string nim)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                string query =
                    "UPDATE Mahasiswa SET Nama = 'HACKED' WHERE NIM = " + nim;

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Update berhasil");
                }
            }
        }
        public DataTable GetMhsByNIM(string nim)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetMahasiswaByNIM", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@pNIM", nim);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dtMhs = new DataTable();
                        da.Fill(dtMhs);
                        return dtMhs;
                    }
                }
            }
        }
        public void InsertLog(string message)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO LogError
                                 VALUES
                                 (GETDATE(), @pesan)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pesan", message);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public DataTable getProdi()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("select namaprodi from prodi", conn);
            cmd.CommandType = CommandType.Text;
            dtProdi = new DataTable();
            da = new SqlDataAdapter(cmd);
            da.Fill(dtProdi);

            return dtProdi;
        }
        public DataTable getDataRekap(string prodi, DateTime tanggalmasuk)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_Report", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inProdi", prodi);
            cmd.Parameters.AddWithValue("@inTglMasuk", tanggalmasuk.Year.ToString());

            da = new SqlDataAdapter(cmd);

            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);
            return dtMahasiswa;
        }
        public DataTable getAllDataChart()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand("sp_Dashboard", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);
            return dtMahasiswa;
        }

    }
}