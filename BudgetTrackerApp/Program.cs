using Microsoft.Extensions.Configuration;

namespace AsyncBudgetTracker
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var defaultCategory = config["DefaultCategory"] ?? "Other";
            const string filePath = @"C:\Projects\BudgetTrackerAsync\Expenses.csv";

            var expenses = new List<Expense>();

            await LoadExpensesAsync(filePath, expenses);

            await CaptureExpensesAsync(defaultCategory, expenses, filePath);
        }

        private static async Task LoadExpensesAsync(string filePath, List<Expense> expenses)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var lines = await File.ReadAllLinesAsync(filePath);
                    expenses.AddRange(lines.Select(line => line.Split(','))
                        .Select(parts => new Expense(double.Parse(parts[1]), parts[0])));

                    Console.WriteLine("Loaded expenses asynchronously!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task CaptureExpensesAsync(string defaultCategory, List<Expense> expenses, string filePath)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Enter expense amount (or 0 to finish):");
                    var amount = Convert.ToDouble(Console.ReadLine());
                    if (amount == 0) break;
                    if (amount < 0) throw new ArgumentException("Amount can't be negative!");

                    Console.WriteLine($"Enter category (default: {defaultCategory}):");
                    var category = Console.ReadLine() ?? defaultCategory;

                    expenses.Add(new Expense(amount, category));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            await DisplayAndSaveExpensesAsync(expenses, filePath);
        }

        private static async Task DisplayAndSaveExpensesAsync(List<Expense> expenses, string filePath)
        {
            try
            {
                //Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? string.Empty);

                var csvContent = string.Join("\n", expenses.Select(e => e.ToCsv()));

                await Task.Delay(1000);

                await File.WriteAllTextAsync(filePath, csvContent);
                Console.WriteLine("Saved expenses asynchronously!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save error: {ex.Message}");
            }

            Console.WriteLine("\nExpenses (sorted);");
            foreach (var e in expenses.OrderBy(e => e.Amount))
            {
                Console.WriteLine(e);
            }

            var totals = expenses.GroupBy(e => e.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount) });
            Console.WriteLine("\nTotals:");
            foreach (var t in totals)
            {
                Console.WriteLine($"{t.Category}: {t.Total:F2}");
            }
        }
    }
}