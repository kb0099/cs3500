using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AgCubio
{
    public partial class Server
    {
        private const string ResponseHeader = "HTTP/1.1 200 OK\r\nConnection: close\r\nContent-Type: text/html; charset=UTF-8\r\n\r\n";
        private const string HtmlHeader = @"<!doctype html><head><title>AgCubio | Game Play Results</title></head>
            <style>
                body{ max-width: 800px;background: darkslategrey;}
                table, th, td { border: 1px solid black; padding: .2em;}
                *{color: cornsilk; text-align: center;  margin: auto; }
                caption {padding: .5em; font-size:  1.2em; font-weight: bold;}
            </style>";
        private static void StartWebServer()
        {
            Network.ServerAwaitingClientLoop(NewHttpRequest, 11100, true);
        }

        private static void NewHttpRequest(PreservedState ps)
        {
            ps.callback = (s) =>
            {
                StringBuilder sb = new StringBuilder(ResponseHeader);
                sb.Append(HandleRequest(ps.receivedData.ToString())); 
                Network.Send(ps.socket, sb.ToString(), true);
            };
            Network.WantMoreData(ps);
        }

        /// <summary>
        /// Parses the header and returns the output response.
        /// </summary>
        /// <param name="requestHeader">The header.</param>
        /// <returns>The output response.</returns>
        private static String HandleRequest(String requestHeader)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(HtmlHeader);
            sb.Append($"<body><div style='padding: 1em'><h1>AgCubio Game Play Info</h1></div><div><hr/></div>");

            // format: /request[?param=value]
            String pattern = @"GET /(?<request>\w*?)(\?(?<param>.+?)=(?<value>.+?))* HTTP/1.1";
            Regex rx = new Regex(pattern);
            Match m = rx.Match(requestHeader);
            
            string table = null;
            switch (m.Groups["request"].Value)
            {
                case "scores":
                    table = Db.GetScoresTable();
                    break;
                case "games":
                    if(m.Groups["param"].Value == "player")
                        table = Db.GetGamesTable(m.Groups["value"].Value);
                    break;
                case "eaten":
                    if(m.Groups["param"].Value == "id")
                        table = Db.GetEatensTable(m.Groups["value"].Value);
                    break;
                case "highscores":
                    if (m.Groups["param"].Value == "player")
                        table = Db.GetHighScoresTable(m.Groups["value"].Value);
                    break;
            }
            if (table == null)
                return sb.Append(errorPage).ToString();
            else
                return sb.Append(table).ToString();
        }

        private const string errorPage = @"
<div><style>p, ul, h2, h3 {margin: 2em; } ul, li, p, h3 {text-align: left; }</style><h2>throw new AgCubioException(""Invalid Web Request"")</h2><h3>Here is what it means: The page you requested does not exist. Please check your requested url and query parameters.</h3>
<p>
Here are the list of valid url formats:
<ul>
    <li><a href='/scores'>Scores Table: 'localhost:11100/scores'</a></li>
    <li>All Game Sessions by a Player: 'localhost:11100/games?player=playerName'</li>
    <li>All Cubes Eaten in a Game Session: 'localhost:11100/eaten?id=sessionID'</li>
    <li>List of High Scores by a Player: 'localhost:11100/highscores?player=playerName'</li>
</ul>
</p>
<h2>Possible Solutions for You</h2>
<p>
First, make sure your url is in one of the valid forms provided above.
Second, make sure the playerName or sessionID query parameters that you provide belong to a valid entity or a game session. Also note: all query parameters and player names are case sensitive. Jim and JIM are treated as different names.
</p>
</div>
";
    }
}
