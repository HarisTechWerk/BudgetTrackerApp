namespace AsyncBudgetTracker
{
    internal class Expense(double amount, string category)
    {
        public double Amount { get; set; } = amount;
        public string Category { get; set; } = category;

        public override string ToString()
        {
            return $"{Category}, {Amount:F2}";
        }

        public string ToCsv()
        {
            return $"{Category},{Amount}";
        }
    }
}