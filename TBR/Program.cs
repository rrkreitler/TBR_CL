using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace TBR
{
    // This is a command line version of the TweetBrowser
    // found in GitHub repo https://github.com/rrkreitler/TBR
    // It reads a web API that has built-in limitations that restricts
    // its response to any query to no more than 100 records. Even
    // if there are more than 100 records that should be returned.
    // The remote host will not give any indication that records were 
    // dropped. This app will retrieve data from the API while working 
    // within the 100 record limitations without dropping records.
    class Program
    {
        enum PageKey { Next, Previous, Home, End, Quit, None }

        static void Main(string[] args)
        {
            // Parse and validate command line args.
            ArgValidator argValidator = new ArgValidator();
            bool? validatedArgs = argValidator.Validate(args);
            if (validatedArgs != true)
            {
                // If there are parse errors display syntax help.
                if (validatedArgs == false)
                {
                    ShowHelp();
                }
                else
                {
                    // User input a filename to save the data and the file already exists.
                    // when prompted to overwrite the file, the user said no. 
                    Console.WriteLine("\n\n** Query cancelled.\n");
                }
                Environment.Exit(0);
            }

            // Query the remote site.
            TweetDataClient tdc = new TweetDataClient();
            IEnumerable<Tweet> queryResult = null;
            try
            {
                queryResult = tdc.GetItemsFromUrl(argValidator.Url, argValidator.StartDate, argValidator.EndDate,
                    argValidator.Verbose);
            }
            catch
            {
                Console.WriteLine("\n**Query Error - Cannot query the remote host\n");
            }

            int recsSaved = 0;

            if (queryResult != null && queryResult.Count() > 0 && !argValidator.NoList)
            {
                ShowPaginatedList(queryResult,argValidator);
                if (argValidator.Filename != String.Empty)
                {
                    recsSaved = SaveToFile(argValidator.Filename, queryResult);
                }
            }

            // Show results summary.
            if (argValidator.PageSize == 0 || !string.IsNullOrEmpty(argValidator.Filename) || argValidator.NoList)
            {
                int recsFound = queryResult?.Count() ?? 0;
                Console.WriteLine("\n============================================================");
                Console.WriteLine($"Query from: {argValidator.StartDate} to {argValidator.EndDate}");
                Console.WriteLine($"Records Found: {recsFound}");
                if (!string.IsNullOrEmpty(argValidator.Filename))
                {
                    Console.WriteLine($"Records Saved: {recsSaved}");
                }
                Console.WriteLine("============================================================");
            }
        }

        // Serializes tweet data to a json file.
        private static int SaveToFile(string filePath, IEnumerable<Tweet> queryResult)
        {
            try
            {
                // Remove the file if it already exists.
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                string fileJsonData = JsonConvert.SerializeObject(queryResult);
                File.AppendAllText(filePath, fileJsonData);
                return queryResult.Count();
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n**File Error - Cannot write to file {filePath}\n");
                Console.WriteLine($"details: {e.Message}\n");
                return 0;
            }
        }
        
        private static void ShowPaginatedList(IEnumerable<Tweet> queryResult, ArgValidator argValidator)
        {
            List<Tweet> results = queryResult.ToList();
            // Set start index for page.
            int startIndex = -1;
            int endIndex = 0;

            // Show page
            PageKey key = PageKey.Home;
            do
            {
                string keyNext = " next - >";
                string keyPrvious = "< - previous ";
                bool updateScreen = false;
                switch (key)
                {
                    // Jump to end.
                    case PageKey.End:
                        if (endIndex != results.Count)
                        {
                            endIndex = results.Count;
                            startIndex = endIndex - argValidator.PageSize;
                            if (startIndex < 0)
                            {
                                startIndex = 0;
                            }
                            keyNext = "";
                            updateScreen = true;
                        }
                        break;
                    // Next page.
                    case PageKey.Next:
                        if (endIndex != results.Count)
                        {
                            if (startIndex + argValidator.PageSize < results.Count)
                            {
                                startIndex += argValidator.PageSize;
                                endIndex += argValidator.PageSize;
                                if (endIndex > results.Count)
                                {
                                    keyNext = "";
                                }
                                updateScreen = true;
                            }
                        }
                        break;
                    // Jump to beginning.
                    case PageKey.Home:
                        if (startIndex != 0)
                        {
                            Console.Clear();
                            startIndex = 0;
                            endIndex = startIndex +
                                       ((argValidator.PageSize == 0) ? results.Count : argValidator.PageSize);
                            updateScreen = true;
                            keyPrvious = "";
                        }

                        break;
                    // Previou page.
                    case PageKey.Previous:
                        if (startIndex != 0)
                        {
                            startIndex -= argValidator.PageSize;
                            if (startIndex <= 0)
                            {
                                startIndex = 0;
                                keyPrvious = "";
                            }
                            endIndex = startIndex + argValidator.PageSize;
                            updateScreen = true;
                        }
                        break;
                    // Show key help on invalid keystrokes.
                    default:
                        Console.WriteLine("*** Valid keys:");
                        Console.WriteLine("N-next P-previous HOME-begin END-end Q-quit\n");
                        updateScreen = false;
                        break;
                }

                for (int i = startIndex; i < endIndex && i < results.Count() && updateScreen; i++)
                {
                    Console.WriteLine($"{results[i].Id}   {results[i].Stamp}\n{results[i].Text}");
                    Console.WriteLine("------------------------------------------------------------");
                }

                // Display page number information
                if (argValidator.PageSize != 0)
                {
                    if (updateScreen)
                    {
                        Console.WriteLine($"\nQuery from: {argValidator.StartDate} to {argValidator.EndDate}");
                        int endNum = (endIndex > results.Count) ? results.Count : endIndex;
                        Console.WriteLine(
                            $"Records {startIndex + 1} - {endNum} of {results.Count}   {keyPrvious}{keyNext}\n");
                    }
                    key = CheckKey(Console.ReadKey());
                }
                else
                {
                    key = PageKey.Quit;
                }
            } while (key != PageKey.Quit);
        }
        
        // Processes key strokes during pagination.
        private static PageKey CheckKey(ConsoleKeyInfo keyInfo)
        {
            Console.Write("\b \b");
            switch (keyInfo.Key)
            {
                // Valid "Next Page" keys
                case ConsoleKey.Spacebar:
                    return PageKey.Next;
                case ConsoleKey.Enter:
                    return PageKey.Next;
                case ConsoleKey.RightArrow:
                    return PageKey.Next;
                case ConsoleKey.DownArrow:
                    return PageKey.Next;
                case ConsoleKey.PageDown:
                    return PageKey.Next;
                case ConsoleKey.OemPeriod:
                    return PageKey.Next;
                case ConsoleKey.N:
                    return PageKey.Next;
                case ConsoleKey.End:
                    return PageKey.End;
                // Valid "Previous Page" keys
                case ConsoleKey.LeftArrow:
                    return PageKey.Previous;
                case ConsoleKey.OemComma:
                    return PageKey.Previous;
                case ConsoleKey.B:
                    return PageKey.Previous;
                case ConsoleKey.P:
                    return PageKey.Previous;
                case ConsoleKey.UpArrow:
                    return PageKey.Previous;
                case ConsoleKey.PageUp:
                    return PageKey.Previous;
                case ConsoleKey.Home:
                    return PageKey.Home;
                // Exit keys
                case ConsoleKey.X:
                    return PageKey.Quit;
                case ConsoleKey.Q:
                    return PageKey.Quit;
                default:
                    break;
            }
            // On invalid key
            return PageKey.None;
        }

        static void ShowHelp()
        {
            Console.WriteLine("Lists and downloads messages from a remote archive.\n");
            Console.WriteLine("TBR url startdate [/ST starttime] enddate [/ET endtime]");
            Console.WriteLine(" [/F [drive:][path]filename] [/NL] [/P pages] [/V]\n");
            Console.WriteLine("url         Url of the message archive.");
            Console.WriteLine("startdate   Start date for the range of messages in the archive.");
            Console.WriteLine("/ST         Start time for the range of messages in the archive.");
            Console.WriteLine("starttime   Timestamp value in the form \"4:35 AM\"");
            Console.WriteLine("enddate     End date for the range of messages in the archive.");
            Console.WriteLine("/ET         End time for the range of messages in the archive.");
            Console.WriteLine("endtime     Timestamp value in the form \"4:35 AM\"");
            Console.WriteLine("/F          Downloads and saves messages.");
            Console.WriteLine("[drive:][path]filename");
            Console.WriteLine("            Specifies drive, directory, and name of file where");
            Console.WriteLine("            downloaded messages will be saved.");
            Console.WriteLine("/NL         No List. Messages will not be displayed. Only the summary");
            Console.WriteLine("            will be shown. NOTE: This overrides /P");
            Console.WriteLine("/P          Sets page size.");
            Console.WriteLine("pages       Sets the number of messages to be displayed on each page.");
            Console.WriteLine("/V          Verbose mode shows remote host query details.");
        }
    }
}
