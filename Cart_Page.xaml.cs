using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Data;

namespace Kashapov;

public partial class Cart_Page : ContentPage
{
    private const string ConnectionString = "Server=server269.hosting.reg.ru;Database=u2917647_default;User ID=u2917647_default;Password=1tB6J7OD3cmt3JD1;Charset=utf8mb4;";
    public ObservableCollection<CartItem> CartItems { get; set; }

    string login_page;

    public Cart_Page(string login)
    {
        InitializeComponent();
        UserNameLabel.Text = login;
        login_page = login;
        CartItems = new ObservableCollection<CartItem>();
        CartItemsCollectionView.ItemsSource = CartItems;
        LoadCartData(login);
    }

    // Структура данных для корзины
    // Структура данных для корзины
    public class CartItem
    {

        public string NameProduct { get; set; }
        public int CountProduct { get; set; }
        public string ProductImage { get; set; } 
    }

    // Загрузка данных из базы данных
    private async void LoadCartData(string UserName)
    {
        string userName = UserName; // Получить имя пользователя, например, из профиля

        using (var connection = new MySqlConnection(ConnectionString))
        {
            await connection.OpenAsync();
            var query = $"SELECT NameProduct, CountProduct, Photo_Number FROM Cart WHERE UserName = @UserName";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserName", userName);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var cartItem = new CartItem
                        {
                            NameProduct = reader.GetString("NameProduct"),
                            CountProduct = reader.GetInt32("CountProduct"),
                            ProductImage = GetProductImage(reader.GetInt32("Photo_Number"))
                        };
                        CartItems.Add(cartItem);
                    }
                }
            }
        }
    }
    private string GetProductImage(int photoNumber)
    {
        switch (photoNumber)
        {
            case 2:
                return "image1.jpg"; // Путь к первому изображению
            case 1:
                return "image2.jpeg"; // Путь ко второму изображению
            case 3:
                return "image3.png"; // Путь к третьему изображению
                                     // Добавьте больше кейсов, если необходимо
            default:
                return "placeholder.png"; // Изображение по умолчанию
        }
    }


    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Cart_Page(login_page));
    }

    private void ImageButton_Clicked_1(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ItemMain(login_page));
    }

    private void ImageButton_Clicked_2(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Lk_Page(login_page));
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is CartItem cartItem)
        {
            bool isConfirmed = await DisplayAlert("Подтверждение", $"Удалить {cartItem.NameProduct} из корзины?", "Да", "Нет");
            if (isConfirmed)
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    var query = "DELETE FROM Cart WHERE UserName = @UserName AND NameProduct = @NameProduct";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", login_page);
                        command.Parameters.AddWithValue("@NameProduct", cartItem.NameProduct);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                CartItems.Remove(cartItem); // Удаляем товар из списка
            }
        }
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        if (CartItems.Count == 0)
        {
            await DisplayAlert("Ошибка", "В корзине нет товаров для оформления заказа.", "OK");
            return;
        }

        decimal totalCost = 0;
        foreach (var item in CartItems)
        {
            // Подсчет общей стоимости. Здесь предположим, что цена за единицу товара доступна в базе данных.
            using (var connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT Cost FROM Products WHERE Name = @NameProduct";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NameProduct", item.NameProduct);
                    var result = await command.ExecuteScalarAsync();
                    if (result != null)
                    {
                        totalCost += Convert.ToDecimal(result) * item.CountProduct;
                    }
                }
            }
        }

        using (var connection = new MySqlConnection(ConnectionString))
        {
            await connection.OpenAsync();
            var query = "INSERT INTO OrderUser (UserName, OrderCost, OrderData) VALUES (@UserName, @OrderCost, @OrderData)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserName", login_page);
                command.Parameters.AddWithValue("@OrderCost", totalCost);
                command.Parameters.AddWithValue("@OrderData", DateTime.Now);
                await command.ExecuteNonQueryAsync();
            }

            // После оформления заказа очищаем корзину
            var deleteCartQuery = "DELETE FROM Cart WHERE UserName = @UserName";
            using (var deleteCommand = new MySqlCommand(deleteCartQuery, connection))
            {
                deleteCommand.Parameters.AddWithValue("@UserName", login_page);
                await deleteCommand.ExecuteNonQueryAsync();
            }
        }

        CartItems.Clear();
        await DisplayAlert("Успех", "Заказ оформлен успешно!", "OK");
    }
}
