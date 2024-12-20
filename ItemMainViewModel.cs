using Kashapov;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

public class ItemMainViewModel
{
    public ObservableCollection<Product> Products { get; set; }

    public ICommand AddToCartCommand { get; set; }

    private string connectionString = "Server=server269.hosting.reg.ru;Database=u2917647_default;User ID=u2917647_default;Password=1tB6J7OD3cmt3JD1;Charset=utf8mb4;";
    private readonly Func<string> _getUserName;

    public ItemMainViewModel(Func<string> getUserName)
    {
        _getUserName = getUserName;

        Products = new ObservableCollection<Product>();
        AddToCartCommand = new Command<Product>(OnAddToCart);

        // Загрузка данных из базы данных
        LoadProductsFromDatabase();
    }

    public ItemMainViewModel()
    {
        Products = new ObservableCollection<Product>();
        AddToCartCommand = new Command<Product>(OnAddToCart);

        // Загрузка данных из базы данных
        LoadProductsFromDatabase();
    }
    private string GetImagePath(int photoNumber)
    {
        return photoNumber switch
        {
            2 => "image1.png",
            1 => "image2.png",
            3 => "image3.png",
            _ => "placeholder.png", // Изображение по умолчанию
        };
    }
    private void LoadProductsFromDatabase()
    {
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT Name, Cost, Product_Photo_number FROM Products"; // Замените "Products" на название вашей таблицы

                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Products.Add(new Product
                        {
                            Name = reader.GetString("Name"),
                            Price = reader.GetDecimal("Cost"),
                            Photo_Number = reader.GetInt32("Product_Photo_number")
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            App.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось загрузить данные: {ex.Message}", "OK");
        }
    }

    private void OnAddToCart(Product product)
    {
        string currentUserName = _getUserName();
        Debug.WriteLine($"Попытка добавить в корзину: {product.Name} для пользователя {currentUserName}");

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                Debug.WriteLine("Подключение к базе данных установлено для добавления в корзину.");

                string selectQuery = "SELECT CountProduct FROM Cart WHERE UserName = @UserName AND NameProduct = @NameProduct";
                using (var selectCommand = new MySqlCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@UserName", currentUserName);
                    selectCommand.Parameters.AddWithValue("@NameProduct", product.Name);

                    var result = selectCommand.ExecuteScalar();
                    if (result != null)
                    {
                        // Обновляем количество
                        int count = Convert.ToInt32(result);
                        string updateQuery = "UPDATE Cart SET CountProduct = @CountProduct WHERE UserName = @UserName AND NameProduct = @NameProduct";
                        using (var updateCommand = new MySqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@CountProduct", count + 1);
                            updateCommand.Parameters.AddWithValue("@UserName", currentUserName);
                            updateCommand.Parameters.AddWithValue("@NameProduct", product.Name);

                            updateCommand.ExecuteNonQuery();
                            Debug.WriteLine($"Обновлено количество товара: {product.Name}, Новое количество: {count + 1}");
                        }
                    }
                    else
                    {
                        // Добавляем новый товар в корзину
                        string insertQuery = "INSERT INTO Cart (UserName, NameProduct, CountProduct,Photo_Number) VALUES (@UserName, @NameProduct, @CountProduct, @Photo_Number)";
                        using (var insertCommand = new MySqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@UserName", currentUserName);
                            insertCommand.Parameters.AddWithValue("@NameProduct", product.Name);
                            insertCommand.Parameters.AddWithValue("@CountProduct", 1);
                            insertCommand.Parameters.AddWithValue("@Photo_Number", product.Photo_Number);

                            insertCommand.ExecuteNonQuery();
                            Debug.WriteLine($"Товар добавлен в корзину: {product.Name}, Количество: 1");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка добавления товара в корзину: {ex.Message}");
        }
    }
}
