using System;
using System.Collections.Generic;
using System.Text;

namespace Lab13.Task1;

//  Завдання 1 — COMMAND (дії гравця і повтор бою)


//Було: дії виконувались одразу в коді обробки клавіш. Щоб зробити повтор бою чи скасування, довелося б копіювати всю логіку.
//Стало: кожна дія (рух, атака, уміння, зілля) — ОКРЕМИЙ ОБ'ЄКТ із двома методами: Execute (виконати) і Undo (скасувати). Об'єкт можна покласти
//в список — і тоді з'являється і запис бою, і повтор, і скасування.


//повтор і скасування:
//-Запис бою = список виконаних команд (_history).
//-Повтор = виконати ті самі команди ще раз на НОВИХ персонажах.Тому команда знає лише СТОРОНУ (Гравець/Противник), а не конкретного
//персонажа: яким саме персонажем діяти, їй каже BattleField.
//-Скасування = взяти останню команду зі списку й викликати Undo. Щоб скасувати ТОЧНО, команда запам'ятовує, що саме змінила.
//-Приклад: зілля лікує на 25, але в героя бракувало лише 15 HP.Реально додалося 15, тому Undo забирає 15, а не 25.

//в журналі видно, хто що робить:
//[Контролер:Бій]   — отримав запит;
//[AttackCommand]   — фактично виконав дію (сама команда);
//[Character:Вовк]  — отримувач: у нього реально змінився стан.

// Журнал: усі події у форматі [час] [джерело] подія
public static class Logger
{
    public static void Log(string source, string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{source}] {message}");
    }
}

//  RECEIVER: персонаж. Саме він змінюється під час виконання команд.
public class Character
{
    public string Name { get; }
    public int Hp { get; private set; }
    public int MaxHp { get; }
    public int Mana { get; private set; }
    public int MaxMana { get; }
    public int Potions { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }

    public Character(string name, int maxHp, int maxMana, int potions)
    {
        Name = name;
        MaxHp = maxHp;
        Hp = maxHp;
        MaxMana = maxMana;
        Mana = maxMana;
        Potions = potions;
    }

    public int ChangeHp(int delta, string reason)
    {
        int before = Hp;
        Hp = Math.Clamp(Hp + delta, 0, MaxHp);
        Logger.Log($"Character:{Name}", $"HP {before} → {Hp} ({reason})");
        return Hp - before;   // скільки реально змінилось
    }

    public int ChangeMana(int delta, string reason)
    {
        int before = Mana;
        Mana = Math.Clamp(Mana + delta, 0, MaxMana);
        Logger.Log($"Character:{Name}", $"Мана {before} → {Mana} ({reason})");
        return Mana - before;
    }

    public int ChangePotions(int delta, string reason)
    {
        int before = Potions;
        Potions = Math.Max(0, Potions + delta);
        Logger.Log($"Character:{Name}", $"Зілля {before} → {Potions} ({reason})");
        return Potions - before;
    }

    public void MoveBy(int dx, int dy, string reason)
    {
        int fromX = X, fromY = Y;
        X += dx;
        Y += dy;
        Logger.Log($"Character:{Name}", $"Позиція ({fromX},{fromY}) → ({X},{Y}) ({reason})");
    }

    // Стан без імені: за ним порівнюємо оригінальний бій і відтворений
    public string Summary() =>
        $"HP {Hp}/{MaxHp}, мана {Mana}/{MaxMana}, позиція ({X},{Y}), зілля {Potions}";
}

//  Сторони бою і «поле бою». Команда знає лише СТОРОНУ, а не персонажа,
//  тому один і той самий запис можна виконати на нових персонажах.
public enum Side
{
    Hero,    // гравець
    Enemy    // ворог
}

public class BattleField
{
    private readonly Character _hero;
    private readonly Character _enemy;

    public BattleField(Character hero, Character enemy)
    {
        _hero = hero;
        _enemy = enemy;
    }

    public Character Get(Side side) => side == Side.Hero ? _hero : _enemy;

    public Character GetOpponent(Side side) => side == Side.Hero ? _enemy : _hero;

    public static string Label(Side side) => side == Side.Hero ? "Гравець" : "Противник";
}

//  COMMAND: спільний інтерфейс усіх дій бою
public interface ICommand
{
    string Description { get; }            // короткий опис для журналу
    void Execute(BattleField field);       // виконати дію
    void Undo(BattleField field);          // скасувати дію
}

//  CONCRETE COMMANDS: кожна дія — окремий клас

//  Рух 
public class MoveCommand : ICommand
{
    private readonly Side _who;
    private readonly int _dx;
    private readonly int _dy;

    public MoveCommand(Side who, int dx, int dy)
    {
        _who = who;
        _dx = dx;
        _dy = dy;
    }

    public string Description => $"{BattleField.Label(_who)}: рух на ({_dx},{_dy})";

    public void Execute(BattleField field)
    {
        Character actor = field.Get(_who);
        Logger.Log("MoveCommand", $"Виконую: {actor.Name} рухається на ({_dx},{_dy})");
        actor.MoveBy(_dx, _dy, "рух");
    }

