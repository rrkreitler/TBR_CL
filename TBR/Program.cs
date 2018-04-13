using System;
using System.Collections.Generic;
using System.Linq;

namespace TBR
{
    class Program
    {
        enum PageKey { Next, Previous, Home, End, Quit, None }

        static void Main(string[] args)
        {
            Console.WriteLine();
            ArgValidator argValidator = new ArgValidator();
            bool? validatedArgs = argValidator.Validate(args);
            if (validatedArgs != true)
            {
                if (validatedArgs == false) { ShowHelp(); }
                Environment.Exit(0);
            }

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
            
            if (queryResult != null && queryResult.Count() > 0)
            {
                List<Tweet> results = queryResult.ToList();
                // Set start index for page.
                int startIndex = -1;
                int endIndex = 0;
                
                // Show page
                PageKey key = PageKey.Home;
                do
                {
                    bool updateScreen = false;
                    switch (key)
                    {
                        case PageKey.End:
                            if (endIndex != results.Count)
                            {
                                endIndex = results.Count;
                                startIndex = endIndex - argValidator.PageSize;
                                if (startIndex < 0)
                                {
                                    startIndex = 0;
                                }
                                updateScreen = true;
                            }
                            break;
                        case PageKey.Next:
                            if (endIndex != results.Count)
                            {
                                if (startIndex + argValidator.PageSize < results.Count)
                                {
                                    startIndex += argValidator.PageSize;
                                    endIndex += argValidator.PageSize;
                                    updateScreen = true;
                                }
                            }
                            break;
                        case PageKey.Home:
                            if (startIndex != 0)
                            {
                                Console.Clear();
                                startIndex = 0;
                                endIndex = startIndex +
                                           ((argValidator.PageSize == 0) ? results.Count : argValidator.PageSize);
                                updateScreen = true;
                            }
                            break;
                        case PageKey.Previous:
                            if (startIndex != 0)
                            {
                                startIndex -= argValidator.PageSize;
                                if (startIndex < 0) startIndex = 0;
                                endIndex = startIndex + argValidator.PageSize;
                                updateScreen = true;
                            }
                            break;
                        default:
                            updateScreen = false;
                            break;
                    }

                    for (int i = startIndex; i < endIndex && i < results.Count() && updateScreen; i++)
                    {
                        Console.WriteLine($"{results[i].Id}   {results[i].Stamp}\n{results[i].Text}");
                        Console.WriteLine("-------------------------------------------------------------------------");
                    }

                    if (argValidator.PageSize != 0)
                    {
                        if (updateScreen)
                        {
                            int endNum = (endIndex > results.Count) ? results.Count : endIndex;
                            Console.WriteLine(
                            $"\nRecords {startIndex + 1} - {endNum} of {results.Count}  < - previous  next - >\n");
                        }
                        key = CheckKey(Console.ReadKey());
                    }
                    else
                    {
                        key = PageKey.Quit;
                    }
                } while (key != PageKey.Quit);
                Console.WriteLine("\n======================================");
                Console.WriteLine($"Query from: {argValidator.StartDate} to {argValidator.EndDate}");
                Console.WriteLine($"Records Found: {queryResult.Count()}");
                Console.WriteLine("======================================");
            }
            else
            {
                Console.WriteLine("\n======================================");
                Console.WriteLine($"Query from: {argValidator.StartDate} to {argValidator.EndDate}");
                Console.WriteLine($"Records Found: 0");
                Console.WriteLine("======================================");
            }
        }

        private static PageKey CheckKey(ConsoleKeyInfo keyInfo)
        {
            Console.Write('\b');
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
                    Console.Write("\b \b");
                    break;
            }
            return PageKey.None;
        }

        static void ShowHelp()
        {
            Console.WriteLine("Lists and downloads messages from a remote archive.\n");
            Console.WriteLine("TBR url startdate [/ST starttime] enddate [/ET endtime]");
            Console.WriteLine(" [/F [drive:][path]filename] [/P pages] [/V]\n");
            Console.WriteLine("url         Url of the message archive.");
            Console.WriteLine("startdate   Start date for the range of messages in the archive.");
            Console.WriteLine("/ST         Start time for the range of messages in the archive.");
            Console.WriteLine("enddate     End date for the range of messages in the archive.");
            Console.WriteLine("/ET         End time for the range of messages in the archive.");
            Console.WriteLine("/F          Downloads and saves messages.");
            Console.WriteLine("[dirve:][path]filename");
            Console.WriteLine("            Specifies drive, directory, and name of file where");
            Console.WriteLine("            downloaded messages will be saved.");
            Console.WriteLine("/P          Displays messages on screen.");
            Console.WriteLine("pages       Sets the number of records to be displayed on each page.");
            Console.WriteLine("/V          Verbose mode shows remote host query details.");

        }
    }
}
