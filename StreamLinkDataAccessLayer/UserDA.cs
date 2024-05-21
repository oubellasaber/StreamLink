using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.DevTools.V122.DOM;

namespace StreamLinkDataAccessLayer
{
    public class UserDA
    {
        public static bool Get(int userId, ref string username, ref string password, ref int roleId)
        {
            bool isFound = false;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM [User] WHERE UserId = @UserId;";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (isFound = reader.Read())
                        {
                            username = (string)reader["Username"];
                            password = (string)reader["Password"];
                            roleId = (int)reader["RoleId"];
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

        public static bool Get(ref int userId, string username, ref string password, ref int userPermissions)
        {
            bool isFound = false;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM [User] WHERE Username = @Username;";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (isFound = reader.Read())
                        {
                            userId = (int)reader["UserId"];
                            password = (string)reader["Password"];
                            userPermissions = (int)reader["RoleId"];
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

        public static int Add(string username, string password, int userPermissions)
        {
            int userId = -1;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO [User](Username, Password, UserPermissions)
                                 VALUES(@Username, @Password, @UserPermissions)
                                 SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@UserPermissions", userPermissions);

                    object insertedRowId = cmd.ExecuteScalar();

                    if (insertedRowId != null && int.TryParse(insertedRowId.ToString(), out int result))
                    {
                        userId = result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return userId;
        }

        public static bool Update(int userId, string username, string password, int userPermissions)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE User
                                 SET Username = @Username,
                                     Password = @Password,
                                     UserPermissions = @UserPermissions
                                 WHERE UserId = @UserId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@UserPermissions", userPermissions);

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
