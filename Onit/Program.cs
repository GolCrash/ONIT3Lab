using Microsoft.EntityFrameworkCore;
using Onit;
using Onit.models;

class Program
{
    static void Main(string[] args)
    {
        AppDbContext db = null;

        while (true)
        {
            try
            {
                db = new AppDbContext();
                db.Database.EnsureCreated();
                break;
            }
            catch
            {
                Console.WriteLine("Ждем запуск SQL Server...");
                Thread.Sleep(5000);
            }
        }

        using (db)
        {

            while (true)
            {
                Console.WriteLine("\n--- Облачный гейминг ---");
                Console.WriteLine("1. Добавить сервер");
                Console.WriteLine("2. Добавить пользователя");
                Console.WriteLine("3. Подключить пользователя к серверу");
                Console.WriteLine("4. Показать всех пользователей с их серверами");
                Console.WriteLine("5. Показать все сервера");
                Console.WriteLine("6. Выход");
                Console.Write("Выберите действие: ");
                var choice = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(choice))
                {
                    Thread.Sleep(500);
                    continue;
                }

                switch (choice)
                {
                    case "1":
                        AddServer(db);
                        break;
                    case "2":
                        AddUser(db);
                        break;
                    case "3":
                        ConnectUserToServer(db);
                        break;
                    case "4":
                        ShowData(db);
                        break;
                    case "5":
                        ShowServers(db);
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Неверный ввод. Попробуйте снова.");
                        break;
                }
            }
        }
    }

    static void AddServer(AppDbContext db)
    {
        Console.Write("Введите игры (через запятую): ");
        var games = Console.ReadLine();
        Console.Write("Введите количество игроков: ");
        var players = int.Parse(Console.ReadLine());

        var server = new Server { Games = games, Players = players };
        db.Servers.Add(server);
        db.SaveChanges();
        Console.WriteLine("Сервер добавлен.");
    }

    static void AddUser(AppDbContext db)
    {
        Console.Write("Введите email: ");
        var email = Console.ReadLine();
        Console.Write("Введите имя: ");
        var name = Console.ReadLine();

        var user = new User { Email = email, Name = name };
        db.Users.Add(user);
        db.SaveChanges();
        Console.WriteLine("Пользователь добавлен.");
    }

    static void ConnectUserToServer(AppDbContext db)
    {
        var users = db.Users.ToList();

        if (!users.Any())
        {
            Console.WriteLine("Нет пользователей. Сначала добавьте пользователя.");
            return;
        }

        Console.WriteLine("Пользователи:");
        foreach (var u in users)
            Console.WriteLine($"{u.Id}: {u.Name} ({u.Email})");

        Console.Write("Введите ID пользователя: ");
        var userId = int.Parse(Console.ReadLine());

        var servers = db.Servers.ToList();

        if (!servers.Any())
        {
            Console.WriteLine("Нет серверов. Сначала добавьте сервер.");
            return;
        }

        Console.WriteLine("Серверы:");
        foreach (var s in servers)
            Console.WriteLine($"{s.Id}: {s.Games} (свободных слотов: {s.Players})");

        Console.Write("Введите ID сервера: ");
        var serverId = int.Parse(Console.ReadLine());

        var exists = db.UserServers.Any(us => us.UserId == userId && us.ServerId == serverId);

        if (exists)
        {
            Console.WriteLine("Этот пользователь уже подключён к данному серверу.");
            return;
        }

        var server = db.Servers.Find(serverId);

        if (server.Players <= 0)
        {
            Console.WriteLine("На сервере нет свободных мест.");
            return;
        }

        server.Players--;

        var userServer = new UserServer { UserId = userId, ServerId = serverId };
        db.UserServers.Add(userServer);
        db.SaveChanges();
        Console.WriteLine("Пользователь подключён к серверу.");
    }

    static void ShowData(AppDbContext db)
    {
        var users = db.Users
            .Include(u => u.UserServers)
            .ThenInclude(us => us.Server)
            .ToList();

        if (!users.Any())
        {
            Console.WriteLine("Нет пользователей.");
            return;
        }

        foreach (var user in users)
        {
            Console.WriteLine($"\nПользователь: {user.Name} (Email: {user.Email})");
            if (user.UserServers.Any())
            {
                Console.WriteLine("Подключён к серверам:");
                foreach (var us in user.UserServers)
                {
                    Console.WriteLine($"  - Сервер ID {us.ServerId}: игры {us.Server.Games}, игроков {us.Server.Players}");
                }
            }
            else
            {
                Console.WriteLine("Не подключён ни к одному серверу.");
            }
        }
    }

    static void ShowServers(AppDbContext db)
    {
        var servers = db.Servers.ToList();

        if (!servers.Any())
        {
            Console.WriteLine("Серверов нет.");
            return;
        }

        foreach (var server in servers)
        {
            Console.WriteLine($"Сервер: {server.Id} (Игры: {server.Games}), свободных мест {server.Players}");
        }
    }
}