using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PS4Client1
{
    class Program
    {
        static void Main(string[] args)
        {
            XElement spreadsheet = new XElement("spreadsheet", new XAttribute("version", "test"),
                new XElement("cell",  new XElement("name", "a1"), new XElement("contents", "9")),
                new XElement("cell", new XElement("name", "b1"), new XElement("contents", "10")),
                new XElement("cell", new XElement("name", "c1"), new XElement("contents", "=a1+b1"))
                );
            spreadsheet.Save("kb01.xml");
            Console.WriteLine("done!");
        }
    }
}
