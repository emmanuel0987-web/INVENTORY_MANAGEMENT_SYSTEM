// Added Product class improvements
using System;
using System.Text.RegularExpressions;

namespace InventoryManagementSystem
{
    public class Category
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Category(int id, string name, string description)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1} - {2}", Id, Name, Description);
        }
    }

    public class Supplier
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public Supplier(int id, string name, string contactPerson, string phone, string email, string address)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ContactPerson = contactPerson ?? string.Empty;
            Phone = phone ?? string.Empty;
            Email = email ?? string.Empty;
            Address = address ?? string.Empty;
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1} | Contact: {2} | Phone: {3}", Id, Name, ContactPerson, Phone);
        }
    }

    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int MinStockLevel { get; set; }
        public Category Category { get; set; }
        public Supplier Supplier { get; set; }
        public DateTime DateAdded { get; private set; }

        public Product(int id, string name, string description, decimal price, int quantity,
                      int minStockLevel, Category category, Supplier supplier)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            Price = price >= 0 ? price : throw new ArgumentException("Price cannot be negative");
            Quantity = quantity >= 0 ? quantity : throw new ArgumentException("Quantity cannot be negative");
            MinStockLevel = minStockLevel >= 0 ? minStockLevel : throw new ArgumentException("Min stock level cannot be negative");
            Category = category ?? throw new ArgumentNullException(nameof(category));
            Supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
            DateAdded = DateTime.Now;
        }

        public decimal GetInventoryValue() => Price * Quantity;
        public bool IsLowStock() => Quantity <= MinStockLevel;

        public override string ToString()
        {
            string stockStatus = IsLowStock() ? " [LOW STOCK!]" : "";
            return string.Format("[{0}] {1}{2}\n    Description: {3}\n    Price: ${4:F2} | Quantity: {5} | Min Level: {6}\n    Category: {7} | Supplier: {8}\n    Added: {9:yyyy-MM-dd} | Value: ${10:F2}",
                Id, Name, stockStatus, Description, Price, Quantity, MinStockLevel,
                Category?.Name ?? "N/A", Supplier?.Name ?? "N/A", DateAdded, GetInventoryValue());
        }
    }

    public enum TransactionType
    {
        StockIn, StockOut, Adjustment, Added, Updated, Deleted
    }

    public class TransactionRecord
    {
        public int Id { get; private set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public TransactionType Type { get; set; }
        public int QuantityChanged { get; set; }
        public int QuantityBefore { get; set; }
        public int QuantityAfter { get; set; }
        public string PerformedBy { get; set; }
        public DateTime Timestamp { get; private set; }
        public string Notes { get; set; }

        public TransactionRecord(int id, int productId, string productName, TransactionType type,
                               int quantityChanged, int quantityBefore, int quantityAfter,
                               string performedBy, string notes)
        {
            Id = id;
            ProductId = productId;
            ProductName = productName ?? "Unknown";
            Type = type;
            QuantityChanged = quantityChanged;
            QuantityBefore = quantityBefore;
            QuantityAfter = quantityAfter;
            PerformedBy = performedBy ?? "System";
            Timestamp = DateTime.Now;
            Notes = notes ?? string.Empty;
        }

        public override string ToString()
        {
            string change = QuantityChanged >= 0 ? $"+{QuantityChanged}" : QuantityChanged.ToString();
            return string.Format("[{0}] {1:yyyy-MM-dd HH:mm} | {2,-12} | Product: {3} ({4})\n    Change: {5} | Before: {6} → After: {7}\n    By: {8} | Notes: {9}",
                Id, Timestamp, Type, ProductName, ProductId, change, QuantityBefore, QuantityAfter, PerformedBy, Notes);
        }
    }

    public static class InputValidator
    {
        public static bool IsLettersOnly(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;
            return Regex.IsMatch(input, @"^[a-zA-Z\s]+$");
        }

        public static string GetLettersOnly(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                if (IsLettersOnly(input))
                    return input.Trim();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Only letters and spaces allowed. No numbers or special characters.");
                Console.ResetColor();
            }
        }
    }
}
