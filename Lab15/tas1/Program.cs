using System;
using System.Text;

namespace Lab15.Task1;


//  ШПОРА (як розпізнати шаблон за описом):
//-«Поведінка об'єкта залежить від його ПОТОЧНОГО СТАНУ, а стани змінюють один одного» -> State.
//-Було: один метод Update з великим розгалуженням if/switch за станом. Додали стан «приклик підкріплення» і зламали решту.
//-Стало: кожен стан — ОКРЕМИЙ КЛАС зі своєю поведінкою й своїми переходами. Монстр просто каже: «станe, оновись» (state.Update).

public static class Logger
{
    public static void Log(string source, string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{source}] {message}");
    }
}

//Гравець (для демо: лише ім'я та позиція на лінії)
public class Player
{
    public string Name { get; }
    public int Position { get; set; }

    public Player(string name, int position)
    {
        Name = name;
        Position = position;
    }
}

//  STATE: спільна абстракція станів
public abstract class MonsterState
{
    public abstract string Name { get; }
    public abstract void Update(Monster monster, Player player);

    // Запис у журнал від імені стану: видно, ЯКИЙ стан зараз виконується
    protected void Say(Monster monster, string text) =>
        Logger.Log($"{monster.Name}:{Name}", text);
}

//  CONTEXT: монстр
//  У методі Update НЕМАЄ ні if, ні switch за станом.
//  Початковий стан передається ззовні, тому клас Monster не знає про
//  конкретні стани — новий стан не потребує змін у цьому класі.
public class Monster
{
    public const int AggroRange = 6;       // гравець ближче -> переслідування
    public const int AttackRange = 1;      // гравець ближче -> атака
    public const int FarRange = 12;        // гравець далі -> повернення
    public const int FleeHpPercent = 20;   // здоров'я менше -> втеча
    public const int Speed = 3;            // крок за одне оновлення

    public string Name { get; }
    public int MaxHp { get; }
    public int Hp { get; private set; }
    public int Position { get; private set; }
    public int SpawnPoint { get; }
    public MonsterState State { get; private set; }

    public int HpPercent => Hp * 100 / MaxHp;

    public Monster(string name, int maxHp, int spawnPoint, MonsterState initialState)
    {
        Name = name;
        MaxHp = maxHp;
        Hp = maxHp;
        SpawnPoint = spawnPoint;
        Position = spawnPoint;
        State = initialState;
        Logger.Log(Name, $"Створено на точці спавну {spawnPoint}, початковий стан: {State.Name}");
    }

    // Уся поведінка делегується поточному стану
    public void Update(Player player) => State.Update(this, player);

    public void ChangeState(MonsterState next)
    {
        Logger.Log(Name, $"Перехід стану: {State.Name} → {next.Name}");
        State = next;
    }

    public void TakeDamage(int damage)
    {
        Hp = Math.Max(0, Hp - damage);
        Logger.Log(Name, $"Отримав {damage} шкоди, здоров'я {Hp}/{MaxHp} ({HpPercent}%)");
    }

    public int DistanceTo(Player player) => Math.Abs(Position - player.Position);

    // Крок до цілі, не ближче ніж stopAt від неї
    public void MoveToward(int target, int stopAt = 0)
    {
        int delta = target - Position;
        int step = Math.Min(Speed, Math.Max(0, Math.Abs(delta) - stopAt));
        Position += Math.Sign(delta) * step;
    }

    // Крок у протилежний від точки бік
    public void MoveAwayFrom(int point)
    {
        int direction = Math.Sign(Position - point);
        if (direction == 0) direction = 1;
        Position += direction * Speed;
    }
}

//  CONCRETE STATES: кожен стан — окремий клас з власними переходами
// Патрулювання: поки гравця немає поруч
public class PatrolState : MonsterState
{
    public override string Name => "Патрулювання";

    public override void Update(Monster monster, Player player)
    {
        if (monster.DistanceTo(player) <= Monster.AggroRange)
        {
            Say(monster, $"Помітив гравця {player.Name} поруч!");
            monster.ChangeState(new ChaseState());   // ПЕРЕХІД: гравець поруч -> переслідування
            return;
        }
        Say(monster, "Патрулює територію, гравця поруч немає");
    }
}

// Переслідування
public class ChaseState : MonsterState
{
    public override string Name => "Переслідування";

