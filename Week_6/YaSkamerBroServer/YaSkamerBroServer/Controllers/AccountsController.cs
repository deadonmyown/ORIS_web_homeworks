using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using YaSkamerBroServer.Model;

namespace YaSkamerBroServer;


[HttpController("accounts")]
public class AccountsController
{
    [HttpGET]
    public List<Account> GetAccounts()
    {
        List<Account> accounts = new List<Account>();

        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ServerDB;Integrated Security=True;";

        string sqlExpression = "SELECT * FROM Accounts";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", reader.GetName(0), reader.GetName(1), reader.GetName(2), reader.GetName(3), reader.GetName(4));

                while (reader.Read())
                {
                    accounts.Add(new Account() 
                    { 
                        Id = reader.GetInt32(0), 
                        Name = reader.GetString(1), 
                        Email = reader.GetString(2), 
                        Phone = reader.GetString(3), 
                        Password = reader.GetString(4) 
                    });
                }
            }

            reader.Close();
        }
        return accounts;
    }
    
    [HttpGET("{id:int}")]
    public Account GetAccountById(int id)
    {
        return GetAccounts()[id];
    }

    [HttpPOST]
    public string SaveAccount(string body, string name, string password)
    {
        if (body == name || body == password)
        {
            string[] parsedBody = body.ParseAsQueryToArray();
            if (parsedBody.Length == 2)
                (name, password) = (parsedBody[0], parsedBody[1]);
        }

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
            return "Wrong body type";

        Console.WriteLine(body + "  " + name + "  " + password);

        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ServerDB;Integrated Security=True;";
        
        string sqlExpression = $"INSERT INTO Accounts (Name, Email, Phone, Password) VALUES ('{name}', '', '', '{password}')";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            int changes = command.ExecuteNonQuery();
            Console.WriteLine($"add {changes} account");
        }

        return name + "  " + password;
    }
    
    //Get (/accounts/) - список акков в формате жсон
    //Get /accounts/{id} - возвращает акк по индексу
    //Post /accounts/ - добавляет инфу на сервер
}