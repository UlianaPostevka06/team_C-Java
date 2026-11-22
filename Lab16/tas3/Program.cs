using System;
using System.Collections.Generic;
using System.Text;

namespace Lab16.Task3;


//-«Типи об'єктів СТАБІЛЬНІ, а нові ОПЕРАЦІЇ над ними з'являються постійно» -> Visitor.
//-Було: кожна нова операція (вибух, статистика, збереження світу) додавалася методом у ВСІ класи об'єктів, і вони перетворилися на
//звалище не пов'язаної логіки.
//-Стало: кожна операція — ОКРЕМИЙ КЛАС-відвідувач, який містить свою логіку для кожного типу об'єкта. Класи об'єктів не змінюються.

public static class Logger
{
    public static void Log(string source, string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{source}] {message}");
    }
}

//  VISITOR: по одному методу Visit на кожен конкретний тип об'єкта
public interface IGameVisitor
{
    void Visit(Player player);
    void Visit(Monster monster);
    void Visit(Merchant merchant);
    void Visit(Chest chest);
    void Visit(Trap trap);
}

//  ELEMENT: спільна абстракція ігрових об'єктів
//  Класи об'єктів містять ЛИШЕ дані та Accept — жодної логіки
//  операцій. Describe потрібен для журналу «стан до / після».
public interface IGameObject
{
    string Name { get; }
    void Accept(IGameVisitor visitor);
    string Describe();
}

//  CONCRETE ELEMENTS
public class Player : IGameObject
{
    public string Name { get; }
    public int Health { get; set; }
    public int Gold { get; set; }

    public Player(string name, int health, int gold)
    {
        Name = name;
        Health = health;
        Gold = gold;
    }

    public void Accept(IGameVisitor visitor) => visitor.Visit(this);   // double dispatch
    public string Describe() => $"Гравець {Name}: HP={Health}, золото={Gold}";
}

public class Monster : IGameObject
{
    public string Name { get; }
    public int Health { get; set; }
    public bool IsAlive => Health > 0;

    public Monster(string name, int health)
    {
        Name = name;
        Health = health;
    }

    public void Accept(IGameVisitor visitor) => visitor.Visit(this);
    public string Describe() => $"Монстр {Name}: HP={Health}, {(IsAlive ? "живий" : "мертвий")}";
}

public class Merchant : IGameObject
{
    public string Name { get; }
    public int Gold { get; set; }
    public bool HasFled { get; set; }

    public Merchant(string name, int gold)
    {
        Name = name;
        Gold = gold;
    }

    public void Accept(IGameVisitor visitor) => visitor.Visit(this);
    public string Describe() => $"Торговець {Name}: золото={Gold}, {(HasFled ? "утік" : "на місці")}";
}

public class Chest : IGameObject
{
    public string Name { get; }
    public bool IsOpen { get; set; }
    public int Loot { get; set; }

    public Chest(string name, int loot)
    {
        Name = name;
        Loot = loot;
    }

    public void Accept(IGameVisitor visitor) => visitor.Visit(this);
    public string Describe() => $"Скриня {Name}: {(IsOpen ? "відкрита" : "закрита")}, вміст={Loot} зол.";
}

public class Trap : IGameObject
{
    public string Name { get; }
    public int Damage { get; }
    public bool IsTriggered { get; set; }

    public Trap(string name, int damage)
    {
        Name = name;
        Damage = damage;
    }

    public void Accept(IGameVisitor visitor) => visitor.Visit(this);
    public string Describe() => $"Пастка {Name}: урон={Damage}, {(IsTriggered ? "спрацювала" : "активна")}";
}

//  OBJECT STRUCTURE: локація
//  Accept(visitor) — це «один прохід по всій локації» з умови.
//  Локація не знає, яку саме операцію виконує відвідувач.
public class Location
{
    private readonly List<IGameObject> _objects = new();

    public string Name { get; }

    public Location(string name) => Name = name;

    public void Add(IGameObject obj) => _objects.Add(obj);

    public void Accept(IGameVisitor visitor)
    {
        foreach (var obj in _objects)
            obj.Accept(visitor);
    }

    // Стан усіх об'єктів у журнал (для порівняння «до / після»)
    public void PrintState(string title)
    {
        Logger.Log("Локація", $"{title} («{Name}»):");
        foreach (var obj in _objects)
            Logger.Log("Локація", "   " + obj.Describe());
    }
}

//  CONCRETE VISITORS: кожна операція — окремий клас

// Операція 1: обробка вибуху. Кожен тип реагує ПО-РІЗНОМУ.
public class ExplosionVisitor : IGameVisitor
{
    private const string Src = "Вибух";
    private readonly int _power;

    public ExplosionVisitor(int power) => _power = power;

    public void Visit(Player player)
    {
        int before = player.Health;
        player.Health = Math.Max(0, player.Health - _power);
        Logger.Log(Src, $"{player.Name} (гравець) отримує шкоду {_power}: HP {before} → {player.Health}");
    }

    public void Visit(Monster monster)
    {
        int before = monster.Health;
        monster.Health = Math.Max(0, monster.Health - _power);
        string result = monster.IsAlive ? "вижив" : "загинув";
        Logger.Log(Src, $"{monster.Name} (монстр) отримує шкоду {_power}: HP {before} → {monster.Health}, {result}");
    }

