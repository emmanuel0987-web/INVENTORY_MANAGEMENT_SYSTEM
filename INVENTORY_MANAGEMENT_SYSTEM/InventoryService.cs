// Added inventory service logic
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryManagementSystem
{
    public class InventoryService
    {
        private List<Category> categories = new List<Category>();
        private List<Supplier> suppliers = new List<Supplier>();
        private List<Product> products = new List<Product>();
        private List<TransactionRecord> transactions = new List<TransactionRecord>();

        private int nextCategoryId = 1;
        private int nextSupplierId = 1;
        private int nextProductId = 1;
        private int nextTransactionId = 1;

        private string currentUser = "Admin";

        public void AddCategory(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty");

            var category = new Category(nextCategoryId++, name.Trim(), description ?? string.Empty);
            categories.Add(category);
            Console.WriteLine("Category added successfully: {0}", category.Name);
        }

        public List<Category> GetAllCategories() => new List<Category>(categories);
        public Category GetCategoryById(int id) => categories.FirstOrDefault(c => c.Id == id);

        public void AddSupplier(string name, string contactPerson, string phone, string email, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Supplier name cannot be empty");

            var supplier = new Supplier(nextSupplierId++, name.Trim(), contactPerson ?? string.Empty,
                                       phone ?? string.Empty, email ?? string.Empty, address ?? string.Empty);
            suppliers.Add(supplier);
            Console.WriteLine("Supplier added successfully: {0}", supplier.Name);
        }

        public List<Supplier> GetAllSuppliers() => new List<Supplier>(suppliers);
        public Supplier GetSupplierById(int id) => suppliers.FirstOrDefault(s => s.Id == id);

        public void AddProduct(string name, string description, decimal price, int quantity,
                              int minStockLevel, int categoryId, int supplierId)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name cannot be empty");
            if (price < 0) throw new ArgumentException("Price cannot be negative");
            if (quantity < 0) throw new ArgumentException("Quantity cannot be negative");
            if (minStockLevel < 0) throw new ArgumentException("Minimum stock level cannot be negative");

            var category = GetCategoryById(categoryId);
            if (category == null) throw new ArgumentException($"Category with ID {categoryId} not found");

            var supplier = GetSupplierById(supplierId);
            if (supplier == null) throw new ArgumentException($"Supplier with ID {supplierId} not found");

            var product = new Product(nextProductId++, name.Trim(), description ?? string.Empty,
                                     price, quantity, minStockLevel, category, supplier);
            products.Add(product);

            RecordTransaction(product.Id, product.Name, TransactionType.Added, quantity, 0, quantity, "New product added");
            Console.WriteLine("Product added successfully: {0} (ID: {1})", product.Name, product.Id);
        }

        public List<Product> GetAllProducts() => new List<Product>(products);
        public Product GetProductById(int id) => products.FirstOrDefault(p => p.Id == id);

        public List<Product> SearchProducts(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return GetAllProducts();

            searchTerm = searchTerm.ToLower();
            return products.Where(p =>
                (p.Name?.ToLower().Contains(searchTerm) ?? false) ||
                (p.Description?.ToLower().Contains(searchTerm) ?? false) ||
                (p.Category?.Name?.ToLower().Contains(searchTerm) ?? false) ||
                (p.Supplier?.Name?.ToLower().Contains(searchTerm) ?? false)
            ).ToList();
        }

        public void UpdateProduct(int productId, string name, string description,
                                 decimal? price, int? minStockLevel, int? categoryId, int? supplierId)
        {
            var product = GetProductById(productId);
            if (product == null) throw new ArgumentException($"Product with ID {productId} not found");

            string oldName = product.Name;
            int oldQty = product.Quantity;

            if (!string.IsNullOrWhiteSpace(name)) product.Name = name.Trim();
            if (description != null) product.Description = description;
            if (price.HasValue)
            {
                if (price.Value < 0) throw new ArgumentException("Price cannot be negative");
                product.Price = price.Value;
            }
            if (minStockLevel.HasValue)
            {
                if (minStockLevel.Value < 0) throw new ArgumentException("Min stock level cannot be negative");
                product.MinStockLevel = minStockLevel.Value;
            }
            if (categoryId.HasValue)
            {
                var cat = GetCategoryById(categoryId.Value);
                if (cat == null) throw new ArgumentException("Category not found");
                product.Category = cat;
            }
            if (supplierId.HasValue)
            {
                var sup = GetSupplierById(supplierId.Value);
                if (sup == null) throw new ArgumentException("Supplier not found");
                product.Supplier = sup;
            }

            RecordTransaction(product.Id, product.Name, TransactionType.Updated, 0, oldQty, product.Quantity, $"Updated from {oldName}");
            Console.WriteLine("Product updated successfully: {0}", product.Name);
        }

        public void DeleteProduct(int productId)
        {
            var product = GetProductById(productId);
            if (product == null) throw new ArgumentException($"Product with ID {productId} not found");

            RecordTransaction(product.Id, product.Name, TransactionType.Deleted, -product.Quantity, product.Quantity, 0, "Product deleted");
            products.Remove(product);
            Console.WriteLine("Product deleted successfully: {0}", product.Name);
        }

        public void RestockProduct(int productId, int amount, string notes)
        {
            if (amount <= 0) throw new ArgumentException("Restock amount must be greater than zero");

            var product = GetProductById(productId);
            if (product == null) throw new ArgumentException($"Product with ID {productId} not found");

            int beforeQty = product.Quantity;
            product.Quantity += amount;

            RecordTransaction(product.Id, product.Name, TransactionType.StockIn, amount, beforeQty, product.Quantity, notes);
            Console.WriteLine("Restocked {0} units of {1}. New quantity: {2}", amount, product.Name, product.Quantity);
        }

        public void DeductStock(int productId, int amount, string notes)
        {
            if (amount <= 0) throw new ArgumentException("Deduction amount must be greater than zero");

            var product = GetProductById(productId);
            if (product == null) throw new ArgumentException($"Product with ID {productId} not found");

            if (product.Quantity < amount)
                throw new InvalidOperationException($"Insufficient stock. Available: {product.Quantity}, Requested: {amount}");

            int beforeQty = product.Quantity;
            product.Quantity -= amount;

            RecordTransaction(product.Id, product.Name, TransactionType.StockOut, -amount, beforeQty, product.Quantity, notes);
            Console.WriteLine("Deducted {0} units of {1}. Remaining: {2}", amount, product.Name, product.Quantity);
        }

        public List<Product> GetLowStockItems() => products.Where(p => p.IsLowStock()).ToList();
        public decimal GetTotalInventoryValue() => products.Sum(p => p.GetInventoryValue());

        public List<TransactionRecord> GetTransactionHistory(int? productId) =>
            productId.HasValue ? transactions.Where(t => t.ProductId == productId.Value).ToList() : new List<TransactionRecord>(transactions);

        private void RecordTransaction(int productId, string productName, TransactionType type,
                                      int quantityChanged, int qtyBefore, int qtyAfter, string notes)
        {
            var transaction = new TransactionRecord(nextTransactionId++, productId, productName, type,
                                                   quantityChanged, qtyBefore, qtyAfter, currentUser, notes);
            transactions.Add(transaction);
        }
    }
}
