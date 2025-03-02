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
            //ShowAllCountry();
            //ShowAllUsers();
            //ShowAllEmails();
            //ShowAllCategories();
            //ShowAllPromotions();
            //ShowUserFromCity();
            //ShowUserFromCountry();
            //ShowPromotionsByCountries();
            //InsertUser();
            //InsertCountry();
            //InsertCity();
            //InsertCategory();
            //InsertPromotion();
            //UpdateUser();
            //UpdateCountry();
            //UpdateCity();
            //UpdateCategory();
            //UpdatePromotion();
            //DeleteUser();
            //DeleteCountry();
            //DeleteCity();
            //DeleteCategory();
            //DeletePromotion();
            ShowCitiesByCountry();
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
                    Console.WriteLine($"Promotion: {i.Name}, {i.StartDate.ToShortDateString()}, {i.EndDate.ToShortDateString()}");
                }
            }
            Console.ReadKey();
        }
        static void InsertUser()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.Write("Input user name: ");
                string userName = Console.ReadLine();

                Console.Write("Input user surname: ");
                string userSurname = Console.ReadLine();

                Console.Write("Input user email: ");
                string userEmail = Console.ReadLine();

                Console.Write("Input user gender: ");
                string userGender = Console.ReadLine();

                Console.Write("Input user birth date: ");
                DateTime birthDate = DateTime.Parse(Console.ReadLine());

                Console.Write("Input country name: ");
                string countryName = Console.ReadLine();

                string countryQuery = "SELECT Id FROM Country WHERE Name = @Name";
                int? countryId = db.QueryFirstOrDefault<int?>(countryQuery, new { Name = countryName });

                if (countryId == null)
                {
                    Console.Write("Input city name for this country: ");
                    string cityName = Console.ReadLine();

                    string cityQuery = "SELECT Id FROM City WHERE Name = @Name";
                    int? cityId = db.QueryFirstOrDefault<int?>(cityQuery, new { Name = cityName });

                    if (cityId == null)
                    {
                        string insertCityQuery = "INSERT INTO City (Name) OUTPUT INSERTED.Id VALUES (@Name)";
                        cityId = db.ExecuteScalar<int>(insertCityQuery, new { Name = cityName });
                        Console.WriteLine($"New city added: {cityName}");
                    }

                    string insertCountryQuery = "INSERT INTO Country (Name, CityId) OUTPUT INSERTED.Id VALUES (@Name, @CityId)";
                    countryId = db.ExecuteScalar<int>(insertCountryQuery, new { Name = countryName, CityId = cityId });
                    Console.WriteLine($"New country added: {countryName}");
                }

                string insertUserQuery = @"
                INSERT INTO User (Name, Surname, Email, Gender, BirthDate, CountryId)
                VALUES (@Name, @Surname, @Email, @Gender, @BirthDate, @CountryId)";

                db.Execute(insertUserQuery, new
                {
                    Name = userName,
                    Surname = userSurname,
                    Email = userEmail,
                    Gender = userGender,
                    BirthDate = birthDate,
                    CountryId = countryId
                });

                Console.WriteLine("User added successfully!");
            }
        }
        static void InsertCountry()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.Write("Input name of country: ");
                string countryName = Console.ReadLine();

                Console.Write("Input name of city: ");
                string cityName = Console.ReadLine();

                string cityQuery = "SELECT Id FROM City WHERE Name = @Name";
                int? cityId = db.QueryFirstOrDefault<int?>(cityQuery, new { Name = cityName });

                if (cityId == null)
                {
                    string insertCityQuery = "INSERT INTO City (Name) OUTPUT INSERTED.Id VALUES (@Name)";
                    cityId = db.ExecuteScalar<int>(insertCityQuery, new { Name = cityName });
                    Console.WriteLine($"New city added: {cityName}");
                }

                string countryQuery = "SELECT Id FROM Country WHERE Name = @Name";
                int? countryId = db.QueryFirstOrDefault<int?>(countryQuery, new { Name = countryName });

                if (countryId != null)
                {
                    Console.WriteLine($"Country '{countryName}' already exists");
                }
                else
                {
                    string insertCountryQuery = "INSERT INTO Country (Name, CityId) OUTPUT INSERTED.Id VALUES (@Name, @CityId)";
                    countryId = db.ExecuteScalar<int>(insertCountryQuery, new { Name = countryName, CityId = cityId });
                    Console.WriteLine($"New country added: {countryName}");
                }
            }
        }
        static void InsertCity()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.Write("Input name of city: ");
                string cityName = Console.ReadLine();

                string cityQuery = "SELECT Id FROM City WHERE Name = @Name";
                int? cityId = db.QueryFirstOrDefault<int?>(cityQuery, new { Name = cityName });

                if (cityId != null)
                {
                    Console.WriteLine($"City '{cityName}' already exists");
                }
                else
                {
                    string insertCityQuery = "INSERT INTO City (Name) OUTPUT INSERTED.Id VALUES (@Name)";
                    cityId = db.ExecuteScalar<int>(insertCityQuery, new { Name = cityName });
                    Console.WriteLine($"New city added: {cityName}");
                }
            }
        }
        static void InsertCategory()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.Write("Input category name: ");
                string categoryName = Console.ReadLine();

                string categoryQuery = "SELECT Id FROM Category WHERE Name = @Name";
                int? categoryId = db.QueryFirstOrDefault<int?>(categoryQuery, new { Name = categoryName });

                if (categoryId != null)
                {
                    Console.WriteLine($"Section '{categoryName}' already exists");
                }
                else
                {
                    string insertCategoryQuery = "INSERT INTO Category (Name) OUTPUT INSERTED.Id VALUES (@Name)";
                    categoryId = db.ExecuteScalar<int>(insertCategoryQuery, new { Name = categoryName });
                    Console.WriteLine($"New category added: {categoryName}");
                }
            }
        }
        static void InsertPromotion()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.Write("Input promotion name: ");
                string promotionName = Console.ReadLine();

                Console.Write("Input start time: ");
                DateTime startTime = DateTime.Parse(Console.ReadLine());

                Console.Write("Input end time: ");
                DateTime endTime = DateTime.Parse(Console.ReadLine());

                Console.Write("Input country name: ");
                string countryName = Console.ReadLine();

                string countryQuery = "SELECT Id FROM Country WHERE Name = @Name";
                int? countryId = db.QueryFirstOrDefault<int?>(countryQuery, new { Name = countryName });

                if (countryId == null)
                {
                    string insertCountryQuery = "INSERT INTO Country (Name, CityId) OUTPUT INSERTED.Id VALUES (@Name, 1)";
                    countryId = db.ExecuteScalar<int>(insertCountryQuery, new { Name = countryName });
                    Console.WriteLine($"New country added: {countryName} (ID: {countryId})");
                }

                string promoQuery = "SELECT Id FROM Promotions WHERE Name = @Name";
                int? promoId = db.QueryFirstOrDefault<int?>(promoQuery, new { Name = promotionName });

                if (promoId != null)
                {
                    Console.WriteLine($"Promotion '{promotionName}' already exists");
                }
                else
                {
                    string insertPromoQuery = @"INSERT INTO Promotions (Name, StartDate, EndDate, CountryId) 
                                                OUTPUT INSERTED.Id 
                                                VALUES (@Name, @StartDate, @EndDate, @CountryId)";
                    promoId = db.ExecuteScalar<int>(insertPromoQuery,
                        new { Name = promotionName, StartDate = startTime, EndDate = endTime, CountryId = countryId });
                    Console.WriteLine($"New promotion added: {promotionName}");
                }
            }
        }
        static void UpdateUser()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.Write("Enter User Name to update: ");
                string userName = Console.ReadLine();

                string userQuery = "SELECT Id FROM [User] WHERE Name = @Name";
                int? userId = db.QueryFirstOrDefault<int?>(userQuery, new { Name = userName });

                if (userId == null)
                {
                    Console.WriteLine("User not found!");
                    return;
                }

                Console.Write("Enter new Surname: ");
                string surname = Console.ReadLine();

                Console.Write("Enter new Email: ");
                string email = Console.ReadLine();

                Console.Write("Enter new Gender: ");
                string gender = Console.ReadLine();

                Console.Write("Enter new Birth Date: ");
                DateTime birthDate = DateTime.Parse(Console.ReadLine());

                Console.Write("Enter Country Name: ");
                string countryName = Console.ReadLine();

                string countryQuery = "SELECT Id FROM Country WHERE Name = @Name";
                int? countryId = db.QueryFirstOrDefault<int?>(countryQuery, new { Name = countryName });

                if (countryId == null)
                {
                    string insertCountryQuery = "INSERT INTO Country (Name, CityId) OUTPUT INSERTED.Id VALUES (@Name, 1)";
                    countryId = db.ExecuteScalar<int>(insertCountryQuery, new { Name = countryName });
                }

                string updateQuery = @" UPDATE [User]
                                    SET Surname = @Surname, Email = @Email, 
                                    Gender = @Gender, BirthDate = @BirthDate, CountryId = @CountryId
                                    WHERE Name = @UserName";
                db.Execute(updateQuery, new { UserName = userName, Surname = surname, Email = email, Gender = gender, BirthDate = birthDate, CountryId = countryId });

                Console.WriteLine("User updated successfully!");
            }
        }
        static void UpdateCountry()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.Write("Enter Country Name to update: ");
                string countryName = Console.ReadLine();

                string countryQuery = "SELECT Id FROM Country WHERE Name = @Name";
                int? countryId = db.QueryFirstOrDefault<int?>(countryQuery, new { Name = countryName });

                if (countryId == null)
                {
                    Console.WriteLine("Country not found!");
                    return;
                }

                Console.Write("Enter new Country Name: ");
                string newName = Console.ReadLine();

                Console.Write("Enter City Name: ");
                string cityName = Console.ReadLine();

                string cityQuery = "SELECT Id FROM City WHERE Name = @Name";
                int? cityId = db.QueryFirstOrDefault<int?>(cityQuery, new { Name = cityName });

                if (cityId == null)
                {
                    string insertCityQuery = "INSERT INTO City (Name) OUTPUT INSERTED.Id VALUES (@Name)";
                    cityId = db.ExecuteScalar<int>(insertCityQuery, new { Name = cityName });
                }

                string updateQuery = "UPDATE Country SET Name = @NewName, CityId = @CityId WHERE Name = @OldName";
                db.Execute(updateQuery, new { OldName = countryName, NewName = newName, CityId = cityId });

                Console.WriteLine("Country updated successfully!");
            }
        }
        static void UpdateCity()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.Write("Enter City Name to update: ");
                string cityName = Console.ReadLine();

                string cityQuery = "SELECT Id FROM City WHERE Name = @Name";
                int? cityId = db.QueryFirstOrDefault<int?>(cityQuery, new { Name = cityName });

                if (cityId == null)
                {
                    Console.WriteLine("City not found!");
                    return;
                }

                Console.Write("Enter new City Name: ");
                string newName = Console.ReadLine();

                string updateQuery = "UPDATE City SET Name = @NewName WHERE Name = @OldName";
                db.Execute(updateQuery, new { OldName = cityName, NewName = newName });

                Console.WriteLine("City updated successfully!");
            }
        }
        static void UpdateCategory()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.Write("Enter Category Name to update: ");
                string categoryName = Console.ReadLine();

                string categoryQuery = "SELECT Id FROM Category WHERE Name = @Name";
                int? categoryId = db.QueryFirstOrDefault<int?>(categoryQuery, new { Name = categoryName });

                if (categoryId == null)
                {
                    Console.WriteLine("Category not found!");
                    return;
                }

                Console.Write("Enter new Category Name: ");
                string newName = Console.ReadLine();

                string updateQuery = "UPDATE Category SET Name = @NewName WHERE Name = @OldName";
                db.Execute(updateQuery, new { OldName = categoryName, NewName = newName });

                Console.WriteLine("Category updated successfully!");
            }
        }
        static void UpdatePromotion()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.Write("Enter Promotion Name to update: ");
                string promoName = Console.ReadLine();

                string promoQuery = "SELECT Id FROM Promotions WHERE Name = @Name";
                int? promoId = db.QueryFirstOrDefault<int?>(promoQuery, new { Name = promoName });

                if (promoId == null)
                {
                    Console.WriteLine("Promotion not found!");
                    return;
                }

                Console.Write("Enter new Promotion Name: ");
                string newName = Console.ReadLine();

                Console.Write("Enter new Start Time: ");
                DateTime startTime = DateTime.Parse(Console.ReadLine());

                Console.Write("Enter new End Time: ");
                DateTime endTime = DateTime.Parse(Console.ReadLine());

                Console.Write("Enter new Country Name: ");
                string countryName = Console.ReadLine();

                string countryQuery = "SELECT Id FROM Country WHERE Name = @Name";
                int? countryId = db.QueryFirstOrDefault<int?>(countryQuery, new { Name = countryName });

                if (countryId == null)
                {
                    string insertCountryQuery = "INSERT INTO Country (Name, CityId) OUTPUT INSERTED.Id VALUES (@Name, 1)";
                    countryId = db.ExecuteScalar<int>(insertCountryQuery, new { Name = countryName });
                }

                string updateQuery = @"UPDATE Promotions
                                    SET Name = @NewName, StartTime = @StartTime, EndTime = @EndTime, CountryId = @CountryId
                                    WHERE Name = @OldName";
                db.Execute(updateQuery, new { OldName = promoName, NewName = newName, StartTime = startTime, EndTime = endTime, CountryId = countryId });

                Console.WriteLine("Promotion updated successfully!");
            }
        }
        static void DeleteUser()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.Write("Enter User Name to delete: ");
                string userName = Console.ReadLine();

                string userQuery = "SELECT Id FROM [User] WHERE Name = @Name";
                int? userId = db.QueryFirstOrDefault<int?>(userQuery, new { Name = userName });

                if (userId == null)
                {
                    Console.WriteLine("User not found!");
                    return;
                }

                string deleteQuery = "DELETE FROM [User] WHERE Name = @Name";
                db.Execute(deleteQuery, new { Name = userName });

                Console.WriteLine("User deleted successfully!");
            }
        }
        static void DeleteCountry()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.Write("Enter Country Name to delete: ");
                string countryName = Console.ReadLine();

                string countryQuery = "SELECT Id FROM Country WHERE Name = @Name";
                int? countryId = db.QueryFirstOrDefault<int?>(countryQuery, new { Name = countryName });

                if (countryId == null)
                {
                    Console.WriteLine("Country not found!");
                    return;
                }

                string deleteQuery = "DELETE FROM Country WHERE Name = @Name";
                db.Execute(deleteQuery, new { Name = countryName });

                Console.WriteLine("Country deleted successfully!");
            }
        }
        static void DeleteCity()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.Write("Enter City Name to delete: ");
                string cityName = Console.ReadLine();

                string cityQuery = "SELECT Id FROM City WHERE Name = @Name";
                int? cityId = db.QueryFirstOrDefault<int?>(cityQuery, new { Name = cityName });

                if (cityId == null)
                {
                    Console.WriteLine("City not found!");
                    return;
                }

                string deleteQuery = "DELETE FROM City WHERE Name = @Name";
                db.Execute(deleteQuery, new { Name = cityName });

                Console.WriteLine("City deleted successfully!");
            }
        }
        static void DeleteCategory()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.Write("Enter Category Name to delete: ");
                string categoryName = Console.ReadLine();

                string categoryQuery = "SELECT Id FROM Category WHERE Name = @Name";
                int? categoryId = db.QueryFirstOrDefault<int?>(categoryQuery, new { Name = categoryName });

                if (categoryId == null)
                {
                    Console.WriteLine("Category not found!");
                    return;
                }

                string deleteQuery = "DELETE FROM Category WHERE Name = @Name";
                db.Execute(deleteQuery, new { Name = categoryName });

                Console.WriteLine("Category deleted successfully!");
            }
        }
        static void DeletePromotion()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.Write("Enter Promotion Name to delete: ");
                string promotionName = Console.ReadLine();

                string promotionQuery = "SELECT Id FROM Promotions WHERE Name = @Name";
                int? promotionId = db.QueryFirstOrDefault<int?>(promotionQuery, new { Name = promotionName });

                if (promotionId == null)
                {
                    Console.WriteLine("Promotion not found!");
                    return;
                }

                string deleteQuery = "DELETE FROM Promotions WHERE Name = @Name";
                db.Execute(deleteQuery, new { Name = promotionName });

                Console.WriteLine("Promotion deleted successfully!");
            }
        }
        static void ShowCitiesByCountry()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Console.Write("Enter Country Name: ");
                string countryName = Console.ReadLine();

                string countryQuery = "SELECT Id FROM Country WHERE Name = @Name";
                int? countryId = db.QueryFirstOrDefault<int?>(countryQuery, new { Name = countryName });

                if (countryId == null)
                {
                    Console.WriteLine("Country not found!");
                    return;
                }

                string citiesQuery = "SELECT Name FROM City WHERE Id IN (SELECT CityId FROM Country WHERE Id = @CountryId)";
                var cities = db.Query<string>(citiesQuery, new { CountryId = countryId });

                if (!cities.Any())
                {
                    Console.WriteLine($"No cities found for {countryName}.");
                    return;
                }

                Console.WriteLine($"Cities in {countryName}:");
                foreach (var city in cities)
                {
                    Console.WriteLine($"- {city}");
                }
            }
        }



    }
}
