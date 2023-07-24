using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace CSVSearch
{
    public class CsvRow
    {
        [Index(0)]
        public int Id { get; set; }
        [Index(1)]
        public string LastName { get; set; }
        [Index(2)]
        public string FirstName { get; set; }
        [Index(3)]
        public DateTime DateOfBirth { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Enter the path to the CSV file, column number and search key:");
                string[] inp = Console.ReadLine().Split(' ');
                args = inp;
            }
            if (args.Length != 3)
            {
                Console.WriteLine("Invalid Arguments");
                return;
            }

            string csvPath = args[0];
            int columnIndex;
            if (!int.TryParse(args[1], out columnIndex) || columnIndex < 0)
            {
                Console.WriteLine("Invalid column index. Please provide a valid non-negative integer.");
                return;
            }

            string searchKey = args[2];

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false // Ignore the header row
                };
                using (var reader = new StreamReader(csvPath))
                using (var csv = new CsvReader(reader, config))
                {
                    var records = csv.GetRecords<CsvRow>();
                    foreach (var record in records)
                    {
                        string fieldValue = GetFieldValueByIndex(record, columnIndex);
                        if (fieldValue.Equals(searchKey, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"{record.Id}, {record.LastName}, {record.FirstName}, {record.DateOfBirth:dd/MM/yyyy};");
                            return;
                        }
                    }

                    Console.WriteLine("No matching record found.");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"File not found: {csvPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static string GetFieldValueByIndex(CsvRow row, int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return row.Id.ToString();
                case 1:
                    return row.LastName;
                case 2:
                    return row.FirstName;
                case 3:
                    return row.DateOfBirth.ToString("dd/MM/yyyy");
                default:
                    return string.Empty;
            }
        }
    }
}
