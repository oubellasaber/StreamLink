using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkDataAccessLayer
{
    public class SerieDA
    {
        public static bool Get(int serieId, ref string title, ref int totalEps, ref int uploadedEps)
        {
            bool isFound = false;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM Serie WHERE SerieId = @SerieId;";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SerieId", serieId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (isFound = reader.Read())
                        {
                            title = (string)reader["Title"];
                            totalEps = (int)reader["TotalEps"];
                            uploadedEps = (int)reader["UploadedEps"];
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

        public static int Add(int serieId, string title, int totalEps, int uploadedEps)
        {
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO Serie(Title, TotalEps, UploadedEps)
                                 VALUES(@Title, @TotalEps, @UploadedEps)
                                 SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SerieId", serieId);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@TotalEps", totalEps);
                    cmd.Parameters.AddWithValue("@UploadedEps", uploadedEps);

                    object insertedRowId = cmd.ExecuteScalar();

                    if (insertedRowId != null && int.TryParse(insertedRowId.ToString(), out int result))
                    {
                        serieId = result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return serieId;
        }

        public static bool Update(int serieId, string title, int totalEps, int uploadedEps)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Serie
                                 SET Title = @Title,
                                     TotalEps = @TotalEps,
                                     uploadedEps = @uploadedEps
                                 WHERE SerieId = @SerieId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SerieId", serieId);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@TotalEps", totalEps);
                    cmd.Parameters.AddWithValue("@UploadedEps", uploadedEps);

                    rowsAffected = cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return rowsAffected > 0;
        }

        public static bool Delete(int serieId)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"DELETE FROM Serie WHERE SerieId = @SerieId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SerieId", serieId);

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
