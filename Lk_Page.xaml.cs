using Mysqlx.Crud;
using System.Collections.ObjectModel;

namespace Kashapov;

public partial class Lk_Page : ContentPage
{
    string login_page;
    public ObservableCollection<OrderUser> Orders { get; set; }
    private DatabaseService databaseService;
    public Lk_Page(string login)
	{
		InitializeComponent();
        UserNameLabel.Text = login;
        login_page =login;
        databaseService = new DatabaseService();
        Orders = new ObservableCollection<OrderUser>();
        BindingContext = this;

        LoadOrders();

    }
    private async void LoadOrders()
    {
        try
        {
            var ordersFromDb = await databaseService.GetOrdersAsync();
            foreach (var order in ordersFromDb)
            {
                Orders.Add(order);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось загрузить заказы: {ex.Message}", "OK");
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

    private void Button_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage());
    }
}