using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace _04.AddMinion
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            string[] minionInformation = Console.ReadLine()
                .Split(": ", StringSplitOptions.RemoveEmptyEntries)[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            string villainName = Console.ReadLine()
                .Split(": ", StringSplitOptions.RemoveEmptyEntries)[1];

            SqlConnection sqlConnection =
                new SqlConnection(Connections.ConnectionString);
            sqlConnection.Open();

            string result = AddNewMinion(sqlConnection, minionInformation, villainName);
            Console.WriteLine(result);

            sqlConnection.Close();
        }

        private static string AddNewMinion(SqlConnection sqlConnection,
            string[] minionInformation, string villainName)
        {
            StringBuilder sb = new StringBuilder();

            string minionName = minionInformation[0];
            int minionAge = int.Parse(minionInformation[1]);
            string minionTown = minionInformation[2];

            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            try
            {
                int townId = GetTownId(sqlConnection, sb, minionTown, sqlTransaction);
                int villainId = GetVillainId(sqlConnection, villainName, sb, sqlTransaction);
                int minionId = AddMinion(sqlConnection, minionName, minionAge, sqlTransaction, townId);

                string addMinionToVillainQuery = @"INSERT INTO MinionsVillains (MinionId, VillainId) 
                                                        VALUES (@minionId, @villainId)";

                SqlCommand addMinionToVillainCmd = new SqlCommand(addMinionToVillainQuery, sqlConnection, sqlTransaction);
                addMinionToVillainCmd.Parameters.AddWithValue("@minionId", minionId);
                addMinionToVillainCmd.Parameters.AddWithValue("@villainId", villainId);
                
                addMinionToVillainCmd.ExecuteNonQuery();
                sb.AppendLine($"Successfully added {minionName} to be minion of {villainName}.");

            }
            catch (Exception e)
            {
                sqlTransaction.Rollback();
                return e.ToString();
            }
            finally
            {
                sqlTransaction.Commit();
            }

            return sb.ToString().TrimEnd();
        }

        private static int AddMinion(SqlConnection sqlConnection, string minionName, int minionAge, SqlTransaction sqlTransaction, int townId)
        {
            string addMinionQuery = @"INSERT INTO Minions (Name, Age, TownId) 
                                               VALUES (@name, @age, @townId)";

            SqlCommand addMinionCmd = new SqlCommand(addMinionQuery, sqlConnection, sqlTransaction);
            addMinionCmd.Parameters.AddWithValue("@name", minionName);
            addMinionCmd.Parameters.AddWithValue("@age", minionAge);
            addMinionCmd.Parameters.AddWithValue("@townId", townId);

            addMinionCmd.ExecuteNonQuery();

            string addedMinionId = @"SELECT [Id]
                                       FROM [Minions]
                                      WHERE [Name] = @minionName AND [Age] = @minionAge AND [TownId] = @TownId";

            SqlCommand getminionIdCmd = new SqlCommand(addedMinionId, sqlConnection, sqlTransaction);
            getminionIdCmd.Parameters.AddWithValue("@minionName", minionName);
            getminionIdCmd.Parameters.AddWithValue("@minionAge", minionAge);
            getminionIdCmd.Parameters.AddWithValue("@TownId", townId);

            int minionId = (int)getminionIdCmd.ExecuteScalar();

            return minionId;
        }

        private static int GetVillainId(SqlConnection sqlConnection, string villainName, StringBuilder sb, SqlTransaction sqlTransaction)
        {
            string villainIdQuery = @"SELECT Id 
                                            FROM Villains 
                                           WHERE Name = @VillainName";

            SqlCommand villainIdCmd = new SqlCommand(villainIdQuery, sqlConnection, sqlTransaction);
            villainIdCmd.Parameters.AddWithValue("@VillainName", villainName);

            object villainId = villainIdCmd.ExecuteScalar();
            if (villainId == null)
            {
                string evilnessFactorQuery = @"SELECT [Id]
                                                 FROM [EvilnessFactors]
                                                WHERE [Name] = 'Evil'";

                SqlCommand evilnessFactoryCmd = new SqlCommand(evilnessFactorQuery, sqlConnection, sqlTransaction);
                int evilnessFactoryId = (int)evilnessFactoryCmd.ExecuteScalar();

                string addVillainQuery = @"INSERT INTO Villains (Name, EvilnessFactorId)  
                                               VALUES (@villainName, @evilnessFactoryId)";

                SqlCommand addVillainCmd = new SqlCommand(addVillainQuery, sqlConnection, sqlTransaction);
                addVillainCmd.Parameters.AddWithValue("@villainName", villainName);
                addVillainCmd.Parameters.AddWithValue("@evilnessFactoryId", evilnessFactoryId);

                addVillainCmd.ExecuteNonQuery();
                sb.AppendLine($"Villain {villainName} was added to the database.");

                villainId = villainIdCmd.ExecuteScalar();
            }

            return (int)villainId;
        }

        private static int GetTownId(SqlConnection sqlConnection, StringBuilder sb, string minionTown, SqlTransaction sqlTransaction)
        {
            string townIdQuery = @"  SELECT Id 
                                       FROM Towns 
                                      WHERE Name = @townName";

            SqlCommand townIdCommand = new SqlCommand(townIdQuery, sqlConnection, sqlTransaction);
            townIdCommand.Parameters.AddWithValue("@townName", minionTown);

            object townIdObj = townIdCommand.ExecuteScalar();
            if (townIdObj == null)
            {
                string addTownQuery = @"INSERT INTO Towns (Name) 
                                            VALUES (@townName)";

                SqlCommand addTownCommand = new SqlCommand(addTownQuery, sqlConnection, sqlTransaction);
                addTownCommand.Parameters.AddWithValue("@townName", minionTown);

                addTownCommand.ExecuteNonQuery();
                sb.AppendLine($"Town {minionTown} was added to the database.");

                townIdObj = townIdCommand.ExecuteScalar();
            }

            return (int)townIdObj;
        }
    }
}
