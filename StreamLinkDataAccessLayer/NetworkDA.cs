using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkDataAccessLayer
{
    public class NetworkDA
    {
        public static bool Get(int networkId, ref string networkName, ref string websiteLink)
        {
            bool isFound = false;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM Network WHERE NetworkId = @NetworkId;";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@NetworkId", networkId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (isFound = reader.Read())
                        {
                            networkName = (string)reader["NetworkName"];
                            websiteLink = (string)reader["WebsiteLink"];
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return isFound;
        }
        public static DataTable GetAll(int networkId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM Network WHERE NetworkId = @NetworkId;";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@NetworkId", networkId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return dt;
        }

        public static int Add(string networkName, string websiteLink)
        {
            int networkId = -1;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO Network(NetworkName, WebsiteLink)
                                 VALUES(@NetworkName, @WebsiteLink)
                                 SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@NetworkName", networkName);
                    cmd.Parameters.AddWithValue("@WebisteLink", websiteLink);

                    object insertedRowId = cmd.ExecuteScalar();

                    if (insertedRowId != null && int.TryParse(insertedRowId.ToString(), out int result))
                    {
                        networkId = result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return networkId;
        }
        public static bool Update(int networkId, string networkName, string websiteLink)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Network
                                 SET NetworkName = @NetworkName,
                                     WebsiteLink = @WebsiteLink
                                 WHERE NetworkId = @NetworkId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@NetworkId", networkId);
                    cmd.Parameters.AddWithValue("@NetworkName", networkName);
                    cmd.Parameters.AddWithValue("@WebsiteLink", websiteLink);

                    rowsAffected = cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return rowsAffected > 0;
        }
        public static bool Delete(int networkId)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"DELETE FROM Network WHERE NetworkId = @NetworkId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@NetworkId", networkId);

                    rowsAffected = cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return rowsAffected > 0;
        }
    }
}
