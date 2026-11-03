using System;
using System.Collections.Generic;
using System.Text;

namespace Lab10.Task1;


// Журнал: усі події у форматі [час] [джерело] подія
public static class Logger
{
    public static void Log(string source, string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{source}] {message}");
    }
}
//  ЗАВДАННЯ 1. FACTORY METHOD — спавн монстрів на локаціях
//
//  ШПОРА (як розпізнати шаблон за описом):
//   • «Процес однаковий для всіх, а ПРОДУКТ у кожного свій» -> Factory Method.
//   • Було: switch за типом локації -> додавання пустелі ламало загальний код.
//   • Стало: загальний процес Spawn() написаний ОДИН раз у базовому класі
//     Location, а «яку істоту створити» вирішує сама локація (метод
//     CreateMonster, який перевизначає кожна локація).
//
//  ШПОРА (ролі в шаблоні):
//   Monster          — Product (абстрактний продукт)
//   Wolf/Ghoul/...   — ConcreteProduct (конкретні продукти)
//   Location         — Creator (містить процес Spawn + абстрактний factory method)
//   Forest/Swamp/... — ConcreteCreator (кожен вирішує, якого монстра створити)
//
//  ШПОРА (чим відрізняється від Abstract Factory, питання на захисті):
//   Factory Method створює ОДИН продукт (один монстр) і працює через
//   успадкування (перевизначення методу). Abstract Factory створює
//   ЦІЛУ РОДИНУ пов'язаних об'єктів (див. завдання 2).

//Product
public abstract class Monster
{
    public string Kind { get; }
    public int Level { get; }
    public int Hp { get; }
    public int Attack { get; }

    protected Monster(string kind, int level, int hp, int attack)
    {
        Kind = kind;
        Level = level;
        Hp = hp;
        Attack = attack;
    }

    // Читабельний вигляд об'єкта для консолі (вимога лаби)
    public override string ToString() =>
        $"{Kind} (рівень {Level}, hp {Hp}, атака {Attack})";
}

// ----- Concrete Products -----
public class Wolf : Monster    { public Wolf()    : base("Вовк",  5, 120, 15) { } }
public class Ghoul : Monster   { public Ghoul()   : base("Упир",  8, 200, 22) { } }
public class Troll : Monster   { public Troll()   : base("Троль", 12, 450, 40) { } }

// ----- Creator -----
public abstract class Location
{
    public string Name { get; }
    private readonly int _limit;                 // максимум монстрів на локації
    private readonly string[] _spawnPoints;      // можливі точки появи
    private readonly List<Monster> _monsters = new();

    public IReadOnlyList<Monster> Monsters => _monsters;

    protected Location(string name, int limit, string[] spawnPoints)
    {
        Name = name;
        _limit = limit;
        _spawnPoints = spawnPoints;
    }

    // FACTORY METHOD: кожна локація сама каже, якого монстра створювати.
    // Тут НЕМАЄ жодного switch/if за типом локації.
    protected abstract Monster CreateMonster();

    // ЗАГАЛЬНИЙ ПРОЦЕС СПАВНУ — описаний ОДИН раз і НЕ змінюється,
    // коли додається нова локація. Він викликає CreateMonster(),
    // але не знає, який саме монстр повернеться.
    public bool Spawn()
    {
        // Крок 1. Перевірити ліміт монстрів
        if (_monsters.Count >= _limit)
        {
            Logger.Log($"Локація:{Name}", $"Ліміт монстрів ({_limit}) досягнуто — спавн скасовано");
            return false;
        }

        // Крок 2. Обрати точку появи
        string point = _spawnPoints[_monsters.Count % _spawnPoints.Length];
        Logger.Log($"Локація:{Name}", $"Обрано точку появи: {point}");

        // Крок 3. Створити монстра (тут працює factory method)
        Monster monster = CreateMonster();

        // Крок 4. Додати його в світ
        _monsters.Add(monster);
        Logger.Log($"Локація:{Name}", $"Заспавнено: {monster} ({_monsters.Count}/{_limit})");
        return true;
    }
}

// Concrete Creators
public class ForestLocation : Location
{
    public ForestLocation() : base("Ліс", 2, new[] { "Галявина", "Старий дуб" }) { }
    protected override Monster CreateMonster() => new Wolf();
}

public class SwampLocation : Location
{
    public SwampLocation() : base("Болото", 2, new[] { "Трясовина", "Мертве дерево" }) { }
    protected override Monster CreateMonster() => new Ghoul();
}

public class MountainLocation : Location
{
    public MountainLocation() : base("Гори", 2, new[] { "Печера", "Перевал" }) { }
    protected override Monster CreateMonster() => new Troll();
}

// ----- ДОДАНО ПІЗНІШЕ: пустеля -----
// ШПОРА: щоб додати нову локацію, пишемо ЛИШЕ два нових класи
// (монстр + локація). Клас Location і метод Spawn() НЕ чіпаємо —
// це і є вимога «розширення без змін загального коду» (принцип OCP).
public class Scorpion : Monster { public Scorpion() : base("Скорпіон", 7, 150, 28) { } }

public class DesertLocation : Location
{
    public DesertLocation() : base("Пустеля", 2, new[] { "Бархан", "Оазис" }) { }
    protected override Monster CreateMonster() => new Scorpion();
}

//  РАННЕР (запуск): завдання 1
public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;   // щоб кирилиця й «→» виводились коректно
        Situation1_FactoryMethod();
    }

    private static void Situation1_FactoryMethod()
    {
        Console.WriteLine("=== Завдання 1: спавн монстрів на локаціях (Factory Method) ===");

        // Список базового типу Location: процес спавну однаковий для всіх
        var locations = new List<Location>
        {
            new ForestLocation(),
            new SwampLocation(),
            new MountainLocation()
        };

        // Один і той самий процес Spawn() на лісі, болоті й горах
        foreach (Location location in locations)
        {
            location.Spawn();
            location.Spawn();
        }

        // Додатково: перевірка кроку «ліміт» (на лісі вже 2 з 2)
        Logger.Log("Server", "Спроба заспавнити третього монстра в лісі");
        locations[0].Spawn();

        // Нова локація БЕЗ зміни Location.Spawn()
        Logger.Log("Server", "Додаємо локацію «Пустеля» (нові класи Scorpion і DesertLocation)");
        var desert = new DesertLocation();
        locations.Add(desert);
        desert.Spawn();
        desert.Spawn();

        // Підсумок: що в світі
        Logger.Log("Server", "=== Склад світу ===");
        foreach (Location location in locations)
            Logger.Log("Server", $"{location.Name}: {string.Join("; ", location.Monsters)}");
    }
}
