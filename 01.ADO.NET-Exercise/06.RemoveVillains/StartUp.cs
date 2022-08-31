using System;
using System.Data.SqlClient;
using System.Text;

namespace _06.RemoveVillains
{
    class StartUp
    {
        static void Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine());

            SqlConnection sqlConnection = 
                new SqlConnection(Connections.ConnectionString);
            sqlConnection.Open();

            string result = DeleteVillain(sqlConnection, villainId);
            Console.WriteLine(result);

            sqlConnection.Close();
        }

        private static string DeleteVillain(SqlConnection sqlConnection, int villainId)
        {
            StringBuilder sb = new StringBuilder();

            string getVillainQuery = @"SELECT Name 
                               FROM Villains 
                              WHERE Id = @villainId";

            SqlCommand getVillainCmd = new SqlCommand(getVillainQuery, sqlConnection);
            getVillainCmd.Parameters.AddWithValue("@villainId", villainId);

            string villainName = (string)getVillainCmd.ExecuteScalar();
            if (villainName == null)
            {
                return $"No such villain was found.";
            }

            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            string releaseMinionsQuery = @"DELETE FROM MinionsVillains 
                                                 WHERE VillainId = @villainId";

            SqlCommand releaseMinionsCmd = new SqlCommand(releaseMinionsQuery, sqlConnection, sqlTransaction);
            releaseMinionsCmd.Parameters.AddWithValue("@villainId", villainId);

            int minionsReleased = (int)releaseMinionsCmd.ExecuteNonQuery();

            string deleteVillainQuery = @"DELETE FROM Villains
                                                WHERE Id = @villainId";

            SqlCommand deleteVillainCmd = new SqlCommand(deleteVillainQuery, sqlConnection, sqlTransaction);
            deleteVillainCmd.Parameters.AddWithValue("@villainId", villainId);

            int deletedVillains = (int)deleteVillainCmd.ExecuteNonQuery();

            if (deletedVillains != 1)
            {
                sqlTransaction.Rollback();
            }

            sqlTransaction.Commit();

            sb.AppendLine($"{villainName} was deleted.")
              .AppendLine($"{minionsReleased} minions were released.");

            return sb.ToString().TrimEnd();
        }

    }
}
