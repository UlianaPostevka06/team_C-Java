using System;
using System.Text;

namespace Lab10.Task2;
// Журнал: усі події у форматі [час] [джерело] подія
public static class Logger
{
    public static void Log(string source, string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{source}] {message}");
    }
}

// ЗАВДАННЯ 2. ABSTRACT FACTORY — стартове спорядження фракцій
//як розпізнати шаблон за описом:
//«Потрібен НАБІР пов'язаних предметів, які працюють лише РАЗОМ
//(однієї фракції), і змішувати їх не можна» -> Abstract Factory.
// Було: кожен предмет вибирався окремо, і ельф отримав гномівську броню.
// Стало: фабрика фракції видає ВЕСЬ набір одразу. Брати зброю з
// однієї фабрики, а броню з іншої код видачі просто не буде, бо працює
// з однією фабрикою за раз.
//
//Ролі в шаблоні:
//Weapon/Armor/Mount          — AbstractProduct (типи предметів)
//ElvenBow, DwarvenAxe ...    — ConcreteProduct (конкретні предмети)
//IFactionEquipmentFactory    — AbstractFactory (інтерфейс: 3 методи Create...)
//ElfEquipmentFactory ...     — ConcreteFactory (по одній на фракцію)
//EquipmentIssuer             — Client (код видачі; знає ЛИШЕ абстракції)
//
//приклад із гри:
//Фабрика ельфів видає: Ельфійський лук + Ельфійську кольчугу + Лісового оленя.
//Фабрика гномів: Сокиру + Панцир + Цапа. Помішати неможливо.
//Фракція це окрема група, до якої належить персонаж, як команда чи народ

// Abstract Products
public abstract class EquipmentItem
{
    public string Faction { get; }   // до якої фракції належить предмет
    public string Name { get; }

    protected EquipmentItem(string faction, string name)
    {
        Faction = faction;
        Name = name;
    }

    public override string ToString() => Name;
}

public abstract class Weapon : EquipmentItem
{
    protected Weapon(string faction, string name) : base(faction, name) { }
}

public abstract class Armor : EquipmentItem
{
    protected Armor(string faction, string name) : base(faction, name) { }
}

public abstract class Mount : EquipmentItem
{
    protected Mount(string faction, string name) : base(faction, name) { }
}

// Concrete Products: ЕЛЬФИ 
public class ElvenBow : Weapon     { public ElvenBow()     : base("Ельфи", "Ельфійський лук") { } }
public class ElvenArmor : Armor    { public ElvenArmor()   : base("Ельфи", "Ельфійська кольчуга") { } }
public class Stag : Mount          { public Stag()         : base("Ельфи", "Лісовий олень") { } }

//  Concrete Products: ГНОМИ 
public class DwarvenAxe : Weapon   { public DwarvenAxe()   : base("Гноми", "Гномівська сокира") { } }
public class DwarvenArmor : Armor  { public DwarvenArmor() : base("Гноми", "Гномівський панцир") { } }
public class MountainGoat : Mount  { public MountainGoat() : base("Гноми", "Гірський цап") { } }

//Concrete Products: ЛЮДИ 
public class HumanSword : Weapon   { public HumanSword()   : base("Люди", "Лицарський меч") { } }
public class HumanArmor : Armor    { public HumanArmor()   : base("Люди", "Лицарські лати") { } }
public class Horse : Mount         { public Horse()        : base("Люди", "Бойовий кінь") { } }

// Abstract Factory 
public interface IFactionEquipmentFactory
{
    string FactionName { get; }
    Weapon CreateWeapon();
    Armor CreateArmor();
    Mount CreateMount();
}

// Concrete Factories 
public class ElfEquipmentFactory : IFactionEquipmentFactory
{
    public string FactionName => "Ельфи";
    public Weapon CreateWeapon() => new ElvenBow();
    public Armor CreateArmor() => new ElvenArmor();
    public Mount CreateMount() => new Stag();
}

