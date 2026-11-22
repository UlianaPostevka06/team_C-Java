using System.Text;

namespace tas2;

// Mediator потрібен, коли багато об'єктів взаємодіють між собою і кожен знає про всіх. 
//Рішення: вони говорять ЛИШЕ з посередником, а всі правила взаємодії зібрані в ньому одному.
static class Logger
{
    private static DateTime _time = new(2026, 1, 1, 12, 0, 0);

    public static void Log(string source, string message)
    {
        Console.WriteLine($"[{_time:HH:mm:ss}] [{source}] {message}");
        _time = _time.AddSeconds(5);
    }

    public static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"- {title}");
    }
}

// Класи домену (мінімум, без якого ситуацію не змоделювати)
///Предмет інвентаря. record -> порівняння за назвою.
record Item(string Name);

///Пропозиція гравця в обміні: набір предметів + золото.
class Offer
{
    public IReadOnlyList<Item> Items { get; }
    public int Gold { get; }

    public Offer(IEnumerable<Item> items, int gold)
    {
        Items = items.ToList();
        Gold = gold;
    }

    public static Offer Empty => new(Array.Empty<Item>(), 0);

    public override string ToString()
    {
        var parts = Items.Select(i => i.Name).ToList();
        if (Gold > 0) parts.Add($"{Gold} золота");
        return parts.Count == 0 ? "нічого" : string.Join(" + ", parts);
    }
}

///Знімок стану інвентаря — потрібен для відкату (атомарність обміну)
record InventoryState(List<Item> Items, int Gold);

class Inventory
{
    private List<Item> _items;

    public string Owner { get; }
    public int Gold { get; private set; }
    public IReadOnlyList<Item> Items => _items;

    public Inventory(string owner, IEnumerable<Item> items, int gold)
    {
        Owner = owner;
        _items = items.ToList();
        Gold = gold;
    }

    // Чи є в гравця те, що він пропонує
    public bool CanGive(Offer offer) =>
        Gold >= offer.Gold && offer.Items.All(i => _items.Contains(i));

    public void Remove(Offer offer)
    {
        if (!CanGive(offer))
            throw new InvalidOperationException($"{Owner}: у інвентарі немає запропонованого");
        foreach (var item in offer.Items) _items.Remove(item);
        Gold -= offer.Gold;
    }

    public void Add(Offer offer)
    {
        _items.AddRange(offer.Items);
        Gold += offer.Gold;
    }

    // Snapshot/Restore — для відкату, якщо обмін зламався посередині
    public InventoryState Snapshot() => new(new List<Item>(_items), Gold);

    public void Restore(InventoryState state)
    {
        _items = new List<Item>(state.Items);
        Gold = state.Gold;
    }

    public override string ToString()
    {
        var parts = _items.Select(i => i.Name).ToList();
        parts.Add($"{Gold} золота");
        return $"{Owner}: {string.Join(", ", parts)}";
    }
}

///Журнал транзакцій. Пише в нього тільки посередник.
class TransactionLog
{
    private readonly List<string> _entries = new();
    public IReadOnlyList<string> Entries => _entries;

    public void Add(string status, string details)
    {
        _entries.Add($"{status}: {details}");
        Logger.Log("Журнал транзакцій", $"{status}: {details}");
    }
}

//MEDIATOR (інтерфейс посередника).
// Колеги (вікна обміну) знають лише його, а не одне одного.
interface ITradeMediator
{
    void Register(TradeWindow window);
    void OfferChanged(TradeWindow sender, Offer offer);
    void Confirm(TradeWindow sender);
    void Disconnect(TradeWindow sender);
}

// COLLEAGUE (колега).
// Вікно обміну НЕ посилається на вікно партнера і на чужий інвентар.
// Усе, що відбувається, воно повідомляє посереднику.
class TradeWindow
{
    private readonly ITradeMediator _mediator;

    public Inventory Inventory { get; }
    public string Owner => Inventory.Owner;

    public TradeWindow(Inventory inventory, ITradeMediator mediator)
    {
        Inventory = inventory;
        _mediator = mediator;
        _mediator.Register(this);
    }

