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
            	var GroupQuery = (from x in keysRead
                            select  x.AdGroup)
							.Distinct()
							.OrderBy(x => x);
				
				
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
					Console.WriteLine(kjs.ToString());
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
