using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkDataAccessLayer
{
    public class SubtitlesDA
    {
        public static DataTable GetAll(int epLinkId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"SELECT ls.SubsId, s.SubsLangId, s.URL from Subtitles s
                                 LEFT JOIN LinkSubs ls on ls.SubsId = s.SubsId
                                 where EpLinkId = @EpLinkId;";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@EpLinkId", epLinkId);

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

        public static int Add(int subsLangId, string url)
        {
            int subsId = -1;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO Subtitles(SubsLangId, URL)
                                 VALUES(@SubsLangId, @URL)
                                 SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SubsLangId", subsLangId);
                    cmd.Parameters.AddWithValue("@URL", url);

                    object insertedRowId = cmd.ExecuteScalar();

                    if (insertedRowId != null && int.TryParse(insertedRowId.ToString(), out int result))
                    {
                        subsId = result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return subsId;
        }

        public static bool Update(int subsId, int subsLangId, string url)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Subtitles
                                 SET SubsLangId = @SubsLangId,
                                     URL = @URL
                                 WHERE SubsId = @SubsId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SubsId", subsId);
                    cmd.Parameters.AddWithValue("@SubsLangId", subsLangId);
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

        public static bool Delete(int subsId)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"DELETE FROM Subtitles WHERE SubsId = @SubsId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SubsId", subsId);

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