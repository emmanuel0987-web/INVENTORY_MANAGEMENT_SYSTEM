# INVENTORY_MANAGEMENT_SYSTEM

 Description

This project is a Console-Based Inventory Management System developed in C#.
It allows users to manage products, categories, suppliers, stock levels, and transaction history efficiently.

The system is designed to simulate a real-world inventory system with essential features such as stock tracking, product management, and reporting.

 Core Features

* Add, view, and manage Categories
* Add, view, and manage Suppliers
* Add, update, delete, and search Products
* Stock Management

  * Restock products (Stock In)
  * Deduct stock (Stock Out)
* Low Stock Alerts
* Inventory Valuation Report

-Advanced Features

* **Transaction History Tracking**

  * Records all actions (Add, Update, Delete, Stock In, Stock Out)
* **User Authentication System**

  * Login required before accessing the system
* **Search Functionality**

  * Search by product name, category, or supplier

 System Structure

 Main Classes

* `Category` – Handles product categories
* `Supplier` – Stores supplier information
* `Product` – Represents inventory items
* `User` – Handles authentication
* `TransactionRecord` – Tracks inventory activities
* `InventoryService` – Core business logic
* `ConsoleUI` – User interface (menu system)
* `Program` – Entry point of the application

 How to Run

1. Open the project in **Visual Studio** or any C# IDE
2. Build the solution
3. Run the program
4. Login using the default credentials

Product Management

* Add new products with category and supplier
* Update product details
* Delete products
* Search products

Stock Management

* Increase stock (Restock)
* Decrease stock (Deduct)
* Automatic low stock detection

 Transaction System
Every action is recorded:
* Product Added
* Product Updated
* Product Deleted
* Stock In / Stock Ou

Reports
* View all products
* View low stock items
* View transaction history
* View total inventory value


##  Author

* John Emmanuel Ordan

