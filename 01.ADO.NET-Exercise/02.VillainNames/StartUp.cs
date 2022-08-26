using System;
using System.Data.SqlClient;
using System.Text;


namespace _02.VillainNames
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            using SqlConnection SqlConnection = 
                new SqlConnection(Connections.ConnectionString);
            SqlConnection.Open();

            string result = GetVillainNamesWithMinionsCount(SqlConnection);
            Console.WriteLine(result); 

            SqlConnection.Close();
        }

        private static string GetVillainNamesWithMinionsCount(SqlConnection sqlConnection)
        {
            StringBuilder sb = new StringBuilder();

            string query = @" SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                                FROM Villains AS v 
                                JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
                            GROUP BY v.Id, v.Name 
                              HAVING COUNT(mv.VillainId) > 3 
                            ORDER BY COUNT(mv.VillainId)";

            SqlCommand cmd = new SqlCommand(query, sqlConnection);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                sb.AppendLine($"{reader["Name"]} - {reader["MinionsCount"]}");
            }

            return sb.ToString().TrimEnd();
        }

    }
}
