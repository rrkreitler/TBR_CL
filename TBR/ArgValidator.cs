using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TBR
{
    // Parses and validates the command line arguments. Will output an error to
    // to the console and return false is parsing fails.
    // Note return is nullable bool. If the return value is null it means parsing 
    // was successful but the user answered "No" to the file overwrite prompt.
    public class ArgValidator
    {
        public string Url { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Verbose { get; set; }
        public string Filename { get; set; }
        public int PageSize { get; set; }

        public bool? Validate(string[] args)
        {
            // Show Help
            try
            {
                if (args.Length < 3 || !string.IsNullOrEmpty(args.FirstOrDefault(a => a.ToUpper() == "/?")))
                {
                    Console.WriteLine("**Syntax Error - invalid command");
                    return false;
                }
            }
            catch (Exception e)
            {
                string t = e.Message;
            }

            // Put array in a stack for parsing.
            Stack<string> argStack = new Stack<string>();
            for (int i = args.Length - 1; i >= 0; i--)
            {
                argStack.Push(args[i]);
            }

            // Set default arg values.
            string startDate = "Error";
            string endDate = "Error";
            string startTime = "12:00 AM";
            string endTime = "12:00 AM";
            string pages = "0";
            Verbose = false;
            Filename = string.Empty;

            // Parse arguments on slashes or dashes
            int paramIndex = 0;
            string arg = String.Empty;

            try
            {
                while (argStack.Count > 0)
                {
                    arg = argStack.Pop();
                    switch (arg.ToUpper())
                    {
                        case "/ST":
                            startTime = argStack.Pop();
                            break;
                        case "/ET":
                            endTime = argStack.Pop();
                            break;
                        case "/F":
                            Filename = argStack.Pop();
                            break;
                        case "/P":
                            pages = argStack.Pop();
                            break;
                        case "/V":
                            Verbose = true;
                            break;
                        case "-ST":
                            startTime = argStack.Pop();
                            break;
                        case "-ET":
                            endTime = argStack.Pop();
                            break;
                        case "-F":
                            Filename = argStack.Pop();
                            break;
                        case "-P":
                            pages = argStack.Pop();
                            break;
                        case "-V":
                            Verbose = true;
                            break;
                        default:
                            // Assumes any args that do not have a slash/dash identifier
                            // are (in order) Url, startDate, and endDate. Any other
                            // string is unknown and will result in an error.
                            if (paramIndex == 0)
                            {
                                Url = arg;
                            }
                            else if (paramIndex == 1)
                            {
                                startDate = arg;
                            }
                            else if (paramIndex == 2)
                            {
                                endDate = arg;
                            }
                            else
                            {
                                Console.WriteLine("**Syntax Error - Unknown value");
                                return false;
                            }

                            paramIndex++;
                            break;
                    }
                }
            }
            catch
            {
                Console.WriteLine($"**Syntax Error - Bad argument value for {arg}");
                return false;
            }

            // URL
            try
            {
                UriBuilder uriBuilder = new UriBuilder(Url);
            }
            catch
            {
                Console.WriteLine("**Syntax Error - Invalid URL");
                return false;
            }

            // Start/EndDates
            try
            {
                StartDate = Convert.ToDateTime(startDate + " " + startTime);
                EndDate = Convert.ToDateTime(endDate + " " + endTime);
            }
            catch
            {
                Console.WriteLine("**Syntax Error - Invalid start/end date or time");
                return false;
            }

            // Pages
            try
            {
                PageSize = Convert.ToInt32(pages);
                if (PageSize < 0)
                {
                    throw new Exception();
                }
            }
            catch
            {
                Console.WriteLine("**Syntax Error - Pages must be a valid positive number");
                return false;
            }

            // Filename
            try
            {
                if (File.Exists(Filename))
                {
                    Console.WriteLine($"The file {Filename} already exists.");
                    Console.Write($"Do you want to overwrite it? (Y/N): ");
                    if (Console.ReadKey().Key != ConsoleKey.Y)
                    {
                        return null;
                    }
                }
            }
            catch
            {
                Console.WriteLine("**Syntax Error - Invalid filename");
                return false;
            }

            return true;
        }

    }
}
