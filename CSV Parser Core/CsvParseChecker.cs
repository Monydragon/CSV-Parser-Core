/* WELCOME TO THE CSV PARSER & CSV Duplicate Column Checker. This program was developed and designed by Mony Dragon.
   This was rapidly put together to showcase some of my programming experience while interpreting the task provided through a job interview assignment.
   Let me explain what the purpose for this application and why I developed it the way I did and why I decided to design and architect it the way I did.
   I designed this around .Net Core I wanted to use .Net Core since it can be used on nearly any platform and truly allows this code to be executed/published to multiple target platforms.
   I programmed this initially to be a console based application since the assignment did not specify if I needed to create a method or a program so I decided on the latter.
   I want to put emphasis that this code is by no means what my production code would look like. I follow Dev/Test/Prod release and development cycles.
   What I could do if provided a position working in a professional capacity. I could use Unit Testing and Perform TDD if needed. Follow proper code standards per company req
   This codebase can be manipulated as needed I will be releasing this Open Source GPL 0. Please look into the licensing terms for Public Domain.
   I hope this is sufficient I quickly put this together, If I was being paid to develop programs/applications/games professionally the quality would be heavily improved.
   Please email me any questions or comments or concerns you have with my code or this project at Monydragon@gmail.com
   Thank you so much for reading this! If you are not seeking to hire Mony Dragon, You can disregard the whole entire comment provided above.
 
   ENJOY!
*/

using System;
using System.Linq;
using Chilkat; //This namespace is just a free nuget package to make handling Loading/Handling CSV's a little easier no need to reinvent the wheel.
               //The same functionaliy could be achieved with the same effect if I wrote a read file parser and specified a delimiter being , and using that.
               //I only used this library since it was free/easy and new for me to try I am always trying new libraries and comparing them for the best fit for each project.


namespace CSV_Parser_Core
{
    /// <summary>
    /// This class represents a Console application that allows the user to input a filepath and column name and through console commands the user can check for duplicates based on the filename and column name.
    /// </summary>
    public class CsvParseChecker
    {
        static void Main(string[] args)
        {
            Csv loadedCsv;

            Console.WriteLine("Welcome to the CSV Parser with duplicate checking. Please feel free to use this to detect duplicates within CSV columns.\n" +
                              "By default this program loads the 2 executable parameters ran on the executable.1: Filepath, 2: Column name to check\n" +
                              "You can also specify within the program the filename and column name manually.\n\n");


            string filepath = "SampleCsv.csv";
            string columnToCheck = "Column1";
            if (args.Length > 0)
            {
                if (!string.IsNullOrEmpty(args[0]))
                {
                    filepath = args[0];
                }
                if (args.Length > 1)
                {
                    if (!string.IsNullOrEmpty(args[1]))
                    {
                        columnToCheck = args[1];
                    }
                }
            }

            bool fileExists = System.IO.File.Exists(filepath);

            StartLoad:
            Console.WriteLine("Loaded CSV Info:\n\n" +
                              $"Filepath: {filepath} (Exists? {fileExists})\n" +
                              $"Column name to check for duplicates: {columnToCheck}\n");

            Console.WriteLine("Do you wish to continue with these settings? (Y/N/Exit)");
            if (InputYesOrNo())
            {
                if (!System.IO.File.Exists(filepath))
                {
                    Console.WriteLine("Invalid filepath please input a valid input path.");
                    filepath = InputValidCsvPath();
                }
                loadedCsv = LoadCsv(filepath);
                if (loadedCsv.GetIndex(columnToCheck) < 0)
                {
                    Console.WriteLine("Invalid Column name please input a valid existing Column name.");
                    columnToCheck = InputValidColumnName(loadedCsv);
                }
            }
            else
            {
                var inputCsv = ManuallyInputCsv();

                loadedCsv = inputCsv.Item1;
                columnToCheck = inputCsv.Item2;
            }

            Console.WriteLine("Would you like to display the csv data on screen? (Y/N/Exit)");
            if (InputYesOrNo()) { PrintCsvText(loadedCsv, true); }
            Console.WriteLine($"Would you like to check for duplicates for column:{columnToCheck}? (Y/N/Exit)");
            if (InputYesOrNo()) { DuplicateCheckByColumn(loadedCsv, columnToCheck); }

            Console.WriteLine("\n\nThank you for using this csv parser / duplicate checker would you like to parse another file? (Y/N/Exit)");

            if (InputYesOrNo())
            {
                goto StartLoad;
            }
            else
            {
                Console.WriteLine("GOOD BYE!");
                Environment.Exit(-1);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// This handles the input checking and validation for Y/N/Exit to handle program handling commands.
        /// </summary>
        /// <returns></returns>
        public static bool InputYesOrNo()
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Environment.Exit(-1);
                    break;
                }

                if (input.Equals("y", StringComparison.OrdinalIgnoreCase)) { return true; }
                if (input.Equals("n", StringComparison.OrdinalIgnoreCase)) { return false; }

                Console.WriteLine("Invalid input please enter a valid command (Y/N/Exit)");

            }

