// Improve console UI
using System;
using System.Collections.Generic;

namespace InventoryManagementSystem
{
    public class ConsoleUI
    {
        private InventoryService inventory;
        private bool running = true;

        public ConsoleUI()
        {
            inventory = new InventoryService();
        }

        public void Run()
        {
            ShowWelcomeScreen();

            while (running)
            {
                try
                {
                    ShowMainMenu();
                    string choice = Console.ReadLine()?.Trim();
                    ProcessMenuChoice(choice);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }

                if (running)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey(true);
                }
            }
        }

        private void ShowWelcomeScreen()
        {
            Console.Clear();
            Console.WriteLine("=== INVENTORY MANAGEMENT SYSTEM ===");
            Console.WriteLine();
        }

        private void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("=== INVENTORY MANAGEMENT SYSTEM ===");
            Console.WriteLine();
            Console.WriteLine("1. Add Category");
            Console.WriteLine("2. Add Supplier");
            Console.WriteLine("3. Add Product");
            Console.WriteLine("4. View All Products");
            Console.WriteLine("5. Search Products");
            Console.WriteLine("6. Update Product");
            Console.WriteLine("7. Delete Product");
            Console.WriteLine("8. Restock Product");
            Console.WriteLine("9. Deduct Stock");
            Console.WriteLine("10. View Transaction History");
            Console.WriteLine("11. Show Low Stock Items");
            Console.WriteLine("12. Show Total Inventory Value");
            Console.WriteLine("0. Exit");
            Console.WriteLine();
            Console.Write("Enter choice: ");
        }

        private void ProcessMenuChoice(string choice)
        {
            switch (choice)
            {
                case "1": AddCategory(); break;
                case "2": AddSupplier(); break;
                case "3": AddProduct(); break;
                case "4": ViewAllProducts(); break;
                case "5": SearchProducts(); break;
                case "6": UpdateProduct(); break;
                case "7": DeleteProduct(); break;
                case "8": RestockProduct(); break;
                case "9": DeductStock(); break;
                case "10": ViewTransactionHistory(); break;
                case "11": ShowLowStockItems(); break;
                case "12": ShowTotalInventoryValue(); break;
                case "0": running = false; break;
                default: Console.WriteLine("Invalid choice!"); break;
            }
        }

        private void AddCategory()
        {
            string name = InputValidator.GetLettersOnly("Enter category name (letters only): ");
            string desc = InputValidator.GetLettersOnly("Enter description (letters only): ");
            inventory.AddCategory(name, desc);
        }

        private void AddSupplier()
        {
            Console.Write("Enter supplier name: ");
            string name = Console.ReadLine();
            Console.Write("Enter contact person: ");
            string contact = Console.ReadLine();
            Console.Write("Enter phone: ");
            string phone = Console.ReadLine();
            Console.Write("Enter email: ");
            string email = Console.ReadLine();
            Console.Write("Enter address: ");
            string address = Console.ReadLine();
            inventory.AddSupplier(name, contact, phone, email, address);
        }

        private void AddProduct()
        {
            if (inventory.GetAllCategories().Count == 0)
            {
                Console.WriteLine("No categories available. Please add a category first.");
                return;
            }
            if (inventory.GetAllSuppliers().Count == 0)
            {
                Console.WriteLine("No suppliers available. Please add a supplier first.");
                return;
            }

            string name = InputValidator.GetLettersOnly("Enter product name (letters only): ");
            string desc = InputValidator.GetLettersOnly("Enter description (letters only): ");
            Console.Write("Enter price: ");
            decimal price = decimal.Parse(Console.ReadLine());
            Console.Write("Enter quantity: ");
            int qty = int.Parse(Console.ReadLine());
            Console.Write("Enter minimum stock level: ");
            int minLevel = int.Parse(Console.ReadLine());

            Console.WriteLine("\nAvailable Categories:");
            foreach (var c in inventory.GetAllCategories())
                Console.WriteLine(c);

            Console.Write("Enter category ID: ");
            int catId = int.Parse(Console.ReadLine());

            Console.WriteLine("\nAvailable Suppliers:");
            foreach (var s in inventory.GetAllSuppliers())
                Console.WriteLine(s);

            Console.Write("Enter supplier ID: ");
            int supId = int.Parse(Console.ReadLine());

            inventory.AddProduct(name, desc, price, qty, minLevel, catId, supId);
        }

        private void ViewAllProducts()
        {
            var products = inventory.GetAllProducts();
            if (products.Count == 0)
            {
                Console.WriteLine("No products found.");
                return;
            }
            foreach (var p in products)
                Console.WriteLine("\n" + p);
        }

        private void SearchProducts()
        {
            string term = InputValidator.GetLettersOnly("Enter search term (letters only): ");
            var results = inventory.SearchProducts(term);
            if (results.Count == 0)
            {
                Console.WriteLine("No products found.");
                return;
            }
            foreach (var p in results)
                Console.WriteLine("\n" + p);
        }

        private void UpdateProduct()
        {
            Console.Write("Enter product ID to update: ");
            int id = int.Parse(Console.ReadLine());
            var product = inventory.GetProductById(id);
            if (product == null)
            {
                Console.WriteLine("Product not found!");
                return;
            }

            Console.WriteLine("Current product: " + product.Name);
            Console.Write("New name (press enter to keep '{0}'): ", product.Name);
            string nameInput = Console.ReadLine();
            string name = string.IsNullOrEmpty(nameInput) ? null : InputValidator.GetLettersOnly("Re-enter new name (letters only): ");

            Console.Write("New description (press enter to keep current): ");
            string descInput = Console.ReadLine();
            string desc = string.IsNullOrEmpty(descInput) ? null : InputValidator.GetLettersOnly("Re-enter description (letters only): ");

            Console.Write("New price (press enter to keep {0}): ", product.Price);
            string priceInput = Console.ReadLine();
            decimal? price = string.IsNullOrEmpty(priceInput) ? null : (decimal?)decimal.Parse(priceInput);

            Console.Write("New min stock level (press enter to keep {0}): ", product.MinStockLevel);
            string minInput = Console.ReadLine();
            int? minLevel = string.IsNullOrEmpty(minInput) ? null : (int?)int.Parse(minInput);

            inventory.UpdateProduct(id, name, desc, price, minLevel, null, null);
        }

        private void DeleteProduct()
        {
            Console.Write("Enter product ID to delete: ");
            int id = int.Parse(Console.ReadLine());
            inventory.DeleteProduct(id);
        }

        private void RestockProduct()
        {
            Console.Write("Enter product ID: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Enter amount to add: ");
            int amount = int.Parse(Console.ReadLine());
            Console.Write("Enter notes: ");
            string notes = Console.ReadLine();
            inventory.RestockProduct(id, amount, notes);
        }

        private void DeductStock()
        {
            Console.Write("Enter product ID: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Enter amount to deduct: ");
            int amount = int.Parse(Console.ReadLine());
            Console.Write("Enter notes: ");
            string notes = Console.ReadLine();
            inventory.DeductStock(id, amount, notes);
        }

        private void ViewTransactionHistory()
        {
            Console.Write("Enter product ID for specific history (press enter for all): ");
            string input = Console.ReadLine();
            int? productId = string.IsNullOrEmpty(input) ? null : (int?)int.Parse(input);

            var transactions = inventory.GetTransactionHistory(productId);
            if (transactions.Count == 0)
            {
                Console.WriteLine("No transactions found.");
                return;
            }
            foreach (var t in transactions)
                Console.WriteLine("\n" + t);
        }

        private void ShowLowStockItems()
        {
            var items = inventory.GetLowStockItems();
            if (items.Count == 0)
            {
                Console.WriteLine("No low stock items.");
                return;
            }
            Console.WriteLine("=== LOW STOCK ALERTS ===");
            foreach (var item in items)
                Console.WriteLine("\n" + item);
        }

        private void ShowTotalInventoryValue()
        {
            decimal total = inventory.GetTotalInventoryValue();
            Console.WriteLine("Total Inventory Value: ${0:F2}", total);
        }

        private void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: {0}", message);
            Console.ResetColor();
        }
    }
}
