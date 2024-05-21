using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace StreamLinkDataAccessLayer
{
    public static class SubsLangDA
    {
        public static bool Get(int subsLangId, ref string language)
        {
            bool isFound = false;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM SubsLang WHERE SubsLangId = @SubsLangId;";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SubsLangId", subsLangId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (isFound = reader.Read())
                        {
                            language = (string)reader["Language"];
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return isFound;
        }

        public static DataTable GetAll()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM SubsLang;";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return dt;
        }

        public static int Add(string language)
        {
            int subsLangId = -1;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO SubsLang(Language)
                                 VALUES(@Language)
                                 SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SubsLangId", subsLangId);
                    cmd.Parameters.AddWithValue("@Language", language);

                    object insertedRowId = cmd.ExecuteScalar();

                    if (insertedRowId != null && int.TryParse(insertedRowId.ToString(), out int result))
                    {
                        subsLangId = result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return subsLangId;
        }

        public static bool Update(int subsLangId, string language)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE SubsLang
                                 SET Language = @Language
                                 WHERE SubsLangId = @SubsLangId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SubsLangId", subsLangId);
                    cmd.Parameters.AddWithValue("@Language", language);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return rowsAffected > 0;
        }

        public static bool Delete(int subsLangId)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"DELETE FROM SubsLang WHERE SubsLangId = @SubsLangId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SubsLangId", subsLangId);

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