using OpenQA.Selenium.DevTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Policy;

namespace StreamLinkDataAccessLayer
{
    public static class LinkDA
    {
        public static DataTable GetAll(int epId)
        {
            DataTable dt = new DataTable();

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
                            dt.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return dt;
        }
        public static int Add(int epId, int networkId, int sourceId, int hostId, int qualityId, string url)
        {
            int epLinkId = -1;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO Link(EpId, NetworkId, SourceId, HostId, QualityId, URL)
                                 VALUES(@EpId, @NetworkId, @SourceId, @HostId, @QualityId, @URL)
                                 SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@EpId", epId);
                    cmd.Parameters.AddWithValue("@NetworkId", networkId);
                    cmd.Parameters.AddWithValue("@SourceId", sourceId);
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
        public static bool Update(int epLinkId, int epId, int networkId, int sourceId, int hostId, int qualityId, string url)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Link
                                 SET EpId = @EpId,
                                     NetworkId = @NetworkId,
                                     SourceId = @SourceId,
                                     HostId = @HostId,
                                     QualityId = @QualityId,
                                     URL = @Url
                                 WHERE EpLinkId = @EpLinkId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@EpLinkId", epLinkId);
                    cmd.Parameters.AddWithValue("@EpId", epId);
                    cmd.Parameters.AddWithValue("@NetworkId", networkId);
                    cmd.Parameters.AddWithValue("@SourceId", sourceId);
                    cmd.Parameters.AddWithValue("@HostId", hostId);
                    cmd.Parameters.AddWithValue("@QualityId", qualityId);
                    cmd.Parameters.AddWithValue("@URL", url);

                    rowsAffected = cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
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