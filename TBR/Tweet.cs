using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace TBR
{
    public class Tweet
    {
        public string Id { get; set; }
        public string Stamp { get; set; }
        public string Text { get; set; }

        // Converts the date string to a typed date.
        // If the date string is invalid it returns a default (min date value).
        [JsonIgnore]
        public DateTime Date
        {
            get
            {
                DateTime date;
                try
                {
                    date = Convert.ToDateTime(Stamp);
                }
                catch
                {
                    return DateTime.MinValue;
                }

                return date;
            }
        }
    }
}
