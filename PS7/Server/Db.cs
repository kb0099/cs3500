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
        /// Adds a new Game to the server with current time set automatically as StartedAt.
        /// Returns the Game.ID on success else, returns -1.
        /// </summary>
        public static int AddGame()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = @"
                                        INSERT INTO Game(StartedAt)
                                        VALUES
                                        (
                                        unix_timestamp()
                                        );
                                        ";
                    conn.Open();
                    int numRows = cmd.ExecuteNonQuery();
                    if (numRows == 1)
                    {
                        cmd.CommandText = @"SELECT LAST_INSERT_ID();";
                        return int.Parse(cmd.ExecuteScalar().ToString());
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return -1;
        }

        /// <summary>
        /// Adds a new Session entry for the given playerName.
        /// If the player doesn't exist an entry exist with the playerName.
        /// Player Name is treated as unique so no 2 player's can be same name.
        /// Means that two same names map to a single player.
        /// </summary>
        /// <param name="gameID">Represents a server Game.ID</param>
        /// <param name="playerName">The player name.</param>
        /// <returns>New Session ID or -1.</returns>
        public static int AddSession(int gameID, string playerName, int startMass)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    MySqlCommand cmd = conn.CreateCommand();

                    // First get the player ID
                    cmd.CommandText = $@"select ID from Player where Player.Name = '{playerName}'";
                    conn.Open();
                    var playerID = cmd.ExecuteScalar();

                    // if player doesn't exist create one
                    if (playerID == null)
                    {
                        cmd.CommandText = $@"INSERT INTO Player
                                            (Name)
                                            VALUES
                                            ('{playerName}');";
                        int numRows = cmd.ExecuteNonQuery();
                        if (numRows == 1)
                        {
                            cmd.CommandText = @"SELECT LAST_INSERT_ID();";
                            playerID = int.Parse(cmd.ExecuteScalar().ToString());
                        }
                    }

                    // now create the Session entry
                    cmd.CommandText = $@"INSERT INTO Session
                                        (GameID, StartedAt, PlayerID, HighestMass)
                                        VALUES
                                        ({gameID}, unix_timestamp(), {playerID}, {startMass});
                                        ";
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        cmd.CommandText = @"SELECT LAST_INSERT_ID();";
                        return int.Parse(cmd.ExecuteScalar().ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return -1;
        }

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
                            "Name",
                            caption: $"List of All Game Plays and Scores"
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
        /// Updates the Session using the provided inputs.
        /// </summary>
        /// <param name="sid">The Session.ID</param>
        /// <param name="fields">Key-Value pair of column names to field values.</param>
        /// <param name="end">Indicates whether to end the session.</param>
        public static bool UpdateSession(int sid, Dictionary<string, string> fields, bool end = false)
        {
            if (fields.Count < 1 && end == false) return true;      // Nothing to update!
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    MySqlCommand cmd = conn.CreateCommand();

                    // Build the command
                    StringBuilder sb = new StringBuilder();
                    sb.Append($@"UPDATE Session SET ");

                    var itr = fields.GetEnumerator();
                    if (end)
                    {
                        sb.Append("EndedAt = unix_timestamp()");
                    }
                    else if (itr.MoveNext())        // this is to fix the extra ", " 
                    {
                        sb.Append($"{itr.Current.Key} = {itr.Current.Value}");
                    }

                    while (itr.MoveNext())
                    {
                        sb.Append($", {itr.Current.Key} = {itr.Current.Value}");
                    }

                    sb.Append($" WHERE ID = {sid}");

                    // set commandText
                    cmd.CommandText = sb.ToString();

                    // open connection and execute
                    conn.Open();
                    return cmd.ExecuteNonQuery() == 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return false;
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
                            new String[] { "SessionID", "Name", "TimeAlive", "HighestMass", "HighestRank", "FoodsEaten", "CubesEaten", "EndedAt" },
                            "CubesEaten",
                            "/eaten?id=",
                            "SessionID",
                            caption: $"List of All Games by Player: {playerName}"
                            ));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return $"<h3>{e.Message}<br/>Unable to get data from server.</h3>";
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
                                            WHERE S.ID IN (SELECT EatenSID FROM Eaten Where EaterSID = '{sid}')"; // 3. Set Command Text

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader session = command.ExecuteReader())
                    {
                        sb.Append(GenerateHTMLTable(session,
                            new String[] { "SessionID", "Name", "TimeAlive", "HighestMass", "HighestRank", "FoodsEaten", "CubesEaten", "EndedAt" },
                            "Name",
                            "/highscores?player=",
                            "Name",
                            caption: $"List of Eaten Cubes in Game Session: {sid}"
                            ));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return $"<h3>{e.Message}<br/>Unable to get data from server.</h3>";
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets data from database for: /highscores?player=name
        /// </summary>
        /// <returns>Formats the data and returns as HTML Table.</returns>
        public static string GetHighScoresTable(string name)
        {
            StringBuilder sb = new StringBuilder();
            // Template adapted from the lab/class resources.
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();                                                    // 1. Open connection.
                    MySqlCommand command = conn.CreateCommand();                    // 2. Create a command
                    command.CommandText = $@"SELECT S.ID as SessionID, S.EndedAt-S.StartedAt as TimeAlive, S.HighestMass, S.HighestRank, S.FoodsEaten, S.CubesEaten, S.EndedAt
                                            FROM Session as S                                        
                                            INNER JOIN Player as P
                                            ON P.ID = S.PlayerID
                                            WHERE S.PlayerID = (SELECT ID FROM Player WHERE Name = '{name}');";                  // 3. Set Command Text

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader session = command.ExecuteReader())
                    {
                        sb.Append(GenerateHTMLTable(session,
                            new String[] { "SessionID", "TimeAlive", "HighestMass", "HighestRank" }, caption: $"High Scores for Player: {name}"
                            ));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return $"<h3>{e.Message}<br/>Unable to get data from server.</h3>";
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
            string linkerCol = null, string query = null, string linkedCol = null, string caption = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table>");
            if (caption != null)
            {
                sb.AppendLine($"<caption>{caption}</caption>");
            }
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
                        sb.AppendLine($"<td><a href='{query}{mysqldr[linkedCol]}'>{mysqldr[col]}</a></td>");
                    else
                        sb.AppendLine($"<td>{mysqldr[col]}</td>");
                }
            }
            sb.AppendLine("</table>");
            return sb.ToString();
        }

    }

}
