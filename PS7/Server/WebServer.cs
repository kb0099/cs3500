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
        private const string HtmlHeader = @"< !doctype html><head><title>AgCubio | Game Play Results</title></head>
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

            // parse the HTTP header method (GET)
            // if valid send data and close connection
            // else invalid show error page
            //ps.socket.send
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
            sb.Append($"<body><h1>GrandOpening</h1><pre>{requestHeader}</pre>");

            // format: /request[?param=value]
            String pattern = @"GET /(?<request>\w*?)(\?(?<param>.+?)=(?<value>.+?))* HTTP/1.1";
            Regex rx = new Regex(pattern);
            Match m = rx.Match(requestHeader);
            switch (m.Groups["request"].Value)
            {
                case "scores":
                    sb.Append("<h1>You will get default page</h1>");
                    sb.Append(Db.GetScoresTable());
                    return sb.ToString();
                case "games":
                    sb.Append(Db.GetGamesTable(m.Groups["value"].Value));
                    return sb.ToString();
                case "eaten":
                    sb.Append(Db.GetEatensTable(m.Groups["value"].Value));
                    return sb.ToString();
                case "highscores":
                    sb.Append(Db.GetHighScoresTable(m.Groups["value"].Value));
                    return sb.ToString();
            }
            return sb.Append("404 : Error").ToString();
        }
    }
}