    public void Visit(Merchant merchant)
    {
        merchant.HasFled = true;
        Logger.Log(Src, $"{merchant.Name} (торговець) тікає з локації");
    }

    public void Visit(Chest chest)
    {
        if (!chest.IsOpen)
        {
            chest.IsOpen = true;
            Logger.Log(Src, $"{chest.Name} (скриня) відкривається вибухом, видно {chest.Loot} зол.");
        }
    }

    public void Visit(Trap trap)
    {
        trap.IsTriggered = true;
        Logger.Log(Src, $"{trap.Name} (пастка) детонує від вибуху");
    }
}

// Операція 2: статистика. Відвідувач може НАКОПИЧУВАТИ стан
// (лічильники) під час проходу, а звіт виводить після нього.
public class StatisticsVisitor : IGameVisitor
{
    private const string Src = "Статистика";
    private int _players, _playersHp, _totalGold;
    private int _monsters, _aliveMonsters;
    private int _merchants, _fled;
    private int _chests, _openChests;
    private int _traps, _activeTraps;

    public void Visit(Player player)
    {
        _players++;
        _playersHp += player.Health;
        _totalGold += player.Gold;
    }

    public void Visit(Monster monster)
    {
        _monsters++;
        if (monster.IsAlive) _aliveMonsters++;
    }

    public void Visit(Merchant merchant)
    {
        _merchants++;
        if (merchant.HasFled) _fled++;
        _totalGold += merchant.Gold;
    }

    public void Visit(Chest chest)
    {
        _chests++;
        if (chest.IsOpen) _openChests++;
    }

    public void Visit(Trap trap)
    {
        _traps++;
        if (!trap.IsTriggered) _activeTraps++;
    }

    public void PrintReport()
    {
        Logger.Log(Src, $"Гравці: {_players}, сумарне HP = {_playersHp}");
        Logger.Log(Src, $"Монстри: {_monsters}, живих = {_aliveMonsters}");
        Logger.Log(Src, $"Торговці: {_merchants}, втекло = {_fled}");
        Logger.Log(Src, $"Скрині: {_chests}, відкритих = {_openChests}");
        Logger.Log(Src, $"Пастки: {_traps}, активних = {_activeTraps}");
        Logger.Log(Src, $"Золото гравців і торговців разом: {_totalGold}");
    }
}

// Операція 3: збереження стану світу.
// Це НОВА операція, додана одним класом. Жоден із класів
// Player/Monster/Merchant/Chest/Trap та Location не змінювався.
public class WorldSaveVisitor : IGameVisitor
{
    private const string Src = "Збереження";
    private readonly StringBuilder _snapshot = new();

    public int Count { get; private set; }
    public string Snapshot => _snapshot.ToString();

    private void Save(string line)
    {
        _snapshot.AppendLine(line);
        Count++;
        Logger.Log(Src, "записано: " + line);
    }

    public void Visit(Player player) =>
        Save($"Player;{player.Name};hp={player.Health};gold={player.Gold}");

    public void Visit(Monster monster) =>
        Save($"Monster;{monster.Name};hp={monster.Health}");

    public void Visit(Merchant merchant) =>
        Save($"Merchant;{merchant.Name};gold={merchant.Gold};fled={merchant.HasFled}");

    public void Visit(Chest chest) =>
        Save($"Chest;{chest.Name};open={chest.IsOpen};loot={chest.Loot}");

    public void Visit(Trap trap) =>
        Save($"Trap;{trap.Name};damage={trap.Damage};triggered={trap.IsTriggered}");
}

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;   

        var location = BuildLocation();

        Situation1_Explosion(location);
        Situation2_Statistics(location);
        Situation3_WorldSave(location);
    }

    private static Location BuildLocation()
    {
        var location = new Location("Темний ліс");
        location.Add(new Player("Ярослав", 100, 50));
        location.Add(new Player("Ольга", 80, 120));
        location.Add(new Monster("Вовк", 40));
        location.Add(new Monster("Троль", 150));
        location.Add(new Merchant("Богдан", 300));
        location.Add(new Chest("Дубова", 75));
        location.Add(new Trap("Капкан", 30));
        return location;
    }

    // Сценарій 1: вибух (гравці й монстри — шкода, скрині відкриваються, торговці тікають)
    private static void Situation1_Explosion(Location location)
    {
        Console.WriteLine("-Завдання 3, сценарій 1: обробка вибуху (Visitor)-");
        location.PrintState("Стан ДО вибуху");
        location.Accept(new ExplosionVisitor(power: 60));
        location.PrintState("Стан ПІСЛЯ вибуху");
    }

    // Сценарій 2: статистика по тій самій локації
    private static void Situation2_Statistics(Location location)
    {
        Console.WriteLine();
        Console.WriteLine("-Завдання 3, сценарій 2: статистика по тій самій локації-");
        var stats = new StatisticsVisitor();
        location.Accept(stats);
        stats.PrintReport();
    }

    // Сценарій 3: нова операція одним класом, без змін у класах об'єктів
    private static void Situation3_WorldSave(Location location)
    {
        Console.WriteLine();
        Console.WriteLine("-Завдання 3, сценарій 3: збереження стану світу (нова операція)-");
        var save = new WorldSaveVisitor();
        location.Accept(save);
        Logger.Log("Раннер", $"Збережено об'єктів: {save.Count}, класи об'єктів не змінювались");
    }
}