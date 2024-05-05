﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkDataAccessLayer
{
    public class VideoQualityDA
    {
        public static bool Get(int qualityId, ref string qualityName)
        {
            bool isFound = false;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"SELECT QualityName FROM VideoQuality WHERE QualityId = @QualityId;";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@QualityId", qualityId);
    
                    object result = cmd.ExecuteScalar();

                    if(result != null)
                    {
                        isFound = true;
                        qualityName = (string)result;
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