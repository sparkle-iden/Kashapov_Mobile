using MySql.Data.MySqlClient;

namespace Kashapov
{
    public partial class MainPage : ContentPage
    {
        

        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Registr());
        }

        private async void Button_Clicked_1(object sender, EventArgs e) { 
        
                if (Login_entry.Text != null)
                {
                    if (Password_entry.Text != null)
                    {
                        string connectionString = "Server=server269.hosting.reg.ru;Database=u2917647_default;User ID=u2917647_default;Password=1tB6J7OD3cmt3JD1;Charset=utf8mb4;";
                        string query = "SELECT Password FROM User WHERE UserName = @UserName";
                        string Password = Password_entry.Text;
                        string login = Login_entry.Text;
                        string PasswordUser = "0";
                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            try
                            {
                                await connection.OpenAsync();
                                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                {
                                    cmd.Parameters.AddWithValue("@UserName", login);
                                    object result = await cmd.ExecuteScalarAsync();
                                    if (result != null)
                                    {
                                        PasswordUser = Convert.ToString(result);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                        }
                        Console.WriteLine(PasswordUser);
                        if (Password == PasswordUser)
                        {
                            login = Login_entry.Text;
                            await Navigation.PushAsync(new ItemMain(login));
                        }
                        else
                        {
                            await DisplayAlert("Ошибка", "Неправильный логин или пин-код.", "OK");
                        }

                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Введите пин-код.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка", "Введите имя", "OK");
                }

            
        }
    }

}
