using App;
using System;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            Database db = new Database();

            while (true)
            {
                Console.WriteLine("\n=== Меню ===");
                Console.WriteLine("1. Додати покупця");
                Console.WriteLine("2. Додати продавця");
                Console.WriteLine("3. Додати продаж");
                Console.WriteLine("4. Показати всі продажі за період");
                Console.WriteLine("5. Показати останню покупку покупця");
                Console.WriteLine("6. Видалити покупця або продавця по ID");
                Console.WriteLine("7. Показати топ-продавця за сумою продаж");
                Console.WriteLine("0. Вийти");
                Console.Write("Виберіть дію: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Ім'я покупця: ");
                        string cFirst = Console.ReadLine();
                        Console.Write("Прізвище покупця: ");
                        string cLast = Console.ReadLine();
                        db.AddCustomer(cFirst, cLast);
                        Console.WriteLine("Покупець доданий!");
                        break;

                    case "2":
                        Console.Write("Ім'я продавця: ");
                        string sFirst = Console.ReadLine();
                        Console.Write("Прізвище продавця: ");
                        string sLast = Console.ReadLine();
                        db.AddSeller(sFirst, sLast);
                        Console.WriteLine("Продавець доданий!");
                        break;

                    case "3":
                        Console.Write("ID покупця: ");
                        int custId = int.Parse(Console.ReadLine());
                        Console.Write("ID продавця: ");
                        int sellId = int.Parse(Console.ReadLine());
                        Console.Write("Сума продажу: ");
                        double amount = double.Parse(Console.ReadLine());
                        Console.Write("Дата продажу (yyyy-MM-dd): ");
                        DateTime date = DateTime.Parse(Console.ReadLine());
                        db.AddSale(custId, sellId, amount, date);
                        Console.WriteLine("Продаж доданий!");
                        break;

                    case "4":
                        Console.Write("Дата початку (yyyy-MM-dd): ");
                        DateTime start = DateTime.Parse(Console.ReadLine());
                        Console.Write("Дата кінця (yyyy-MM-dd): ");
                        DateTime end = DateTime.Parse(Console.ReadLine());
                        db.ShowSalesByPeriod(start, end);
                        break;

                    case "5":
                        Console.Write("Ім'я покупця: ");
                        string cfName = Console.ReadLine();
                        Console.Write("Прізвище покупця: ");
                        string clName = Console.ReadLine();
                        db.ShowLastPurchase(cfName, clName);
                        break;

                    case "6":
                        Console.Write("Введіть 'c' для покупця або 's' для продавця: ");
                        string type = Console.ReadLine().ToLower();
                        Console.Write("Введіть ID для видалення: ");
                        int id = int.Parse(Console.ReadLine());
                        db.DeletePerson(type, id);
                        break;

                    case "7":
                        db.ShowTopSeller();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        break;
                }
            }
        }
    }
}
