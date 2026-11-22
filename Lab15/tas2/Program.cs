using System;
using System.Collections.Generic;
using System.Text;

namespace Lab15.Task2;

//-«Послідовність кроків ОДНАКОВА, а окремі кроки у кожного типу СВОЇ» -> Template Method.
//-Було: код скопійований у три класи, і зміну правил нагороди виправили лише для печери.
//-Стало: послідовність описана ОДИН раз у базовому класі Dungeon (метод Run). Підтипи перевизначають лише ті кроки, що в них різні.


public static class Logger
{
    public static void Log(string source, string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{source}] {message}");
    }
}

//  Гравець 
public class Player
{
    public string Name { get; }
    public int Level { get; }
    public int Gold { get; set; }

    public Player(string name, int level)
    {
        Name = name;
        Level = level;
    }
}

//  ABSTRACT CLASS: шаблон проходження підземелля
public abstract class Dungeon
{
    public string Name { get; }
    private readonly int _minLevel;
    protected List<string> Rooms { get; } = new();

    protected string Source => $"Підземелля:{Name}";

    protected Dungeon(string name, int minLevel)
    {
        Name = name;
        _minLevel = minLevel;
    }

    // TEMPLATE METHOD: єдина послідовність етапів. НЕ virtual — змінити не можна.
    public bool Run(Player player)
    {
        Logger.Log(Source, $"-Проходження: {player.Name} (рівень {player.Level})-");

        Stage(1, "вхід і перевірка рівня");
        if (!Enter(player))
            return false;                      // не пустили — решта етапів не виконується

        Stage(2, "генерація кімнат");
        GenerateRooms();                       // ОБОВ'ЯЗКОВИЙ крок (abstract)
        Logger.Log(Source, $"Згенеровано кімнат: {Rooms.Count} ({string.Join(", ", Rooms)})");

        Stage(3, "пастки (необов'язковий етап)");
        PrepareTraps();                        // ГАЧОК (virtual, є поведінка за замовчуванням)

        Stage(4, "проходження кімнат");
        PassRooms();                           // ЗАГАЛЬНИЙ крок

        Stage(5, "бій з босом");
        FightBoss();                           // ОБОВ'ЯЗКОВИЙ крок (abstract)

        Stage(6, "видача нагороди");
        GiveReward(player);                    // ЗАГАЛЬНИЙ крок

        Logger.Log(Source, "-Проходження завершено-");
        return true;
    }

    private void Stage(int number, string title) =>
        Logger.Log(Source, $"Етап {number}/6: {title}");

    //ЗАГАЛЬНІ кроки: private, без virtual
    private bool Enter(Player player)
    {
        if (player.Level < _minLevel)
        {
            Logger.Log(Source, $"ВІДМОВА: потрібен рівень {_minLevel}, у гравця {player.Level}");
            return false;
        }
        Logger.Log(Source, $"Вхід дозволено (потрібен рівень {_minLevel})");
        return true;
    }

    private void PassRooms()
    {
        for (int i = 0; i < Rooms.Count; i++)
            Logger.Log(Source, $"Кімната {i + 1}/{Rooms.Count}: {Rooms[i]} — пройдено");
    }

    private void GiveReward(Player player)
    {
        // Правило нагороди в ОДНОМУ місці: змінили тут — змінилось для всіх типів
        int reward = Rooms.Count * 40 + 100;
        player.Gold += reward;
        Logger.Log(Source, $"Нагорода: {reward} золота (гравець {player.Name}, усього {player.Gold})");
    }

    //ОБОВ'ЯЗКОВІ кроки: кожен тип реалізує свій
    protected abstract void GenerateRooms();
    protected abstract void FightBoss();

    //ГАЧОК: поведінка за замовчуванням (пасток немає)
    protected virtual void PrepareTraps() =>
        Logger.Log(Source, "Пасток немає (поведінка за замовчуванням)");
}

//  CONCRETE CLASSES: лише те, що відрізняється
// Печера: пастки НЕ перевизначає — працює гачок за замовчуванням
public class CaveDungeon : Dungeon
{
    public CaveDungeon() : base("Печера", 1) { }

    protected override void GenerateRooms() =>
        Rooms.AddRange(new[] { "Вхідна печера", "Підземне озеро", "Лігво кажанів" });

    protected override void FightBoss() =>
        Logger.Log(Source, "Бос «Печерний троль» кидає валуни, але переможений");
}

// Крижаний замок: перевизначає гачок і має власні пастки
public class IceCastleDungeon : Dungeon
{
    public IceCastleDungeon() : base("Крижаний замок", 10) { }

    protected override void GenerateRooms() =>
        Rooms.AddRange(new[] { "Крижана брама", "Зала дзеркал", "Бібліотека інею", "Тронна зала" });

    protected override void PrepareTraps()
    {
        Logger.Log(Source, "Власні пастки замку: крижані шипи в коридорі");
        Logger.Log(Source, "Власні пастки замку: ковзка підлога в залі дзеркал");
    }

    protected override void FightBoss() =>
        Logger.Log(Source, "Бос «Крижаний лич» заморожує героя, але переможений");
}

// ДОДАНО ПІЗНІШЕ: болото — ОДИН новий клас, лише обов'язкові кроки.
// Базовий клас Dungeon і метод Run НЕ змінюються.
public class SwampDungeon : Dungeon
{
    public SwampDungeon() : base("Болото", 5) { }

    protected override void GenerateRooms() =>
        Rooms.AddRange(new[] { "Туманна стежка", "Трясовина", "Хатина відьми" });

    protected override void FightBoss() =>
        Logger.Log(Source, "Бос «Болотна відьма» кличе жаб, але переможена");
}


public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;   // щоб кирилиця виводилась коректно
        Situation2_TemplateMethod();
    }

    private static void Situation2_TemplateMethod()
    {
        Console.WriteLine("-Завдання 2: проходження підземель");

        var hero = new Player("Aldric", 15);

        // Змінна має тип Dungeon: процедура Run однакова для всіх типів
        Logger.Log("Server", "-Сценарій 1: пройти печеру");
        Dungeon cave = new CaveDungeon();
        cave.Run(hero);

        Logger.Log("Server", "-Сценарій 2: пройти крижаний замок тією самою процедурою");
        Dungeon castle = new IceCastleDungeon();
        castle.Run(hero);

        // Додатково: етап 1 зупиняє проходження, решта етапів не виконується
        Logger.Log("Server", "Додатково: гравець 3 рівня намагається увійти в крижаний замок");
        castle.Run(new Player("Newbie", 3));

        Logger.Log("Server", "-Сценарій 3: додано тип «болото» одним класом");
        Dungeon swamp = new SwampDungeon();
        swamp.Run(hero);

        Logger.Log("Server", $"Золото гравця {hero.Name} після трьох підземель: {hero.Gold}");
    }
}