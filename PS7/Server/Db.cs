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
                        sb.Append(GenerateHTMLTable(session,
                            new String[] { "Name", "TimeAlive", "HighestMass", "HighestRank", "FoodsEaten", "CubesEaten", "EndedAt" },
                            "Name",
                            "/games?player=",
                            "Name"                            
                            ));
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
        /// Helper to create HTML Table.
        /// </summary>
        /// <param name="mysqldr">Data Reader.</param>
        /// <param name="colNames">Column Names to show in table.</param>
        /// <param name="linkerCol">A column field that has anchor tag.</param>
        /// <param name="query">Query part of URI including upto the =. (example: "/pathname/index.htm?=")</param>
        /// <param name="linkedCol">This is used to extract value and join to <paramref name="query"/>.</param>
        /// <returns></returns>
        private static String GenerateHTMLTable(MySqlDataReader mysqldr, string[] colNames,
            string linkerCol = null, string query = null, string linkedCol = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table>");
            sb.AppendLine("<tr>");
            foreach (string col in colNames)
            {
                sb.AppendLine($"<th>{col}</th>");
            }
            sb.AppendLine("</tr>");
            while (mysqldr.Read())
            {
                sb.AppendLine("<tr>");

                foreach (string col in colNames)
                {
                    if (col == linkerCol)
                        sb.AppendLine($"<td><a href='{query}{mysqldr[linkedCol]}'>{mysqldr["Name"]}</a></td>");
                    else
                        sb.AppendLine($"<td>{mysqldr[col]}</td>");
                }
            }
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        /// <summary>
        /// Gets all games by a particular player for: /games?player=name
        /// </summary>
        /// <returns>Formats the data and returns as HTML Table.</returns>
        public static string GetGamesTable(string playerName)
        {
            StringBuilder sb = new StringBuilder();
            // Template adapted from the lab/class resources.
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();                                                    // 1. Open connection.
                    MySqlCommand command = conn.CreateCommand();                    // 2. Create a command
                    command.CommandText = $@"SELECT S.ID as SessionID, P.Name, S.EndedAt-S.StartedAt as TimeAlive, S.HighestMass, S.HighestRank, S.FoodsEaten, S.CubesEaten, S.EndedAt
                                            FROM Session as S                                        
                                            INNER JOIN Player as P
                                            ON S.PlayerID = P.ID
                                            WHERE P.Name = '{playerName}'";                  // 3. Set Command Text

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader session = command.ExecuteReader())
                    {
                        sb.Append(GenerateHTMLTable(session,
                            new String[] { "SessionID", "Name",  "TimeAlive", "HighestMass", "HighestRank", "FoodsEaten", "CubesEaten", "EndedAt" },
                            "CubesEaten",
                            "/eaten?id=?",
                            "SessionID"
                            ));
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
        /// Returns table with list of all eaten cubes for a particular game session <paramref name="sid"/>.
        /// </summary>
        /// <param name="sid">The session id.</param>
        /// <returns></returns>
        public static string GetEatensTable(string sid)
        {
            StringBuilder sb = new StringBuilder();
            // Template adapted from the lab/class resources.
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();                                                    // 1. Open connection.
                    MySqlCommand command = conn.CreateCommand();                    // 2. Create a command
                    command.CommandText = $@"SELECT S.ID as SessionID, P.Name, S.EndedAt-S.StartedAt as TimeAlive, S.HighestMass, S.HighestRank, S.FoodsEaten, S.CubesEaten, S.EndedAt
                                            FROM Session as S                                        
                                            INNER JOIN Player as P
                                            ON S.PlayerID = P.ID
                                            INNER JOIN Eaten as E
                                            ON E.EaterSID = '{sid}'
                                            WHERE S.ID = '{sid}' AND ";                  // 3. Set Command Text

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader session = command.ExecuteReader())
                    {
                        sb.Append(GenerateHTMLTable(session,
                            new String[] { "SessionID", "Name", "TimeAlive", "HighestMass", "HighestRank", "FoodsEaten", "CubesEaten", "EndedAt" },
                            "Name",
                            "/highscores?player=",
                            "Name"
                            ));
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
        /// Gets data from database for: /highscores?player=name
        /// </summary>
        /// <returns>Formats the data and returns as HTML Table.</returns>
        public static string GetHighScoresTable(string sid)
        {
            StringBuilder sb = new StringBuilder();
            // Template adapted from the lab/class resources.
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();                                                    // 1. Open connection.
                    MySqlCommand command = conn.CreateCommand();                    // 2. Create a command
                    command.CommandText = @"SELECT S.ID as SessionID, S.EndedAt-S.StartedAt as TimeAlive, S.HighestMass, S.HighestRank, S.FoodsEaten, S.CubesEaten, S.EndedAt
                                            FROM Session as S                                        
                                            INNER JOIN Player as P
                                            ON S.PlayerID = P.ID";                  // 3. Set Command Text

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader session = command.ExecuteReader())
                    {
                        sb.Append(GenerateHTMLTable(session,
                            new String[] { "SessionID", "Name", "TimeAlive", "HighestMass", "HighestRank", "FoodsEaten", "CubesEaten", "EndedAt" }
                            ));
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

    }

}
