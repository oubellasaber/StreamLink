using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkDataAccessLayer
{
    public class SubsLangDA
    {
        public static bool Get(int subsLangId, ref string language)
        {
            bool isFound = false;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"SELECT LangName FROM SubsLang WHERE SubsLangId = @SubsLangId;";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SubsLangId", subsLangId);

                    object result = cmd.ExecuteScalar();

                    if(result != null)
                    {
                        isFound = true;
                        language = (string)result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return isFound;
        }
    }
}