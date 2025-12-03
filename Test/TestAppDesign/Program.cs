using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FileHandlingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFile = @"E:\\School\\C.IV\\Prog Project\\Project 2 - OOP\\SCHAFF-Leeloo-Prog_ProOOP\\TestAppDesign\\TestAppDesign\\EmployeeData.csv";
            string outputFileCsvHelper = "ProcessedData_CsvHelper.csv";
            string outputFileStreamWriter = "ProcessedData_StreamWriter.csv";

            //CsvHelper
            Console.WriteLine("Testing CsvHelper...");
            TestCsvHelper(inputFile, outputFileCsvHelper);

            //StreamWriter
            Console.WriteLine("Testing StreamWriter...");
            TestStreamWriter(inputFile, outputFileStreamWriter);

            Console.WriteLine("\nTesting completed, check output files.");
        }

        //CsvHelper
        static void TestCsvHelper(string inputFile, string outputFile)
        {
            try
            {
                //Read
                Console.WriteLine("Readinng data...");
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true, //false?
                };

                //IEnumerable<Employee> records;
                List<Employee> records = new List<Employee>();

                using (var reader = new StreamReader(inputFile))
                using (var csv = new CsvReader(reader, config))
                {
                    //records = csv.GetRecords<Employee>().ToList();
                    var csvRecords = csv.GetRecords<dynamic>();
                    foreach (var record in csvRecords)
                    {
                        try
                        {
                            records.Add(new Employee
                            {
                                EmployeeID = int.Parse(record.EmployeeID),
                                Name = record.Name,
                                HoursWorked = int.Parse(record.HoursWorked),
                                HourlyRate = int.Parse(record.HourlyRate)
                            });
                        }
                        catch (Exception ex)
                        { 
                            Console.WriteLine($"Skipping record due to conversion error: {ex.Message}");
                        }
                    }
                }

                //Data processing
                Console.WriteLine("Processing data...");
                //calculate gross pay for each row
                foreach (var record in records)
                {
                    record.GrossPay = record.HoursWorked * record.HourlyRate;
                    Console.WriteLine($"Processed: {record.EmployeeID}, {record.Name}, Gross pay: {record.GrossPay}");
                }

                //Write
                Console.WriteLine("Writing data...");
                using (var writer = new StreamWriter(outputFile))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteHeader<Employee>();
                    csv.NextRecord();
                    csv.WriteRecords(records);
                }
                Console.WriteLine($"File written to: {outputFile}");
            }
            catch (CsvHelper.TypeConversion.TypeConverterException ex)
            {
                Console.WriteLine($"CsvHelper error: {ex.Message}");
                Console.WriteLine($"Invalid value: {ex.Text}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CsvHelper error: {ex.Message}");
            }
        }
    
        //StreamWriter
        static void TestStreamWriter(string inputFile, string outputFile)
        {
            try
            {
                var processedData = new StringBuilder();
                using (var reader = new StreamReader(inputFile))
                {
                    string headerLine = reader.ReadLine();
                    processedData.AppendLine(headerLine + ",GrossPay");

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var values = line.Split(',');
                        int hoursWorked = int.Parse(values[2]);
                        int hourlyRate = int.Parse(values[3]);
                        double grossPay = hoursWorked * hourlyRate;
                        processedData.AppendLine($"{line},{grossPay}");
                        Console.WriteLine($"Processed: {values[0]}, {values[1]}, Gross Pay: {grossPay}");
                    }
                }

                File.WriteAllText(outputFile, processedData.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"StreamWriter Error: {ex.Message}");
            }
        }
    }

    //Employee Class
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public int HoursWorked { get; set; }
        public int HourlyRate { get; set; }
        public double GrossPay { get; set; } //calculated field
    }
}