    // Дії гравця -> повідомляємо посередника
    public void SetOffer(Offer offer)
    {
        Logger.Log($"Вікно {Owner}", $"гравець пропонує: {offer}");
        _mediator.OfferChanged(this, offer);
    }

    public void Confirm()
    {
        Logger.Log($"Вікно {Owner}", "гравець натискає «Підтвердити»");
        _mediator.Confirm(this);
    }

    public void Disconnect()
    {
        Logger.Log($"Вікно {Owner}", "з'єднання втрачено");
        _mediator.Disconnect(this);
    }

    // Виклики ВІД посередника (що показати гравцю)
    public void ShowPartnerOffer(string partner, Offer offer) =>
        Logger.Log($"Вікно {Owner}", $"бачить пропозицію {partner}: {offer}");

    public void ShowConfirmationReset(string reason) =>
        Logger.Log($"Вікно {Owner}", $"підтвердження скинуто ({reason})");

    public void ShowTradeCompleted() =>
        Logger.Log($"Вікно {Owner}", "обмін завершено успішно");

    public void ShowTradeCancelled(string reason) =>
        Logger.Log($"Вікно {Owner}", $"обмін скасовано ({reason})");
}

//CONCRETE MEDIATOR (конкретний посередник).
//Тут ВСІ правила обміну:
//1) обмін лише після підтвердження ОБОМА сторін;
//2) зміна пропозиції скидає підтвердження;
//3) обмін АТОМАРНИЙ: або виконується повністю, або не виконується зовсім (перевірка перед обміном + відкат за знімками при збої);
//4) відключення учасника скасовує обмін, предмети залишаються на місці.

class TradeMediator : ITradeMediator
{
    private const string Src = "Посередник обміну";

    private readonly List<TradeWindow> _windows = new();
    private readonly Dictionary<TradeWindow, Offer> _offers = new();
    private readonly HashSet<TradeWindow> _confirmed = new();
    private readonly TransactionLog _journal;
    private bool _finished;

    public TradeMediator(TransactionLog journal) => _journal = journal;

    public void Register(TradeWindow window)
    {
        if (_windows.Count >= 2)
            throw new InvalidOperationException("В обміні можуть брати участь лише двоє");
        _windows.Add(window);
        _offers[window] = Offer.Empty;
        Logger.Log(Src, $"зареєстровано вікно гравця {window.Owner}");
    }

    public void OfferChanged(TradeWindow sender, Offer offer)
    {
        if (_finished) return;

        _offers[sender] = offer;
        Logger.Log(Src, $"отримав пропозицію від {sender.Owner}: {offer}");

        // Правило 2: зміна пропозиції скидає підтвердження обох сторін
        if (_confirmed.Count > 0)
        {
            _confirmed.Clear();
            Logger.Log(Src, "пропозицію змінено -> скидаю підтвердження обох сторін");
            foreach (var w in _windows)
                w.ShowConfirmationReset($"{sender.Owner} змінив пропозицію");
        }

        Partner(sender).ShowPartnerOffer(sender.Owner, offer);
    }

    public void Confirm(TradeWindow sender)
    {
        if (_finished) return;

        _confirmed.Add(sender);
        Logger.Log(Src, $"{sender.Owner} підтвердив обмін ({_confirmed.Count}/2)");

        // Правило 1: обмін лише коли підтвердили обидва
        if (_confirmed.Count == 2) Execute();
    }

    public void Disconnect(TradeWindow sender)
    {
        if (_finished) return;
        Cancel($"{sender.Owner} відключився до завершення обміну");
    }