public class DwarfEquipmentFactory : IFactionEquipmentFactory
{
    public string FactionName => "Гноми";
    public Weapon CreateWeapon() => new DwarvenAxe();
    public Armor CreateArmor() => new DwarvenArmor();
    public Mount CreateMount() => new MountainGoat();
}

public class HumanEquipmentFactory : IFactionEquipmentFactory
{
    public string FactionName => "Люди";
    public Weapon CreateWeapon() => new HumanSword();
    public Armor CreateArmor() => new HumanArmor();
    public Mount CreateMount() => new Horse();
}

//  Набір, який отримує персонаж 
public class StarterKit
{
    public Weapon Weapon { get; }
    public Armor Armor { get; }
    public Mount Mount { get; }

    public StarterKit(Weapon weapon, Armor armor, Mount mount)
    {
        Weapon = weapon;
        Armor = armor;
        Mount = mount;
    }

    public override string ToString() =>
        $"зброя: {Weapon}, броня: {Armor}, їздова тварина: {Mount}";
}

// Client: код видачі спорядження
// У цьому класі НЕМАЄ слів Elf/Dwarf/Human/Orc, жодного if за
// фракцією і жодного new конкретного предмета. Він працює лише з
// абстракціями (IFactionEquipmentFactory, Weapon, Armor, Mount).
// Тому нова фракція не потребує змін у цьому коді.
public class EquipmentIssuer
{
    public StarterKit Issue(string character, IFactionEquipmentFactory factory)
    {
        Logger.Log("Видача", $"Персонаж {character}: видаємо набір фракції «{factory.FactionName}»");

        // Усі три предмети беруться з ОДНІЄЇ фабрики -> одна фракція гарантовано
        Weapon weapon = factory.CreateWeapon();
        Armor armor = factory.CreateArmor();
        Mount mount = factory.CreateMount();
        var kit = new StarterKit(weapon, armor, mount);

        Logger.Log("Видача", $"  Склад набору: {kit}");

        bool sameFaction = weapon.Faction == armor.Faction && armor.Faction == mount.Faction;
        Logger.Log("Видача", sameFaction
            ? $"  Усі предмети належать до фракції «{weapon.Faction}»"
            : "  ПОМИЛКА: предмети різних фракцій!");

        return kit;
    }
}

//  ДОДАНО ПІЗНІШЕ: фракція ОРКІВ 
//нова фракція = нові класи (3 предмети + 1 фабрика).
// EquipmentIssuer і IFactionEquipmentFactory НЕ змінюються.
public class OrcClub : Weapon      { public OrcClub()      : base("Орки", "Орківська булава") { } }
public class OrcArmor : Armor      { public OrcArmor()     : base("Орки", "Орківські шкури") { } }
public class Warg : Mount          { public Warg()         : base("Орки", "Варг") { } }

public class OrcEquipmentFactory : IFactionEquipmentFactory
{
    public string FactionName => "Орки";
    public Weapon CreateWeapon() => new OrcClub();
    public Armor CreateArmor() => new OrcArmor();
    public Mount CreateMount() => new Warg();
}

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;   
        Situation2_AbstractFactory();
    }

    private static void Situation2_AbstractFactory()
    {
        Console.WriteLine("-Завдання 2: стартове спорядження фракцій (Abstract Factory)");

        var issuer = new EquipmentIssuer();   // ОДИН код видачі для всіх фракцій

        issuer.Issue("Ліріель", new ElfEquipmentFactory());
        issuer.Issue("Торін", new DwarfEquipmentFactory());
        issuer.Issue("Артур", new HumanEquipmentFactory());

        // Нова фракція БЕЗ зміни коду видачі
        Logger.Log("Server", "Додаємо фракцію орків (нові класи, EquipmentIssuer не змінюється)");
        issuer.Issue("Грош", new OrcEquipmentFactory());
    }
}