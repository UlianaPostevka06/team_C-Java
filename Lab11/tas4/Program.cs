using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Lab11.Task4;
// Завдання 4 — PROXY (завантаження локацій)
//  як розпізнати шаблон за описом):
//-«Об'єкт ДОРОГО створювати, а потрібен він не одразу» -> віртуальний Proxy (ледаче завантаження): створюємо справжній об'єкт лише при першому справжньому зверненні.
//-«Доступ до об'єкта треба ПЕРЕВІРЯТИ» (тут — рівень гравця) -> захисний Proxy: пропускає лише тих, кому дозволено.
//-Обидва випадки вирішує ОДИН замісник з ТИМ САМИМ інтерфейсом.
//
//ролі в шаблоні:
//ILocation       — Subject (спільний інтерфейс замісника і справжнього об'єкта)
//RealLocation    — RealSubject (справжня локація з дорогим завантаженням мапи)
//LocationProxy   — Proxy (замісник: перевіряє рівень, завантажує мапу ліниво)
//GameServer      — Client (працює з ILocation і НЕ знає, чи мапу завантажено)
//
// де в журналі видно, хто що робить:
//[Proxy:Чорний ліс]        — замісник: перевіряє рівень, вирішує, чи завантажувати;
//[RealLocation:Чорний ліс] — справжній об'єкт: сам завантажує мапу і пускає гравця.
public static class Logger
{
    public static void Log(string source, string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{source}] {message}");
    }
}

// Гравець
public class Player
{
    public string Name { get; }
    public int Level { get; }

    public Player(string name, int level)
    {
        Name = name;
        Level = level;
    }
}

//  SUBJECT: спільний інтерфейс. Сервер знає ЛИШЕ його.
public interface ILocation
{
    string Name { get; }
    bool Enter(Player player);   // true — гравця впустили
}

//  REAL SUBJECT: справжня локація. Створення ДОРОГЕ (велика мапа).
public class RealLocation : ILocation
{
    // Скільки мап реально завантажено (статичний лічильник для демо)
    public static int LoadedCount { get; private set; }

    public string Name { get; }

    public RealLocation(string name)
    {
        Name = name;
        Logger.Log($"RealLocation:{name}", "Завантаження великої мапи...");
        Thread.Sleep(500);   // імітація довгого завантаження
        LoadedCount++;
        Logger.Log($"RealLocation:{name}", "Мапу завантажено");
    }

    public bool Enter(Player player)
    {
        Logger.Log($"RealLocation:{Name}",
            $"Гравець {player.Name} (рівень {player.Level}) увійшов у локацію");
        return true;
    }
}
//  PROXY: замісник. Стоїть на місці справжньої локації.
//два завдання в одному класі:
//1) захисне: перевірка рівня гравця (до завантаження мапи!);
//2) віртуальне: справжню локацію створюємо ЛИШЕ при першому дозволеному вході.
public class LocationProxy : ILocation
{
    public string Name { get; }
    private readonly int _minLevel;
    private RealLocation? _real;   // null = мапу ще не завантажено

    public LocationProxy(string name, int minLevel)
    {
        Name = name;
        _minLevel = minLevel;
        // ШПОРА: у конструкторі НЕ створюємо RealLocation — тому при старті
        // сервера жодна мапа не завантажується.
    }

    public bool Enter(Player player)
    {
        Logger.Log($"Proxy:{Name}",
            $"Запит входу від {player.Name} (рівень {player.Level}), вимога: від {_minLevel} рівня");

        // 1. ЗАХИСНИЙ proxy: перевірка рівня. Мапа при відмові НЕ завантажується.
        if (player.Level < _minLevel)
        {
            Logger.Log($"Proxy:{Name}", $"ВІДМОВА: Локація доступна з {_minLevel} рівня");
            return false;
        }

        // 2. ВІРТУАЛЬНИЙ proxy: ледаче створення справжнього об'єкта
        if (_real == null)
        {
            Logger.Log($"Proxy:{Name}", "Мапу ще не завантажено — створюю справжню локацію");
            _real = new RealLocation(Name);
        }
        else
        {
            Logger.Log($"Proxy:{Name}", "Мапу вже завантажено — повторного завантаження немає");
        }

        // 3. Делегуємо справжній дії справжньому об'єкту
        return _real.Enter(player);
    }
}

//  CLIENT: сервер. Працює з ILocation і не знає, чи це proxy, чи завантажено мапу.
public class GameServer
{
    private readonly Dictionary<string, ILocation> _locations = new();

    public void AddLocation(ILocation location)
    {
        _locations[location.Name] = location;
        Logger.Log("Server", $"Локацію «{location.Name}» зареєстровано");
    }

    public int LocationCount => _locations.Count;

    public bool Enter(Player player, string locationName)
    {
        if (!_locations.TryGetValue(locationName, out ILocation? location))
        {
            Logger.Log("Server", $"Локації «{locationName}» не існує");
            return false;
        }

        Logger.Log("Server", $"{player.Name} намагається увійти в «{locationName}»");
        return location.Enter(player);   // той самий виклик для будь-якої локації
    }
}

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;   
        Situation4_Proxy();
    }

    private static void Situation4_Proxy()
    {
        Console.WriteLine("-Завдання 4: завантаження локацій (Proxy)");

        // Крок 1: сервер стартує з 5 локаціями — жодного завантаження мап
        var server = new GameServer();
        server.AddLocation(new LocationProxy("Чорний ліс", 1));
        server.AddLocation(new LocationProxy("Тихе болото", 5));
        server.AddLocation(new LocationProxy("Гірський перевал", 10));
        server.AddLocation(new LocationProxy("Крижані печери", 20));
        server.AddLocation(new LocationProxy("Вогняна пустка", 30));
        Logger.Log("Server",
            $"Сервер стартував: локацій {server.LocationCount}, завантажених мап: {RealLocation.LoadedCount}");

        // Крок 2: гравець 10 рівня входить у «Чорний ліс» — завантаження один раз
        var storm = new Player("Storm", 10);
        server.Enter(storm, "Чорний ліс");

        // Інший гравець входить у ту саму локацію — без повторного завантаження
        var ember = new Player("Ember", 7);
        server.Enter(ember, "Чорний ліс");

        // Крок 3: гравець 3 рівня намагається увійти в «Крижані печери» (від 20 рівня)
        var newbie = new Player("Newbie", 3);
        server.Enter(newbie, "Крижані печери");

        Logger.Log("Server",
            $"Підсумок: завантажено мап {RealLocation.LoadedCount} з {server.LocationCount}");
    }
}