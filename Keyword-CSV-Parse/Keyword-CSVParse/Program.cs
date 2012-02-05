using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LINQtoCSV;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
				
                // We will assume for now that they will always pass in a , separated list. not a | or whatever else.
                CsvFileDescription inputFileDescript = new CsvFileDescription
                {
                    SeparatorChar = ',',
                    FirstLineHasColumnNames = true // yep and we want ColumnNames too
                };

                CsvContext cc = new CsvContext();

                // Load the CSV file into a query'able list
                IEnumerable<Keywords> keysRead = cc.Read<Keywords>(args[0], inputFileDescript);
                
                // order the data how we want it, nix the dupes
            	var GroupQuery = (from x in keysRead
                            select  x.AdGroup)
							.Distinct()
							.OrderBy(x => x);
				
				//Create a file object to store the output in
				var output = File.CreateText(args[1]);
				
				
				// Iterate through the ordered query - using the groups we scraped
				// and build an object for each group - then write it to the file
				// in proper JSON format
				foreach(var group in GroupQuery)
				{
					JObject kjs =
						new JObject(
							new JProperty(@group,
							new JArray(
							from x in keysRead
								where x.AdGroup == @group
								select new JValue(x.Name)
							)));
					// Save the file.
					output.WriteLine(kjs.ToString());
				}
				
				// close the file
				output.Close();
				
            }
            else
            {
                Console.WriteLine("No file exists in the current path.\n Usage: Keyword_CSVParse.exe input_filename output_filename");
                return;
            }


			Console.WriteLine("Done!");
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
