using MySql.Data.MySqlClient;

namespace Kashapov;

public partial class Registr : ContentPage
{
  
    public Registr()
	{
		InitializeComponent();
	}

    private async void CounterBtn_Clicked(object sender, EventArgs e)
    {
       
        string login = Login_entry.Text;
        int password = Convert.ToInt32(Password_entry.Text);
        if (!string.IsNullOrEmpty(login))
        {
            if (password != null)
            {
                // ���������� ������ � ���� ������
                await SaveLoginToDatabaseAsync(login, password);
                Preferences.Set("UserName", login);
                // ������� �� ��������� ��������
            }
            else
            {
                await DisplayAlert("������", "������� ���-���.", "OK");
            }
        }
        else
        {
            await DisplayAlert("������", "������� ���.", "OK");
        }


    }

    private async Task SaveLoginToDatabaseAsync(string login, int password)
    {
        string connectionString = "Server=server269.hosting.reg.ru;Database=u2917647_default;User ID=u2917647_default;Password=1tB6J7OD3cmt3JD1;Charset=utf8mb4;";

        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync(); // ����������� �������� ����������
                try
                {
                    Console.WriteLine("����������� �������!");

                    string query = "INSERT INTO User (UserName, UserMoney, Password) VALUES (@login, 0, @password)";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password);
                        // ����������� ���������� �������
                        await command.ExecuteNonQueryAsync();
                        Console.WriteLine("����� ������� ������� � ���� ������.");
                        await Navigation.PushAsync(new ItemMain(login));
                    }

                }
                catch
                {
                    await DisplayAlert("������", "����� ��� ��� ����������, ������� ������.", "OK");
                }
            }

        }
        catch
        {
            await DisplayAlert("������", "������ �����������.", "OK");
        }
    }

    private async void Return_Button(object sender, EventArgs e)
    {
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
                    await DisplayAlert("������", "������������ ����� ��� ���-���.", "OK");
                }

            }
            else
            {
                await DisplayAlert("������", "������� ���-���.", "OK");
            }
        }
        else
        {
            await DisplayAlert("������", "������� ���", "OK");
        }

    }
}