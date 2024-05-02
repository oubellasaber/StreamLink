using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkDataAccessLayer
{
    internal class LinkSubsMapperDA
    {
        public static List<object> GetAll(int epLinkId)
        {
            List<object> linkSubMappers = new List<object>();

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM LinkSubsMapper WHERE EpLinkId = @EpLinkId;";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@EpLinkId", epLinkId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var linkSubsMapper = new
                            {
                                LinkSubsMapperId = (int)reader["LinkSubsMapperId"],
                                EpId = (int)reader["EpId"],
                                EpLinkId = (int)reader["EpLinkId"],
                                SubsId = (int)reader["SubsId"]
                            }
                            linkSubMappers.Add(linkSubsMapper);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return linkSubMappers;
        }
    }
}
