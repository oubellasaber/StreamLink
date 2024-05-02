using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkDataAccessLayer
{
    public class SubtitlesDA
    {
        public static List<object> GetAll(int epLinkId)
        {
            List<object> subtitlesList = new List<object>();

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"SELECT lsm.SubsId, s.SubsLangId, s.URL from Subtitles s
                                 LEFT JOIN LinkSubsMapper lsm on lsm.SubsId = s.SubsId
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
                            var subs = new
                            {
                                SubsId = (int)reader["SubsId"],
                                SubsLangId = (int)reader["SubsLangId"],
                                Url = (string)reader["URL"],
                            };
                            subtitlesList.Add(subs);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return subtitlesList;
        }
    }
}