    public override void Update(Monster monster, Player player)
    {
        if (monster.HpPercent < Monster.FleeHpPercent)
        {
            monster.ChangeState(new FleeState());     // ПЕРЕХІД: здоров'я < 20% -> втеча
            return;
        }
        if (monster.DistanceTo(player) > Monster.FarRange)
        {
            monster.ChangeState(new ReturnState());   // ПЕРЕХІД: гравець далеко -> повернення
            return;
        }
        if (monster.DistanceTo(player) <= Monster.AttackRange)
        {
            monster.ChangeState(new AttackState());   // ПЕРЕХІД: гравець поруч -> атака
            return;
        }

        monster.MoveToward(player.Position, Monster.AttackRange);
        Say(monster, $"Біжить за гравцем, позиція {monster.Position}, відстань {monster.DistanceTo(player)}");
    }
}

// Атака
public class AttackState : MonsterState
{
    private const int Damage = 8;

    public override string Name => "Атака";

    public override void Update(Monster monster, Player player)
    {
        if (monster.HpPercent < Monster.FleeHpPercent)
        {
            monster.ChangeState(new FleeState());     // ПЕРЕХІД: здоров'я < 20% -> втеча
            return;
        }
        if (monster.DistanceTo(player) > Monster.AttackRange)
        {
            monster.ChangeState(new ChaseState());    // ПЕРЕХІД: гравець відійшов -> переслідування
            return;
        }

        Say(monster, $"Кусає гравця {player.Name}, шкода {Damage}");
    }
}

// Втеча при низькому здоров'ї
public class FleeState : MonsterState
{
    public override string Name => "Втеча";

    public override void Update(Monster monster, Player player)
    {
        if (monster.DistanceTo(player) > Monster.FarRange)
        {
            monster.ChangeState(new ReturnState());   // ПЕРЕХІД: гравець далеко -> повернення
            return;
        }

        monster.MoveAwayFrom(player.Position);
        Say(monster, $"Тікає від гравця, позиція {monster.Position}, відстань {monster.DistanceTo(player)}");
    }
}

// Повернення на місце спавну
public class ReturnState : MonsterState
{
    public override string Name => "Повернення";

    public override void Update(Monster monster, Player player)
    {
        if (monster.Position == monster.SpawnPoint)
        {
            Say(monster, "Дійшов до місця спавну");
            monster.ChangeState(new PatrolState());   // ПЕРЕХІД: на місці -> патрулювання
            return;
        }

        monster.MoveToward(monster.SpawnPoint);
        Say(monster, $"Повертається на місце спавну, позиція {monster.Position}");
    }
}


public static class Program
{
    private static int _tick;

    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;  
        Situation1_State();
    }

    // Один «такт» гри: ставимо гравця й оновлюємо вовка
    private static void Tick(Monster wolf, Player player, int playerPosition, string what)
    {
        player.Position = playerPosition;
        Logger.Log("Server",
            $"--- Крок {++_tick}: {what} (гравець {playerPosition}, вовк {wolf.Position}, HP {wolf.HpPercent}%) ---");
        wolf.Update(player);
    }

    private static void Situation1_State()
    {
        Console.WriteLine("-Завдання 1: поведінка монстра (State)-");

        var player = new Player("Aldric", 30);
        var wolf = new Monster("Вовк", 100, 0, new PatrolState());

        // Гравець наближається: патрулювання -> переслідування -> атака
        Tick(wolf, player, 30, "гравець далеко");
        Tick(wolf, player, 6, "гравець наближається");
        Tick(wolf, player, 6, "гравець стоїть, вовк реагує");
        Tick(wolf, player, 6, "вовк наздоганяє");
        Tick(wolf, player, 6, "вовк майже поруч");
        Tick(wolf, player, 6, "гравець у зоні атаки");

        // Здоров'я падає нижче 20% -> втеча
        Logger.Log("Server", "Гравець завдає вовку сильного удару");
        wolf.TakeDamage(85);
        Tick(wolf, player, 6, "вовк поранений");
        Tick(wolf, player, 6, "вовк тікає");
        Tick(wolf, player, 6, "вовк тікає далі");

        // Гравець відходить далеко -> повернення на місце спавну
        Tick(wolf, player, 40, "гравець відходить далеко");
        Tick(wolf, player, 40, "вовк повертається");
        Tick(wolf, player, 40, "вовк на місці");
        Tick(wolf, player, 40, "знову спокійно");
    }
}