    // Правило 3: атомарне виконання обміну
    private void Execute()
    {
        var a = _windows[0];
        var b = _windows[1];
        var offerA = _offers[a];
        var offerB = _offers[b];

        // Крок 1: попередня перевірка — чи є в обох те, що вони пропонують
        if (!a.Inventory.CanGive(offerA) || !b.Inventory.CanGive(offerB))
        {
            Cancel("у когось із гравців немає запропонованих предметів/золота");
            return;
        }

        // Крок 2: знімки для відкату
        var snapA = a.Inventory.Snapshot();
        var snapB = b.Inventory.Snapshot();

        try
        {
            Logger.Log(Src, "починаю обмін: списую в обох, потім зараховую");
            a.Inventory.Remove(offerA);
            b.Inventory.Remove(offerB);
            a.Inventory.Add(offerB);
            b.Inventory.Add(offerA);
        }
        catch (Exception ex)
        {
            // Крок 3: збій посеред обміну -> повний відкат
            a.Inventory.Restore(snapA);
            b.Inventory.Restore(snapB);
            Cancel($"збій під час обміну ({ex.Message}), виконано відкат");
            return;
        }

        _finished = true;
        _journal.Add("УСПІХ", $"{a.Owner} віддав [{offerA}], {b.Owner} віддав [{offerB}]");
        foreach (var w in _windows) w.ShowTradeCompleted();
    }

    private void Cancel(string reason)
    {
        _finished = true;
        Logger.Log(Src, $"скасовує обмін: {reason}");
        _journal.Add("СКАСОВАНО", reason);
        foreach (var w in _windows) w.ShowTradeCancelled(reason);
        Logger.Log(Src, "предмети й золото залишились у власників");
    }

    private TradeWindow Partner(TradeWindow w) => _windows.First(x => x != w);
}

// Клас-раннер (Client): окремий метод для кожної ситуації/сценарію
class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Situation2_Trade_Mediator();
    }

    static void Situation2_Trade_Mediator()
    {
        Logger.Section("ЗАВДАННЯ 2. Торгівля між гравцями (Mediator)");

        Scenario21_SuccessfulTrade();
        Scenario22_OfferChangeResetsConfirmation();
        Scenario23_DisconnectCancelsTrade();
    }

    // Свіжа сесія обміну: новий посередник і нові інвентарі
    static (TradeWindow a, TradeWindow b, TransactionLog journal) CreateSession()
    {
        var journal = new TransactionLog();
        var mediator = new TradeMediator(journal);

        var invA = new Inventory("Артем", new[] { new Item("Меч"), new Item("Щит") }, 50);
        var invB = new Inventory("Богдана", new[] { new Item("Лук") }, 500);

        var windowA = new TradeWindow(invA, mediator);
        var windowB = new TradeWindow(invB, mediator);
        return (windowA, windowB, journal);
    }

    static void PrintInventories(TradeWindow a, TradeWindow b)
    {
        Logger.Log("Інвентар", a.Inventory.ToString());
        Logger.Log("Інвентар", b.Inventory.ToString());
    }

    // 2.1 Обидва підтвердили -> обмін виконано
    static void Scenario21_SuccessfulTrade()
    {
        Logger.Section("Сценарій 2.1: А пропонує меч, Б — 300 золота, обидва підтверджують");
        var (a, b, _) = CreateSession();

        a.SetOffer(new Offer(new[] { new Item("Меч") }, 0));
        b.SetOffer(new Offer(Array.Empty<Item>(), 300));
        a.Confirm();
        b.Confirm();

        PrintInventories(a, b);
    }

    // 2.2 А змінює пропозицію після підтвердження Б -> підтвердження скинуто
    static void Scenario22_OfferChangeResetsConfirmation()
    {
        Logger.Section("Сценарій 2.2: А змінює пропозицію після підтвердження Б");
        var (a, b, _) = CreateSession();

        a.SetOffer(new Offer(new[] { new Item("Меч") }, 0));
        b.SetOffer(new Offer(Array.Empty<Item>(), 300));
        b.Confirm();                                           // Б підтвердив
        a.SetOffer(new Offer(new[] { new Item("Щит") }, 0));   // А передумав -> скидання
        a.Confirm();                                           // обмін не відбувся: Б має підтвердити знову

        PrintInventories(a, b);
    }

    // 2.3 Б відключається до підтвердження -> обмін скасовано
    static void Scenario23_DisconnectCancelsTrade()
    {
        Logger.Section("Сценарій 2.3: Б відключається до підтвердження");
        var (a, b, _) = CreateSession();

        a.SetOffer(new Offer(new[] { new Item("Меч") }, 0));
        b.SetOffer(new Offer(Array.Empty<Item>(), 300));
        a.Confirm();
        b.Disconnect();

        PrintInventories(a, b);
    }
}