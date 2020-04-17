using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace PerformanceTool
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            mainAsync().GetAwaiter().GetResult();
            watch.Stop();
            long time = watch.ElapsedMilliseconds;
            Console.WriteLine(time);


        }

        static async Task mainAsync()
        {
            using (var reader = new StreamReader("C:\\Users\\t-dacue\\Downloads\\query_data (16).csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<PerfToolHelper>().ToList();
                var wantedList = new List<CalcRunTime>();

                int count = 0;
                foreach (PerfToolHelper record in records)
                {
                    Console.WriteLine(record.packageId);
                    count++;
                   

       
                    for (int i = 1; i <= 20; i++)
                    {
                        CalcRunTime item = new CalcRunTime();
                        item.Iteration = i;
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        //Console.WriteLine(record.packageId);
                        item.packageId = record.packageId;
                        try
                        {
                            var responseString = await client.GetStringAsync("https://localhost/packages/" + record.packageId + "/");
                        }
                        catch (Exception ex)
                        {
                           // Console.WriteLine(ex.Message);
                            item.ErrorMessage = ex.Message;
                        }

                        watch.Stop();
                       // Console.WriteLine(watch.ElapsedMilliseconds);
                        item.Duration = watch.ElapsedMilliseconds;
                        //Console.WriteLine(item.Iteration + " " + item.packageId + " " +  item.Duration + " " + item.ErrorMessage); 
                        wantedList.Add(item);
                    }
                }

                using (var writer = new StreamWriter("C:\\Users\\t-dacue\\Downloads\\time_data(preFeat).csv"))
                using (var csv2 = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv2.WriteRecords(wantedList);
                }
            }

        }
    }

    public class PerfToolHelper
    {
        public String packageId { get; set; }
        public int requestCount { get; set; }
        public String p50Duration { get; set; }
        public String pctOfTotal { get; set; }
        public String runningPctOfTotal { get; set; }
        public int rank { get; set; }
    }

    public class CalcRunTime
    {
        public int Iteration { get; set; }

        public String packageId { get; set; }

        public long Duration { get; set; }

        public String ErrorMessage { get; set; }
    }
}
