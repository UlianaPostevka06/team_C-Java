using System;
using System.Collections.Generic;
using System.Text;

namespace Lab12.Task3;

//  Завдання 3 — COMPOSITE (гільдії і загони)

//«Деревоподібна структура: частини складаються в групи, групи в більші групи» (гравець -> загін -> гільдія -> альянс) -> Composite.
//-Було: окремі цикли на кожен рівень. Коли загін поклали в загін, цикли не знали про цей рівень і рахували неправильно.
//-Стало: кожен вузол сам вміє порахувати себе. Група просто питає своїх дітей, а ті — своїх (рекурсія). Глибина вкладеності не важлива.
//
// ролі в шаблоні):
//   IGuildMember               — Component (спільний інтерфейс гравця й об'єднання)
//   Player                     — Leaf (листок: не має дітей)
//   Union, Squad/Guild/Alliance — Composite (містить дітей і делегує їм роботу)
//   Program                    — Client (викликає ті самі методи для будь-якого вузла)

// Журнал: усі події у форматі [час] [джерело] подія
// ---------------------------------------------------------------------
public static class Logger
{
    public static void Log(string source, string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{source}] {message}");
    }
}

//  COMPONENT: спільний інтерфейс. Клієнт знає ЛИШЕ його.
public interface IGuildMember
{
    string Name { get; }
    int GetPower();                      // сумарна сила
    int GetMemberCount();                // кількість учасників (гравців)
    void Print(string indent);           // вивести вузол і все, що в ньому
    void DistributeReward(int amount);   // нарахувати нагороду
}

//  LEAF: окремий гравець. Дітей не має.
public class Player : IGuildMember
{
    public string Name { get; }
    public int Power { get; }
    public int Coins { get; private set; }   // нараховані монети

    public Player(string name, int power)
    {
        Name = name;
        Power = power;
    }

    public int GetPower() => Power;

    public int GetMemberCount() => 1;

    public void Print(string indent) =>
        Logger.Log("Дерево", $"{indent}Гравець {Name} — сила {Power}");

    // Лист просто отримує свою частку
    public void DistributeReward(int amount)
    {
        Coins += amount;
        Logger.Log($"Гравець:{Name}", $"Отримав нагороду: {amount} монет (усього {Coins})");
    }
}

//COMPOSITE: об'єднання. Може містити і гравців, і ІНШІ об'єднання.
//список _children має тип IGuildMember, тому всередині може лежати
//що завгодно: гравець, загін, гільдія. Звідси й вкладення «загін у загоні».
public abstract class Union : IGuildMember
{
    public string Name { get; }
    public string Kind { get; }                 // «Загін» / «Гільдія» / «Альянс»
    private readonly List<IGuildMember> _children = new();

    protected Union(string kind, string name)
    {
        Kind = kind;
        Name = name;
    }

    public void Add(IGuildMember member)
    {
        _children.Add(member);
    }

    // РЕКУРСІЯ: сума сил дітей. Кожна дитина рахує себе сама,
    // тому глибина вкладеності не має значення.
    public int GetPower()
    {
        int sum = 0;
        foreach (IGuildMember child in _children)
            sum += child.GetPower();
        return sum;
    }

    public int GetMemberCount()
    {
        int sum = 0;
        foreach (IGuildMember child in _children)
            sum += child.GetMemberCount();
        return sum;
    }

    // Виводить себе, а потім кожну дитину з відступом — виходить дерево
    public void Print(string indent)
    {
        Logger.Log("Дерево", $"{indent}{Kind} «{Name}» — сила {GetPower()}, учасників {GetMemberCount()}");
        foreach (IGuildMember child in _children)
            child.Print(indent + "    ");
    }

    // Нагорода ділиться між дітьми пропорційно їхній силі,
    // а кожна дитина ділить свою частку далі (аж до гравців).
    public void DistributeReward(int amount)
    {
        if (_children.Count == 0)
        {
            Logger.Log($"{Kind}:{Name}", "Порожнє об'єднання — нагороду нікому розподілити");
            return;
        }

        int totalPower = GetPower();
        Logger.Log($"{Kind}:{Name}",
            $"Отримало нагороду {amount} монет, ділить між {_children.Count} частинами пропорційно силі (сума сили {totalPower})");

        int remaining = amount;
        for (int i = 0; i < _children.Count; i++)
        {
            IGuildMember child = _children[i];
            bool isLast = i == _children.Count - 1;

            // Остання частка забирає залишок від округлення, щоб нічого не загубилося
            int share = isLast
                ? remaining
                : (totalPower > 0
                    ? (int)((long)amount * child.GetPower() / totalPower)
                    : amount / _children.Count);

            remaining -= share;
            Logger.Log($"{Kind}:{Name}", $" частка для «{child.Name}»: {share}");
            child.DistributeReward(share);
        }
    }
}

// Конкретні види об'єднань: різниця лише в назві виду
public class Squad : Union
{
    public Squad(string name) : base("Загін", name) { }
}

public class Guild : Union
{
    public Guild(string name) : base("Гільдія", name) { }
}

public class Alliance : Union
{
    public Alliance(string name) : base("Альянс", name) { }
}

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;   
        Situation3_Composite();
    }

    // ці два методи приймають IGuildMember. Їм однаково, що передали:
    // гравця, загін, гільдію чи альянс. Це і є «однакова обробка через інтерфейс».
    private static void ShowStats(IGuildMember node)
    {
        Logger.Log("Server",
            $"«{node.Name}»: сума сили {node.GetPower()}, учасників {node.GetMemberCount()}");
    }

    private static void GiveReward(IGuildMember node, int amount)
    {
        Logger.Log("Server", $"Нараховуємо нагороду {amount} монет для «{node.Name}»");
        node.DistributeReward(amount);
    }

    private static void Situation3_Composite()
    {
        Console.WriteLine("- Завдання 3: гільдії і загони (Composite)");

        //Крок 1: збираємо структуру альянс -> гільдії -> загони -> гравці
        var scouts = new Squad("Розвідники");
        scouts.Add(new Player("Aldric", 120));
        scouts.Add(new Player("Mira", 95));
        scouts.Add(new Player("Torin", 110));

        var assault = new Squad("Штурмовики");
        assault.Add(new Player("Brann", 140));
        assault.Add(new Player("Kael", 130));

        // Загін у загоні: «Штурмовики» вкладені в «Рейдову групу» (для рейдів)
        var raid = new Squad("Рейдова група");
        raid.Add(assault);
        raid.Add(new Player("Lys", 105));

        var wolves = new Guild("Вовки Сіверу");
        wolves.Add(scouts);
        wolves.Add(raid);

        var archers = new Squad("Лучники");
        archers.Add(new Player("Elara", 100));
        archers.Add(new Player("Finn", 90));

        var ravens = new Guild("Ворони");
        ravens.Add(archers);
        ravens.Add(new Player("Ravan", 150));   // гравець прямо в гільдії, без загону

        var alliance = new Alliance("Північна Зоря");
        alliance.Add(wolves);
        alliance.Add(ravens);

        //Крок 2: виводимо структуру деревом із силою кожного вузла
        Logger.Log("Server", "Структура альянсу:");
        alliance.Print("");

        //Крок 3: ті самі виклики для різних рівнів
        Logger.Log("Server", "Показники на різних рівнях:");
        ShowStats(alliance);
        ShowStats(wolves);
        ShowStats(raid);     // загін, у якому лежить інший загін

        //Крок 4: нагорода альянсу і окремій гільдії тим самим викликом
        GiveReward(alliance, 1000);
        GiveReward(wolves, 600);
    }
}