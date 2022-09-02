using System;
using System.Data.SqlClient;
using System.Text;

namespace _09.IncreaseAgeStoredProcedure
{
    class StartUp
    {
        static void Main(string[] args)
        {
            int minionId = int.Parse(Console.ReadLine());

            SqlConnection sqlConnection =
                new SqlConnection(Connections.ConnectionString);
            sqlConnection.Open();

            string result = IncreaseMinionAge(sqlConnection, minionId);
            Console.WriteLine(result);

            sqlConnection.Close();

        }

        private static string IncreaseMinionAge(SqlConnection sqlConnection, int minionId)
        {
            StringBuilder sb = new StringBuilder();

            string increaseAgeQuery = @"EXEC [dbo].[usp_GetOlder] @minionId";
            SqlCommand increaseAgeCmd = new SqlCommand(increaseAgeQuery, sqlConnection);
            increaseAgeCmd.Parameters.AddWithValue("@minionId", minionId);

            increaseAgeCmd.ExecuteNonQuery();

            string minionsInfoQuery = @"SELECT Name, 
                                               Age 
                                          FROM Minions 
                                         WHERE Id = @minionId";

            SqlCommand minionsInfoCmd = new SqlCommand(minionsInfoQuery, sqlConnection);
            minionsInfoCmd.Parameters.AddWithValue("@minionId", minionId);

            using SqlDataReader reader = minionsInfoCmd.ExecuteReader();

            while (reader.Read())
            {
                sb.AppendLine($"{reader["Name"]} – {reader["Age"]} years old");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
