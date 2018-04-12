using System;
using System.Linq;

namespace TBR
{
    class Program
    {
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
            var results = tdc.GetItemsFromUrl(argValidator.Url, argValidator.StartDate, argValidator.EndDate, argValidator.Verbose);
            Console.WriteLine("\n======================================");
            Console.WriteLine($"Query from: {argValidator.StartDate} to {argValidator.EndDate}");
            Console.WriteLine($"Records Found: {results.Count()}");
            Console.WriteLine("======================================");
            foreach (var result in results)
            {
                Console.WriteLine($"{result.Id}   {result.Stamp}\n{result.Text}");
            }

            Console.Write("\nHit any key...");
            Console.ReadKey();
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
