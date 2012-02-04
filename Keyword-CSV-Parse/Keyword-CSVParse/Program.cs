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
			args = new string[2];
			args[0] = "biz.csv";
			args[1] = "out.js";
			
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

                    // predicate for setting the propper quote pattern
					Boolean groupFirst = false;
					
					// allocate a string to hold the ADGroup Title
                    string group = string.Empty;
					
					// Notify the user that we are processing x entries
					Console.WriteLine("Processing " + query.Count() + " records....");
                    for (int i = 0; i < query.Count(); i++)
                    {
                        // assign and sanitize the keyword string
                        string word = query.ElementAt(i).Name.ToString();
                        word = word.Replace("{ Name =", "");
                        word = word.Replace("}", "");
                        word = word.Replace("[", "");
                        word = word.Replace("]", "");
                        
                        // Detect if we need to be part of a sub group
                        if (group != query.ElementAt(i).AdGroup.ToString())
                        {
							group = query.ElementAt(i).AdGroup.ToString();
							if (i != 0) 
							{
								output.WriteLine("]} \n\n{ \"" + group + "\" : [");
								groupFirst = true;
							} 
							else
							{
								output.WriteLine("{ \"" + group + "\" : [");	
								groupFirst = true;
							}
                            
                            
                        };
                        
                        // add the item to the group
						if (word != string.Empty)
						{
							if (groupFirst)
							{
								output.Write("\"" + word + "\"");
								groupFirst = false;
							}
							else
							{
                        		output.Write(", \"" + word + "\"");
							}
						}

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


			Console.WriteLine("Proessing Complete");
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
