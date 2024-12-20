using Kashapov;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System.Collections.ObjectModel;
using System.Data;

public class DatabaseService
{
    private const string ConnectionString = "Server=server269.hosting.reg.ru;Database=u2917647_default;User ID=u2917647_default;Password=1tB6J7OD3cmt3JD1;Charset=utf8mb4;";

    public async Task<ObservableCollection<OrderUser>> GetOrdersAsync()
    {
        var orders = new ObservableCollection<OrderUser>();

        using (var connection = new MySqlConnection(ConnectionString))
        {
            await connection.OpenAsync();

            string query = "SELECT UserName, OrderID, OrderCost, OrderData FROM OrderUser";
            using (var command = new MySqlCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    orders.Add(new OrderUser
                    {
                        UserName = reader.GetString("UserName"),
                        OrderID = reader.GetInt32("OrderID"),
                        OrderCost = reader.GetDecimal("OrderCost"),
                        OrderDate = reader.GetDateTime("OrderData")
                    });
                }
            }
        }

        return orders;
    }
}
