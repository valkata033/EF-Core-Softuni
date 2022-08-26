using System;
using System.Data.SqlClient;
using System.Text;

namespace _03.MinionNames
{
    class StartUp
    {
        public static void Main(string[] args)
        {
            using SqlConnection SqlConnection 
                = new SqlConnection(Connections.ConnectionString);
            SqlConnection.Open();
            int id = int.Parse(Console.ReadLine());

            string result = GetMinionNames(SqlConnection, id);
            Console.WriteLine(result);

            SqlConnection.Close();
        }

        private static string GetMinionNames(SqlConnection sqlConnection, int VillainId)
        {
            StringBuilder sb = new StringBuilder();

            string getVillainNameQuery =
                           @"SELECT Name 
                               FROM Villains 
                              WHERE Id = @VillainId";

            SqlCommand getVillainNameCmd = new SqlCommand(getVillainNameQuery, sqlConnection);
            getVillainNameCmd.Parameters.AddWithValue("@VillainId", VillainId);

            string villainName = (string)getVillainNameCmd.ExecuteScalar();
            if (villainName == null)
            {
                return $"No villain with ID {VillainId} exists in the database.";
            }
            sb.AppendLine($"Villain: {villainName}");

            string getMinionsQuery = @"SELECT ROW_NUMBER()
                                              OVER (ORDER BY m.Name) as RowNum,
                                              m.Name, 
                                              m.Age
                                         FROM MinionsVillains AS mv
                                         JOIN Minions As m ON mv.MinionId = m.Id
                                        WHERE mv.VillainId = @VillainId
                                     ORDER BY m.Name";

            SqlCommand getMinionsCmd = new SqlCommand(getMinionsQuery, sqlConnection);
            getMinionsCmd.Parameters.AddWithValue("@VillainId", VillainId);

            SqlDataReader minionsReader = getMinionsCmd.ExecuteReader();

            while (minionsReader.Read())
            {
                sb.AppendLine($"{minionsReader["RowNum"]}. {minionsReader["Name"]} {minionsReader["Age"]}");
            }


            return sb.ToString().TrimEnd();
        }

    }
}