            return false;
        }

        /// <summary>
        /// This is a Tuple that gathers the filepath and Column name input to be used for manual input.
        /// </summary>
        /// <returns></returns>
        public static (Csv, string) ManuallyInputCsv()
        {
           var loadedCsv = LoadCsv(InputValidCsvPath());
           var columnName = InputValidColumnName(loadedCsv);
            return (loadedCsv,columnName);
        }

        /// <summary>
        /// This validates a filepath th make sure that the filepath is valid and will return the string back after it's valid.
        /// </summary>
        /// <returns></returns>
        public static string InputValidCsvPath()
        {
            string filepath = string.Empty;
            Console.WriteLine("Please input a filepath for the CSV file.");

            while (!System.IO.File.Exists(filepath))
            {
                var filepathInput = Console.ReadLine();
                if (System.IO.File.Exists(filepathInput))
                {
                    filepath = filepathInput;
                }
                else
                {
                    Console.WriteLine("Invalid Filepath unable to load CSV file.");
                }
            }

            return filepath;
        }

        /// <summary>
        /// This validates the column name based on the provided Csv and input validation.
        /// </summary>
        /// <param name="csv"></param>
        /// <returns></returns>
        public static string InputValidColumnName(Csv csv)
        {
            string columnCheckName = string.Empty;
            int columnCheckIndex = -1;


            Console.WriteLine("Please input Column name to check.");

            while (columnCheckIndex == -1)
            {
                columnCheckName = Console.ReadLine();
                columnCheckIndex = csv.GetIndex(columnCheckName);
                if (columnCheckIndex < 0)
                {
                    Console.WriteLine($"Unable to find Column called: {columnCheckName} please provide a valid Column name");
                }
            }
            return columnCheckName;
        }

        /// <summary>
        /// Loads the CSV file based on the the provided file path.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Chilkat.Csv data object</returns>
        public static Csv LoadCsv(string filePath, bool hasColumnNames = true)
        {
            Csv csvParser = new Csv();
            csvParser.HasColumnNames = hasColumnNames;
            bool loadedCsv = csvParser.LoadFile(filePath);

            if (loadedCsv)
            {
                return csvParser;
            }
            else
            {
                try
                {
                    throw new Exception("CSV Load Failed");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e + $"\n {csvParser.LastErrorText}");
                    return null;
                }
            }
        }

        /// <summary>
        /// This Unused method allows to print out the raw csv file without special formatting.
        /// </summary>
        /// <param name="csv"></param>
        public static void PrintCsvRaw(Csv csv)
        {
            var text = csv.SaveToString();
            Console.WriteLine(text);
        }

        /// <summary>
        /// This method prints out to the console what the parsed CSV contains and optionally shows the ColumnNames if bool is enabled.
        /// </summary>
        /// <param name="csv"></param>
        /// <param name="showColumnNames"></param>
        public static void PrintCsvText(Csv csv, bool showColumnNames = false)
        {
            string columnNames = String.Empty;
            if (showColumnNames)
            {
                for (int i = 0; i < csv.NumColumns; i++)
                {
                    columnNames += csv.GetColumnName(i) + " ";
                }
            }

            Console.WriteLine(columnNames);

            string displayText = String.Empty;
            for (int r = 0; r < csv.NumRows; r++)
            {
                for (int c = 0; c < csv.NumColumns; c++)
                {
                    if (c >= 1 && c <= csv.NumColumns)
                    {
                        displayText += "\t";
                    }

                    displayText += csv.GetCell(r, c);

                }

                displayText += "\n";
            }

            Console.WriteLine(displayText);
        }

        /// <summary>
        /// This method checks duplicates based on the provided csv and columnName this will print out the duplicates and the amounts of duplicates within the csv.
        /// </summary>
        /// <param name="csv"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool DuplicateCheckByColumn(Csv csv, string columnName)
        {
            
            string[] rowArray = new string[csv.NumRows];
            int columnIndex = csv.GetIndex(columnName);

            for (int r = 0; r < csv.NumRows; r++)
            {
                rowArray[r] = csv.GetCell(r, columnIndex);
            }

            if (rowArray.Distinct().Count() != rowArray.Length)
            {
                var dupList = rowArray.GroupBy(x => x)
                                    .Where(g => g.Count() > 1)
                                    .Select(y => y.Key)
                                    .ToList();

                Console.WriteLine($"Duplicates found: {dupList.Count}");
                foreach (var duplicate in dupList)
                {
                    Console.WriteLine($"{duplicate} ({rowArray.Count(x=> x == duplicate)} Duplicates)");
                }
                return true;
            }
            Console.WriteLine("Duplicates Not Found");
            return false;
        }
    }
}
