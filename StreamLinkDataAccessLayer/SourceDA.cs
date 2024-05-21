using System;
using System.Data;
using System.Data.SqlClient;

namespace StreamLinkDataAccessLayer
{
    public class SourceDA
    {
        public static bool Get(int sourceId, ref string sourceName, ref string sourceDescription)
        {
            bool isFound = false;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM Source WHERE SourceId = @SourceId;";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SourceId", sourceId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (isFound = reader.Read())
                        {
                            sourceName = (string)reader["SourceName"];
                            sourceDescription = (string)reader["SourceDescription"];
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

        public static DataTable GetAll()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM Source";
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
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return dt;
        }

        public static int Add(string sourceName, string sourceDescription)
        {
            int sourceId = -1;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO Source(SourceName, SourceDescription)
                                 VALUES(@SourceName, @SourceDescription)
                                 SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SourceName", sourceName);
                    cmd.Parameters.AddWithValue("@SourceDescription", sourceDescription);

                    object insertedRowId = cmd.ExecuteScalar();

                    if (insertedRowId != null && int.TryParse(insertedRowId.ToString(), out int result))
                    {
                        sourceId = result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return sourceId;
        }

        public static bool Update(int sourceId, string sourceName, string sourceDescription)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Source
                                 SET SourceName = @SourceName,
                                     SourceDescription = @SourceDescription
                                 WHERE SourceId = @SourceId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SourceId", sourceId);
                    cmd.Parameters.AddWithValue("@SourceName", sourceName);
                    cmd.Parameters.AddWithValue("@SourceDescription", sourceDescription);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return rowsAffected > 0;
        }

        public static bool Delete(int sourceId)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"DELETE FROM Role WHERE SourceId = @SourceId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SourceId", sourceId);

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