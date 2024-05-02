using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkDataAccessLayer
{
    public class EpDA
    {
        public static bool Get(int epId, ref int epNum, ref int serieId)
        {
            bool isFound = false;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM Ep WHERE EpId = @EpId;";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@EpId", epId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (isFound = reader.Read())
                        {
                            epNum = (int)reader["EpNum"];
                            serieId = (int)reader["SerieId"];
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

        public static List<object> GetInRange(int serieId, int from, int to)
        {
            List<object> eps = new List<object>();

            using (SqlConnection connection = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = $@"SELECT * FROM Ep WHERE SerieId = @SerieId and EpNum BETWEEN {from} AND {to};";
                SqlCommand cmd = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@SerieId", serieId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var ep = new
                            {
                                EpId = (int)reader["EpId"],
                                EpNum = (int)reader["EpNum"],
                                SerieId = (int)reader["SerieId"]
                            };

                            eps.Add(ep);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return eps;
        }

        public static int Add(int epNum, int serieId)
        {
            int epId = -1;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO Ep(EpNum, SerieId)
                                 VALUES(@EpNum, @SerieId)
                                 SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@EpNum", epNum);
                    cmd.Parameters.AddWithValue("@SerieId", serieId);

                    object insertedRowId = cmd.ExecuteScalar();

                    if (insertedRowId != null && int.TryParse(insertedRowId.ToString(), out int result))
                    {
                        epId = result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return epId;
        }

        public static bool Update(int epId, int epNum, int serieId)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Ep
                                 SET EpNum = @EpNum,
                                     SerieId = @SerieId
                                 WHERE EpId = @EpId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@EpId", epId);
                    cmd.Parameters.AddWithValue("@EpNum", epNum);
                    cmd.Parameters.AddWithValue("@SerieId", serieId);

                    rowsAffected = cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return rowsAffected > 0;
        }

        public static bool Delete(int epId)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"DELETE FROM Ep WHERE EpId = @EpId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@EpId", epId);

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