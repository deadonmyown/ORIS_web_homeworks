using MyORM;
using MyORM.Model;

namespace YaSkamerBroServer.Controllers;


[HttpController("accounts")]
public class AccountsController
{
    [HttpGET]
    public List<Account> GetAccounts()
    {
        var dao =
            new AccountDao(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ServerDB;Integrated Security=True;");
        return dao.Select();
    }
    
    [HttpGET("{id:int}")]
    public Account GetAccountById(int id)
    {
        var dao =
            new AccountDao(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ServerDB;Integrated Security=True;");
        return dao.Select(id);
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

        var dao =
            new AccountDao(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ServerDB;Integrated Security=True;");
        dao.Insert(name, " ", " ", password);
        
        return "add account";
    }

    [HttpDELETE("delete")]
    public string DeleteAllAccounts()
    {
        var dao =
            new AccountDao(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ServerDB;Integrated Security=True;");
        dao.Delete();

        return "delete all accounts";
    }
    
    [HttpDELETE("delete/{id:int}")]
    public string DeleteAccount(int id)
    {
        var dao =
            new AccountDao(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ServerDB;Integrated Security=True;");
        dao.Delete(id);

        return $"delete account by id = {id}";
    }


    //http://localhost:1337/accounts/login?login=oyda&password=zae**l -> true
    [HttpPOST("login")]
    public bool Login(string login, string password)
    {
        Console.WriteLine($"Check this method: login: {login} password: {password}");
        var dao = new AccountDao(
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ServerDB;Integrated Security=True;");
        return dao.Select().Any(acc => acc.Name == login && acc.Password == password);
    }

    //http://localhost:1337/accounts/login?email=aboba&password=123 -> false
    //http://localhost:1337/accounts/login?email=aboba&password=prvi -> true
    [HttpGET("login")]
    public bool GetLogin(string email, string password)
    {
        Console.WriteLine($"Check this method: email: {email} password: {password}");
        var dao = new AccountDao(
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ServerDB;Integrated Security=True;");
        return dao.Select().Any(acc => acc.Email == email && acc.Password == password);
    }

    [HttpGET("update")]
    public string UpdateAccountInfo(int id, string tableName, string newValue)
    {
        var dao = new AccountDao(
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ServerDB;Integrated Security=True;");
        dao.Update(id, tableName, newValue);
        return $"update {tableName} with this value - {newValue}";
    }


    //http://localhost:1337/accounts/testInsert?name=aboba&email=aobba@mail.ru&phone=1337228&password=
    [HttpGET("createAccount")]
    public string CreateAccount(string name, string email, string phone, string password)
    {
        var dao = new AccountDao(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ServerDB;Integrated Security=True;");
        dao.Insert(new Account() { Name = name, Email = email, Phone = phone, Password = password });
        return $"{name}, {email}, {phone}, {password}";
    }
}