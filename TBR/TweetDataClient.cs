using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TBR
{
    // Implements an http client to retrieve data from a remote site.
    // A request for data will contain the url for the remote site
    // and a start and end date/time for the range of records to be returned.
    // NOTE: The remote API being called for this demo has a limitation.
    // It can only return up to a max of 100 records per request. 
    // If the query results in more than 100 records, the API will only
    // return the first 100 and ignore the rest. No notification of any
    // kind will be sent to indicate records were ignored.
    public class TweetDataClient
    {
        private readonly HttpClient client = new HttpClient();
        private TimeSpan _maxTimeSpan;
        private bool _verbose;

        // This method is used to submit a request to the client.
        public IEnumerable<Tweet> GetItemsFromUrl(string url, DateTime startDate, DateTime endDate, bool verbose)
        {
            _verbose = verbose;
            IEnumerable<Tweet> tweets = RunAsync(url, startDate, endDate).GetAwaiter().GetResult();
            return tweets;
        }

        // This task sends a request to the remote site.
        // The response is a json object that maps to a collection of Tweet objects.
        private async Task<ICollection<Tweet>> GetTweetsAsync(string path)
        {
            ICollection<Tweet> tweets = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                tweets = await response.Content.ReadAsJsonAsync<ICollection<Tweet>>();
            }
            return tweets;
        }

        // This method builds a query based on start and end date/times. 
        // It then submits the query. If 100 records are returned, it
        // breaks the query into mulitple smaller queries and resubmits.
        private async Task<IEnumerable<Tweet>> RunAsync(string uriString, DateTime startDate, DateTime maxDate)
        {
            // Set the initial timespan to the original start and end date/times.
            DateTime endDate = maxDate;
            _maxTimeSpan = endDate.Subtract(startDate);

            if (_verbose)
            {
                Console.WriteLine("==============================================");
                Console.WriteLine($"Begin remote query to {uriString}  TimeSpan={_maxTimeSpan.TotalDays} days");
            }

            // Build the header.
            if (client.BaseAddress == null)
            {
                client.BaseAddress = new Uri(uriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            }

            // Build the query.
            List<Tweet> allTweets = new List<Tweet>();
            bool doneQuerying = false;

            while (!doneQuerying)
            {
                UriBuilder uriBuilder = new UriBuilder(uriString);
                uriBuilder.Port = -1;
                uriBuilder.Query = "startDate=" + startDate.ToString() + "&endDate=" + endDate.ToString();
                Uri uri = new Uri(uriBuilder.ToString());

                // Get the tweets.
                IEnumerable<Tweet> tweets = null;
                if (_verbose)
                {
                    Console.WriteLine($"Query request: StartDate: {startDate}  EndDate: {endDate}");
                }

                tweets = await GetTweetsAsync(uri.PathAndQuery);

                // Check to see if the Tweet API's 100 record limit was reached.
                if (!MaxRecordsExceeded(tweets))
                {
                    allTweets.AddRange(tweets);
                    doneQuerying = endDate == maxDate;
                    if (!doneQuerying)
                    {
                        startDate = IncrementDate(startDate, maxDate);
                        endDate = IncrementDate(endDate, maxDate);
                    }
                }
                else
                {
                    // Adjust endDate based on new _maxTimespan and resubmit the query
                    endDate = IncrementDate(startDate, maxDate);
                }
            }

            if (_verbose)
            {
                Console.WriteLine($"Remote query to {uriString} completed successfully");
                Console.WriteLine("==============================================");
            }

            return allTweets;
        }

        private bool MaxRecordsExceeded(IEnumerable<Tweet> tweets)
        {
            // If 100 records were returned it is likely the Tweet API's 100 record
            // limitation was reached. This reduces the timespan of the query
            // to reduce the number of records returned to be less than 100. 
            // Note: In this version it always starts at max and then throttles down.
            // No attempt is made to throttle back up again.
            string span = "Default";

            if (tweets.Count() >= 100)
            {
                if (_maxTimeSpan > TimeSpan.FromDays(6))
                {
                    int days = _maxTimeSpan.Days / 2;
                    _maxTimeSpan = TimeSpan.FromDays(days);
                    span = days + " days";
                }
                else if (_maxTimeSpan > TimeSpan.FromDays(3))
                {
                    _maxTimeSpan = TimeSpan.FromDays(3);
                    span = "3 days";
                }
                else if (_maxTimeSpan > TimeSpan.FromDays(2))
                {
                    _maxTimeSpan = TimeSpan.FromDays(2);
                    span = "2 days";
                }
                else if (_maxTimeSpan > TimeSpan.FromDays(1))
                {
                    _maxTimeSpan = TimeSpan.FromDays(1);
                    span = "1 day";
                }
                else if (_maxTimeSpan > TimeSpan.FromHours(12))
                {
                    _maxTimeSpan = TimeSpan.FromHours(12);
                    span = "12 hours";
                }
                else if (_maxTimeSpan > TimeSpan.FromHours(1))
                {
                    _maxTimeSpan = TimeSpan.FromHours(1);
                    span = "1 hour";
                }
                else if (_maxTimeSpan > TimeSpan.FromMinutes(30))
                {
                    _maxTimeSpan = TimeSpan.FromMinutes(30);
                    span = "30 mins";
                }
                else
                {
                    // Timespan could be throttled even further but at this point
                    // performance will be so bad another solution should be considered.
                    if (_verbose)
                    {
                        Console.WriteLine(
                            "***ERROR - Query result > 100 records and minimum timespan has been reached.");
                    }

                    throw new TweetWebApiException("Unrealiable host data. Records may be missing.");
                }

                if (_verbose)
                {
                    Console.WriteLine($"Query result > 100 records. Reduce timespan to {span}", span);
                }

                return true;
            }
            return false;
        }

        // Increments the date for the next query.
        private DateTime IncrementDate(DateTime date, DateTime maxDate)
        {
            if (date + _maxTimeSpan > maxDate)
            {
                return maxDate;
            }

            return date + _maxTimeSpan;
        }
    }

    // Extension used to support async reading of json objects in Core 2.0.
    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            string json = await content.ReadAsStringAsync();
            T value = JsonConvert.DeserializeObject<T>(json);
            return value;
        }
    }

    // Exception used to indicate the timespan has been reduced too many times.
    public class TweetWebApiException : Exception
    {
        public TweetWebApiException(string message) : base(message) { }
    }
}
