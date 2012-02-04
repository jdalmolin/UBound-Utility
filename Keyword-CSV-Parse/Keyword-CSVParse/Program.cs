using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LINQtoCSV;
using System.IO;

namespace Keyword_CSVParse
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length > 1)
            {

                if (!File.Exists(args[0]))
                { Console.WriteLine("Cannot locate the file " + args[0]);  return; }
                
                //Cache the argument as a string type
                string filename = args[0].ToString();
                
                // We will assume for now that they will always pass in a , separated list. not a | or whatever else.
                CsvFileDescription inputFileDescript = new CsvFileDescription
                {
                    SeparatorChar = ',',
                    FirstLineHasColumnNames = true // yep and we want ColumnNames too
                };

                CsvContext cc = new CsvContext();

                // Load the CSV file into a query'able list
                IEnumerable<Keywords> keysRead = cc.Read<Keywords>(filename, inputFileDescript);
                
                // order the data how we want it
                var query = from x in keysRead
                            orderby x.AdGroup
                            select new { x.Name, x.AdGroup };

                // if we got any results - lets do this thang
                if (query.Any())
                {
                    // be unsafe and CLOBBER the file
                    var output = File.CreateText(args[1]);

                    

                    string group = string.Empty;
                    foreach (var keyword in query)
                    {
                        // assign and sanitize the keyword string
                        string word = keyword.Name.ToString();
                        word = word.Replace("{ Name =", "");
                        word = word.Replace("}", "");
                        word = word.Replace("[", "");
                        word = word.Replace("]", "");
                        
                        // Detect if we need to be part of a sub group
                        if (group != keyword.AdGroup.ToString())
                        {
                            group = keyword.AdGroup.ToString();
                            output.WriteLine("]} \n\n{ \"" + group + "\" : [");
                        };
                        
                        // add the item to the group
                        output.WriteLine("\"" + word + "\",");

                        //Console.WriteLine("{\"" + word + "\" : \"" +  group + "\"}"); -- cool testing worked
                    }
                }
                else
                {
                    Console.WriteLine("No results found while parsing CSV file");
                    return;
                }

            }
            else
            {
                Console.WriteLine("No file exists in the current path.\n Usage: Keyword_CSVParse.exe input_filename output_filename");
                return;
            }



        }
    }


    // dirty hack for convenience

    public class Keywords
    {
        [CsvColumn(Name = "Keyword", FieldIndex = 1)]
        public string Name { get; set; }

        [CsvColumn(Name = "Ad Group", FieldIndex = 2)]
        public string AdGroup { get; set; }
    }



}
