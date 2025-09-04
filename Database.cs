using System;
using System.Data.SQLite;

namespace App
{
    public class Database
    {
        private string connectionString;

        public Database()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SalesDB"].ConnectionString;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string createCustomers = @"CREATE TABLE IF NOT EXISTS Customers (
                                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            FirstName TEXT NOT NULL,
                                            LastName TEXT NOT NULL
                                        );";
                
                string createSellers = @"CREATE TABLE IF NOT EXISTS Sellers (
                                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            FirstName TEXT NOT NULL,
                                            LastName TEXT NOT NULL
                                        );";
             
                string createSales = @"CREATE TABLE IF NOT EXISTS Sales (
                                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            CustomerId INTEGER NOT NULL,
                                            SellerId INTEGER NOT NULL,
                                            Amount REAL NOT NULL,
                                            SaleDate TEXT NOT NULL,
                                            FOREIGN KEY(CustomerId) REFERENCES Customers(Id),
                                            FOREIGN KEY(SellerId) REFERENCES Sellers(Id)
                                        );";

                SQLiteCommand cmd = new SQLiteCommand(createCustomers, connection);
                cmd.ExecuteNonQuery();
                cmd.CommandText = createSellers;
                cmd.ExecuteNonQuery();
                cmd.CommandText = createSales;
                cmd.ExecuteNonQuery();
            }
        }

        public void AddCustomer(string firstName, string lastName)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var cmd = new SQLiteCommand("INSERT INTO Customers (FirstName, LastName) VALUES (@first, @last)", connection);
                cmd.Parameters.AddWithValue("@first", firstName);
                cmd.Parameters.AddWithValue("@last", lastName);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddSeller(string firstName, string lastName)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var cmd = new SQLiteCommand("INSERT INTO Sellers (FirstName, LastName) VALUES (@first, @last)", connection);
                cmd.Parameters.AddWithValue("@first", firstName);
                cmd.Parameters.AddWithValue("@last", lastName);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddSale(int customerId, int sellerId, double amount, DateTime saleDate)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var cmd = new SQLiteCommand("INSERT INTO Sales (CustomerId, SellerId, Amount, SaleDate) VALUES (@cId, @sId, @amount, @date)", connection);
                cmd.Parameters.AddWithValue("@cId", customerId);
                cmd.Parameters.AddWithValue("@sId", sellerId);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@date", saleDate.ToString("yyyy-MM-dd"));
                cmd.ExecuteNonQuery();
            }
        }

        public void ShowSalesByPeriod(DateTime start, DateTime end)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var cmd = new SQLiteCommand("SELECT s.Id, c.FirstName || ' ' || c.LastName AS Customer, " +
                                            "sel.FirstName || ' ' || sel.LastName AS Seller, s.Amount, s.SaleDate " +
                                            "FROM Sales s " +
                                            "JOIN Customers c ON s.CustomerId = c.Id " +
                                            "JOIN Sellers sel ON s.SellerId = sel.Id " +
                                            "WHERE s.SaleDate BETWEEN @start AND @end", connection);
                cmd.Parameters.AddWithValue("@start", start.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@end", end.ToString("yyyy-MM-dd"));

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["Id"]}, Покупець: {reader["Customer"]}, Продавець: {reader["Seller"]}, Сума: {reader["Amount"]}, Дата: {reader["SaleDate"]}");
                }
            }
        }

        public void ShowLastPurchase(string firstName, string lastName)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var cmd = new SQLiteCommand("SELECT s.Id, sel.FirstName || ' ' || sel.LastName AS Seller, s.Amount, s.SaleDate " +
                                            "FROM Sales s " +
                                            "JOIN Customers c ON s.CustomerId = c.Id " +
                                            "JOIN Sellers sel ON s.SellerId = sel.Id " +
                                            "WHERE c.FirstName = @first AND c.LastName = @last " +
                                            "ORDER BY s.SaleDate DESC LIMIT 1", connection);
                cmd.Parameters.AddWithValue("@first", firstName);
                cmd.Parameters.AddWithValue("@last", lastName);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Console.WriteLine($"Остання покупка: Продавець: {reader["Seller"]}, Сума: {reader["Amount"]}, Дата: {reader["SaleDate"]}");
                }
                else
                {
                    Console.WriteLine("Покупець не знайдений або покупок немає.");
                }
            }
        }

        public void DeletePerson(string type, int id)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string table = type == "c" ? "Customers" : "Sellers";
                var cmd = new SQLiteCommand($"DELETE FROM {table} WHERE Id=@id", connection);
                cmd.Parameters.AddWithValue("@id", id);
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? "Видалено!" : "ID не знайдено.");
            }
        }

        public void ShowTopSeller()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var cmd = new SQLiteCommand("SELECT sel.FirstName || ' ' || sel.LastName AS Seller, SUM(s.Amount) AS Total " +
                                            "FROM Sales s " +
                                            "JOIN Sellers sel ON s.SellerId = sel.Id " +
                                            "GROUP BY sel.Id " +
                                            "ORDER BY Total DESC LIMIT 1", connection);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Console.WriteLine($"Топ-продавець: {reader["Seller"]}, Загальна сума продаж: {reader["Total"]}");
                }
                else
                {
                    Console.WriteLine("Продажі відсутні.");
                }
            }
        }

    }
}
