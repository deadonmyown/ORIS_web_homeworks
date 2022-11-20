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
}