    public void Undo(BattleField field)
    {
        Character actor = field.Get(_who);
        Logger.Log("MoveCommand", $"Скасовую: {actor.Name} повертається назад");
        actor.MoveBy(-_dx, -_dy, "скасування руху");
    }
}

//Атака 
public class AttackCommand : ICommand
{
    private readonly Side _attacker;
    private readonly int _damage;
    private int _appliedHp;   // на скільки HP цілі РЕАЛЬНО змінилось (для точного Undo)

    public AttackCommand(Side attacker, int damage)
    {
        _attacker = attacker;
        _damage = damage;
    }

    public string Description => $"{BattleField.Label(_attacker)}: атака (шкода {_damage})";

    public void Execute(BattleField field)
    {
        Character attacker = field.Get(_attacker);
        Character target = field.GetOpponent(_attacker);
        Logger.Log("AttackCommand", $"Виконую: {attacker.Name} атакує {target.Name}, шкода {_damage}");
        _appliedHp = target.ChangeHp(-_damage, "атака");
    }

    public void Undo(BattleField field)
    {
        Character attacker = field.Get(_attacker);
        Character target = field.GetOpponent(_attacker);
        Logger.Log("AttackCommand", $"Скасовую: атака {attacker.Name} по {target.Name}");
        target.ChangeHp(-_appliedHp, "скасування атаки");
    }
}

//  Уміння (витрачає ману і б'є по цілі)
public class UseAbilityCommand : ICommand
{
    private readonly Side _caster;
    private readonly string _ability;
    private readonly int _manaCost;
    private readonly int _damage;
    private int _appliedMana;
    private int _appliedHp;

    public UseAbilityCommand(Side caster, string ability, int manaCost, int damage)
    {
        _caster = caster;
        _ability = ability;
        _manaCost = manaCost;
        _damage = damage;
    }

    public string Description => $"{BattleField.Label(_caster)}: уміння «{_ability}»";

    public void Execute(BattleField field)
    {
        Character caster = field.Get(_caster);
        Character target = field.GetOpponent(_caster);
        Logger.Log("UseAbilityCommand",
            $"Виконую: {caster.Name} застосовує «{_ability}» на {target.Name} (мана {_manaCost}, шкода {_damage})");
        _appliedMana = caster.ChangeMana(-_manaCost, $"уміння «{_ability}»");
        _appliedHp = target.ChangeHp(-_damage, $"уміння «{_ability}»");
    }

    public void Undo(BattleField field)
    {
        Character caster = field.Get(_caster);
        Character target = field.GetOpponent(_caster);
        Logger.Log("UseAbilityCommand", $"Скасовую: «{_ability}» від {caster.Name}");
        caster.ChangeMana(-_appliedMana, "скасування уміння");
        target.ChangeHp(-_appliedHp, "скасування уміння");
    }
}

//Зілля лікування 
public class UsePotionCommand : ICommand
{
    private readonly Side _user;
    private readonly int _heal;
    private bool _used;           // чи зілля реально було використано
    private int _appliedHp;
    private int _appliedPotions;

    public UsePotionCommand(Side user, int heal)
    {
        _user = user;
        _heal = heal;
    }

    public string Description => $"{BattleField.Label(_user)}: зілля лікування (+{_heal})";

    public void Execute(BattleField field)
    {
        Character user = field.Get(_user);
        if (user.Potions <= 0)
        {
            Logger.Log("UsePotionCommand", $"Виконую: у {user.Name} немає зілля, дія не виконана");
            _used = false;
            return;
        }

        Logger.Log("UsePotionCommand", $"Виконую: {user.Name} п'є зілля (лікування {_heal})");
        _appliedPotions = user.ChangePotions(-1, "випито зілля");
        _appliedHp = user.ChangeHp(_heal, "лікування");   // може додатися менше, ніж _heal
        _used = true;
    }

    public void Undo(BattleField field)
    {
        Character user = field.Get(_user);
        if (!_used)
        {
            Logger.Log("UsePotionCommand", "Скасовую: зілля не використовувалось, нічого повертати");
            return;
        }

        Logger.Log("UsePotionCommand", $"Скасовую: зілля {user.Name} (повертаю реально додане: {_appliedHp} HP)");
        user.ChangeHp(-_appliedHp, "скасування лікування");
        user.ChangePotions(-_appliedPotions, "зілля повернено");
    }
}

//INVOKER: контролер бою («код обробки клавіш»)
//він НЕ знає, що робить кожна команда. Він лише отримує запит,
//  викликає Execute, записує команду і за потреби викликає Undo.
public class BattleController
{
    private readonly string _label;
    private readonly BattleField _field;
    private readonly List<ICommand> _history = new();   // запис бою

    public bool TrainingMode { get; }
    public IReadOnlyList<ICommand> History => _history;

    public BattleController(string label, BattleField field, bool trainingMode)
    {
        _label = label;
        _field = field;
        TrainingMode = trainingMode;
    }

    private string Source => $"Контролер:{_label}";

