using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Policy;

namespace StreamLinkDataAccessLayer
{
    public static class LinkDA
    {
        public static List<object> GetAll(int epId)
        {
            List<object> links = new List<object>();

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM Link WHERE EpId = @EpId;";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@EpID", epId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var link = new
                            {
                                EpLinkId = (int)reader["EpLinkId"],
                                NetworkId = (int)reader["NetworkId"],
                                EpId = (int)reader["EpId"],
                                HostId = (int)reader["HostId"],
                                QualityId = (int)reader["QualityId"],
                                Url = (string)reader["URL"]
                            };
                            links.Add(link);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return links;
        }

        // Contributors - (Link extractoes - admins)

        public static int Add(int epId, int hostId, int qualityId, string url)
        {
            int epLinkId = -1;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO Link(EpId, HostId, QualityId, URL)
                                 VALUES(@EpId, @HostId, @QualityId, @URL)
                                 SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@EpId", epId);
                    cmd.Parameters.AddWithValue("@HostId", hostId);
                    cmd.Parameters.AddWithValue("@QualityId", qualityId);
                    cmd.Parameters.AddWithValue("@URL", url);

                    object insertedRowId = cmd.ExecuteScalar();

                    if (insertedRowId != null && int.TryParse(insertedRowId.ToString(), out int result))
                    {
                        epLinkId = result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return epLinkId;
        }

        public static bool Update(int epLinkId, int epId, int hostId, int qualityId, string url)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Link
                                 SET EpId = @EpId,
                                     HostId = @HostId,
                                     QualityId = @QualityId,
                                     URL = @URL
                                 WHERE EpLinkId = @EpLinkId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@EpLinkId", epLinkId);
                    cmd.Parameters.AddWithValue("@EpId", epId);
                    cmd.Parameters.AddWithValue("@HostId", hostId);
                    cmd.Parameters.AddWithValue("@QualityId", qualityId);
                    cmd.Parameters.AddWithValue("@URL", url);

                    rowsAffected = cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return rowsAffected > 0;
        }

        public static bool Delete(int epLinkId)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"DELETE FROM Link WHERE EpLinkId = @EpLinkId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@EpLinkId", epLinkId);

                    rowsAffected = cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return rowsAffected > 0;
        }
    }
}