using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;

namespace Spam
{
    internal class Program
    {
        static string? connectionString;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            string path = Directory.GetCurrentDirectory();
            
            builder.SetBasePath(path);
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            connectionString = config.GetConnectionString("DefaultConnection");

            //ShowAllCities();
            ShowAllCountry();
            //ShowAllUsers();
            //ShowAllEmails();
            //ShowAllCategories();
            //ShowAllPromotions();
            //ShowUserFromCity();
            //ShowUserFromCountry();
            ShowPromotionsByCountries();
        }

        static void ShowAllCities()
        {
            Console.Clear();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var cities = db.Query<City>("SELECT * FROM City");
                foreach (var city in cities)
                    Console.WriteLine($"City - {city.Name}");
            }
            Console.ReadKey();
        }
        static void ShowAllCountry()
        {
            Console.Clear();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var countries = db.Query<Country>("SELECT DISTINCT Name FROM Country");
                foreach (var i in countries)
                    Console.WriteLine($"Country - {i.Name}");
            }
            Console.ReadKey();
        }
        static void ShowAllUsers()
        {
            Console.Clear();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var users = db.Query<User>("SELECT * FROM [User]");
                foreach (var i in users)
                    Console.WriteLine($"User - {i.Name} {i.Surname}");
            }
            Console.ReadKey();
        }
        static void ShowAllEmails()
        {
            Console.Clear();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var emails = db.Query<User>("SELECT Name, Email FROM [User]");
                foreach (var i in emails)
                    Console.WriteLine($"{i.Name}'s email - {i.Email}");
            }
            Console.ReadKey();
        }
        static void ShowAllCategories()
        {
            Console.Clear();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var categories = db.Query<Category>("SELECT Name FROM Category");
                Console.WriteLine("List of categories: ");
                foreach (var i in categories)
                    Console.WriteLine($"\t- {i.Name}");
            }
            Console.ReadKey();
        }
        static void ShowAllPromotions()
        {
            Console.Clear();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var promotions = db.Query<Category>("SELECT Name FROM Promotions");
                Console.WriteLine("List of promotions: ");
                foreach (var i in promotions)
                    Console.WriteLine($"\t- {i.Name}");
            }
            Console.ReadKey();
        }
        static void ShowUserFromCity()
        {
            Console.Clear();

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.WriteLine("Input name of city: ");
                string cityName = Console.ReadLine()!;

                var users = db.Query<User>(@"SELECT u.* FROM [User] u
                                            INNER JOIN Country co ON co.Id = u.CountryId
                                            INNER JOIN City ci ON ci.Id = co.CityId
                                            WHERE ci.Name = @CityName ", new { CityName = cityName }).ToList();
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.Id}, {user.Name} {user.Surname}, {user.Email}, {user.Gender}, {user.BirthDate.ToShortDateString()}");
                }
                
            }
            Console.ReadKey();
        }
        static void ShowUserFromCountry()
        {
            Console.Clear();

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.WriteLine("Input name of country: ");
                string countryName = Console.ReadLine()!;

                var users = db.Query<User>(@"SELECT u.* FROM [User] u
                                            INNER JOIN Country co ON co.Id = u.CountryId
                                            WHERE co.Name = @CountryName ", new { CountryName = countryName }).ToList();
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.Id}, {user.Name} {user.Surname}, {user.Email}, {user.Gender}, {user.BirthDate.ToShortDateString()}");
                }
            }
            Console.ReadKey();
        }
        static void ShowPromotionsByCountries()
        {
            Console.Clear();

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.WriteLine("Input name of country: ");
                string countryName = Console.ReadLine()!;

                var prromotions = db.Query<Promotions>(@"SELECT p.* FROM Promotions p
                                            INNER JOIN Country co ON co.Id = p.CountryId
                                            WHERE co.Name = @CountryName ", new { CountryName = countryName }).ToList();
                foreach (var i in prromotions)
                {
                    Console.WriteLine($"Promotion: {i.Name}, {i.StartTime.ToShortDateString()}, { i.EndTime.ToShortDateString()}");
                }
            }
            Console.ReadKey();
        }

    }
}