    // Виконати дію і записати її
    public void Execute(ICommand command)
    {
        Logger.Log(Source, $"Отримано запит: «{command.Description}»");
        command.Execute(_field);
        _history.Add(command);
        Logger.Log(Source, $"Дію записано (усього дій: {_history.Count})");
    }

    // Скасувати останню дію (лише в режимі тренування)
    public bool Undo()
    {
        if (!TrainingMode)
        {
            Logger.Log(Source, "Скасування недоступне: це не режим тренування");
            return false;
        }

        if (_history.Count == 0)
        {
            Logger.Log(Source, "Немає дій для скасування");
            return false;
        }

        ICommand last = _history[_history.Count - 1];
        _history.RemoveAt(_history.Count - 1);
        Logger.Log(Source, $"Скасовую останню дію: «{last.Description}»");
        last.Undo(_field);
        return true;
    }

    // Повтор бою: виконати записані дії заново (на цьому полі бою)
    public void Replay(IReadOnlyList<ICommand> recorded)
    {
        // Копія списку, бо Execute додає команди до власної історії контролера
        var copy = new List<ICommand>(recorded);
        Logger.Log(Source, $"Відтворення запису бою: {copy.Count} дій");
        foreach (ICommand command in copy)
            Execute(command);
    }
}


public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;   // щоб кирилиця й «→» виводились коректно
        Situation1_Command();
    }

    // П'ять дій бою. Щоразу створюємо НОВІ об'єкти команд.
    private static List<ICommand> CreateBattleActions()
    {
        return new List<ICommand>
        {
            new MoveCommand(Side.Hero, 2, 0),                          // 1. гравець наближається
            new AttackCommand(Side.Hero, 20),                          // 2. гравець атакує
            new AttackCommand(Side.Enemy, 15),                         // 3. противник кусає гравця
            new UseAbilityCommand(Side.Hero, "Вогняна куля", 30, 35),  // 4. уміння
            new UsePotionCommand(Side.Hero, 25)                        // 5. зілля (лікує лише на 15!)
        };
    }

    private static void PrintState(string title, Character hero, Character enemy)
    {
        Logger.Log("Server", title);
        Logger.Log("Server", $"  {hero.Name}: {hero.Summary()}");
        Logger.Log("Server", $"  {enemy.Name}: {enemy.Summary()}");
    }

    private static void Situation1_Command()
    {
        Console.WriteLine("-Завдання 1: дії гравця і повтор бою");

        //Сценарій 1: короткий бій з 5 дій
        Logger.Log("Server", "-Сценарій 1: короткий бій з 5 дій");
        var hero = new Character("Aldric", 100, 100, 3);
        var wolf = new Character("Вовк", 100, 0, 0);
        var battle = new BattleController("Бій", new BattleField(hero, wolf), trainingMode: false);

        foreach (ICommand action in CreateBattleActions())
            battle.Execute(action);

        PrintState("Стан після бою:", hero, wolf);

        // Додатково: у звичайному бою скасування заборонене
        battle.Undo();

        //Сценарій 2: повтор бою на НОВИХ персонажах
        Logger.Log("Server", "-Сценарій 2: відтворення бою із запису на нових персонажах");
        var hero2 = new Character("Aldric (відтворення)", 100, 100, 3);
        var wolf2 = new Character("Вовк (відтворення)", 100, 0, 0);
        var replay = new BattleController("Відтворення", new BattleField(hero2, wolf2), trainingMode: false);

        replay.Replay(battle.History);

        PrintState("Стан після відтворення:", hero2, wolf2);
        bool same = hero.Summary() == hero2.Summary() && wolf.Summary() == wolf2.Summary();
        Logger.Log("Server", $"Результат відтворення збігається з оригіналом: {same}");

        //Сценарій 3: режим тренування, скасування двох останніх дій 
        Logger.Log("Server", "-Сценарій 3: режим тренування, скасування двох останніх дій");
        var hero3 = new Character("Aldric (тренування)", 100, 100, 3);
        var wolf3 = new Character("Вовк (тренування)", 100, 0, 0);
        var training = new BattleController("Тренування", new BattleField(hero3, wolf3), trainingMode: true);

        string heroAfter3 = "", wolfAfter3 = "";
        List<ICommand> actions = CreateBattleActions();
        for (int i = 0; i < actions.Count; i++)
        {
            training.Execute(actions[i]);
            if (i == 2)   // запам'ятовуємо стан після 3-ї дії
            {
                heroAfter3 = hero3.Summary();
                wolfAfter3 = wolf3.Summary();
            }
        }

        PrintState("Стан перед скасуванням:", hero3, wolf3);

        training.Undo();   // скасовуємо 5-ту дію (зілля)
        training.Undo();   // скасовуємо 4-ту дію (уміння)

        PrintState("Стан після скасування двох останніх дій:", hero3, wolf3);
        bool restored = hero3.Summary() == heroAfter3 && wolf3.Summary() == wolfAfter3;
        Logger.Log("Server", $"Стан повернувся до стану після 3-ї дії: {restored}");
    }
}