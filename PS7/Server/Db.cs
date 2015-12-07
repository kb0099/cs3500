using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AgCubio
{
    /// <summary>
    /// Represents a way to communicate to a database.
    /// </summary>
    public static class Db
    {
        private const string connectionString = "server=atr.eng.utah.edu;database=cs3500_bastakot;uid=cs3500_bastakot;password=ps9c#jim";

        /// <summary>
        /// Gets data from database.
        /// </summary>
        /// <returns>Formats the data and returns as HTML Table.</returns>
        public static string GetScoresTable()
        {
            StringBuilder sb = new StringBuilder();
            // Template adapted from the lab/class resources.
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();                                                    // 1. Open connection.
                    MySqlCommand command = conn.CreateCommand();                    // 2. Create a command
                    command.CommandText = @"SELECT P.Name, S.EndedAt-S.StartedAt as TimeAlive, S.HighestMass, S.HighestRank, S.FoodsEaten, S.CubesEaten, S.EndedAt
                                            FROM Session as S                                        
                                            INNER JOIN Player as P
                                            ON S.PlayerID = P.ID;
                                          ";    // 3. Set CommandText

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader session = command.ExecuteReader())
                    {
                        sb.AppendLine("<table>");
                        sb.AppendLine("<th>");
                        sb.AppendLine("<td>Name</td>");
                        sb.AppendLine("<td>TimeAlive</td>");
                        sb.AppendLine("<td>HighestMass</td>");
                        sb.AppendLine("<td>HighestRank</td>");
                        sb.AppendLine("<td>FoodsEaten</td>");
                        sb.AppendLine("<td>EatenAt</td>");
                        sb.AppendLine("</th>");
                        while (session.Read())
                        {
                            sb.AppendLine("<tr>");
                            sb.AppendLine($"<td>{session["Name"]}</td>");
                            sb.AppendLine($"<td>{session["TimeAlive"]}</td>");
                            sb.AppendLine($"<td>{session["HighestMass"]}</td>");
                            sb.AppendLine($"<td>{session["HighestRank"]}</td>");
                            sb.AppendLine($"<td>{session["FoodsEaten"]}</td>");
                            sb.AppendLine($"<td>{session["EndedAt"]}</td>");
                            sb.AppendLine($"</tr>");
                            //Console.WriteLine(sessions["ID"] + " " + sessions["Name"]);
                        }
                        sb.AppendLine("</table>");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return $"<h3>e.Message<br/>Unable to get data from server.</h3>";
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// Gets data from database.
        /// </summary>
        /// <returns>Formats the data and returns as HTML Table.</returns>
        public static string GetGamesTable(string playerName)
        {
            return null;
        }


        /// <summary>
        /// Gets data from database.
        /// </summary>
        /// <returns>Formats the data and returns as HTML Table.</returns>
        public static string GetHighScoresTable(string PlayerName)
        {
            return null;
        }
        public static void test()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "SELECT ID, Name from People";

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader["ID"] + " " + reader["Name"]);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

    }

}
