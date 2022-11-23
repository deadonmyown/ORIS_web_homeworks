using MyORM;
using MyORM.Model;
using System.Net;

namespace YaSkamerBroServer.Controllers;


[HttpController("accounts")]
public class AccountsController: Controller
{
    [HttpGET]
    public List<Account> GetAccounts()
    {
        if(!HttpContext.Request.Cookies.CheckSessionId())
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return null;
        }
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

    [HttpGET("info")]
    public Account GetAccountInfo()
    {
        int? id = HttpContext.Request.Cookies.CheckAuthorizedAccount();
        if (!id.HasValue)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return null;
        }
        var dao =
            new AccountDao(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ServerDB;Integrated Security=True;");
        return dao.Select(id.Value);
    }


    [HttpPOST]
    public string Login(string body, string login, string password)
    {
        if (body == login || body == password)
        {
            string[] parsedBody = body.ParseAsQueryToArray();
            if (parsedBody.Length == 2)
                (login, password) = (parsedBody[0], parsedBody[1]);
        }

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            return "Wrong body type";

        Console.WriteLine(body + "  " + login + "  " + password);

        Console.WriteLine($"Check this method: login: {login} password: {password}");
        var dao = new AccountDao(
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ServerDB;Integrated Security=True;");
        var account = dao.Select().FirstOrDefault(acc => acc.Name == login && acc.Password == password);
        if (account != null)
        {
            var guid = Guid.NewGuid();
            var session = new Session(guid, account.Id, account.Email, DateTime.Now);
            SessionManager.CreateOrGetSession(guid, () => session);
            SessionManager.CheckSession(guid);
            //HttpContext.Response.AppendHeader("Set-Cookie", $"SessionId=IsAuthorize: true, Id={account.Id}");
            HttpContext.Response.AppendHeader("Set-Cookie", $"SessionId={session.Id}");
            return $"welcome {account.Name}";
        }
        return "account not found";
    }

    [HttpPOST("saveAccount")]
    public string SaveAccount(string name, string password)
    {
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

    #region Test methods
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
    #endregion
}