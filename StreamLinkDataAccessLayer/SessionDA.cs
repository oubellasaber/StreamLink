using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkDataAccessLayer
{
    public class SessionDA
    {
        public static bool Get(string sessionId, ref int userId)
        {
            bool isFound = false;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"SELECT UserId FROM Session WHERE SessionId = @SessionId;";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SessionId", sessionId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (isFound = reader.Read())
                        {
                            userId = (int)reader["UserId"];
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

        private static string GenerateSessionId()
        {
            byte[] randomBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }


        public static string Add(int userId)
        {
            string sessionId = SessionDA.GenerateSessionId();

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO Session(SessionId, UserId)
                                 VALUES(@SessionId, @UserId);";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SessionId", sessionId);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return sessionId;
        }

        /*
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
        */
    }
}
