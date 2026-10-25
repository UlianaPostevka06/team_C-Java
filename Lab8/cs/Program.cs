#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Variant6_RestaurantBooking
{
    // ЗАВДАННЯ 2: Information Expert + Creator
    //Проблема:Усі класи мали публічні get; set;, тобто стан можна міняти звідки завгодно
    //Проблема:TimeSlot, Status, CourseType були рядками (“HappyHours” тощо), без перевірки
    //Проблема:FoodSubtotal, ServiceCharge, TotalBill були звичайними полями, які рахував LegacyRestaurantProcessor зовні.
    //Проблема:PreOrderedItems був відкритим List, тож будь-хто міг додати чи видалити позицію.
    //Проблема:Об'єкти створювались порожніми й заповнювались покроково
    //Проблема:Валідації не було (кількість ≤ 0, від'ємна ціна, порожнє ім'я).


    //Було:TimeSlot, Status, CourseType були рядками (“HappyHours” тощо), без перевірки
    //Стало:неможливо ввести неіснуюче значення(патерн: захист стану)
    //Перше: замінили текстові рядки на enum, тобто на фіксований список значень. 
    //Раніше можна було написати “HappyHourz” з помилкою, і ніхто б не помітив. 
    //Тепер можна вибрати тільки HappyHours, Standard або Evening
    public enum TimeSlot { HappyHours, Standard, Evening }
    public enum CourseType { Main, Drink, Dessert }
    public enum ReservationStatus { Draft, Reserved }

    //Стало:приватний конструктор + static Create(...) з валідацією, властивості лише get(патерн: Creator)
    public sealed class Guest
    {
        public string Name { get; }
        public string Phone { get; }

        private Guest(string name, string phone) { Name = name; Phone = phone; }
        public static Guest Create(string name, string phone)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Ім'я гостя не може бути порожнім.");
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Телефон не може бути порожнім.");
            return new Guest(name.Trim(), phone.Trim());
        }
    }
    //Стало:приватний конструктор + static Create(...) з валідацією, властивості лише get(патерн: Creator)
    public sealed class MenuItem
    {
        public string Name { get; }
        public decimal Price { get; }
        public CourseType CourseType { get; }

        private MenuItem(string name, decimal price, CourseType type) { Name = name; Price = price; CourseType = type; }

        public static MenuItem Create(string name, decimal price, CourseType type)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Назва страви не може бути порожньою.");
            if (price < 0) throw new ArgumentException("Ціна не може бути від'ємною.");
            return new MenuItem(name.Trim(), price, type);
        }
    }
    //Стало:конструктор internal, створює його тільки TableReservation(патерн: Creator (агрегат створює своїх «дітей»))
    public sealed class OrderLine
    {
        private readonly MenuItem _item;
        public int Count { get; }

        internal OrderLine(MenuItem item, int count) { _item = item; Count = count; } // створює лише агрегат

        public string ItemName => _item.Name;                 // Закон Деметри: делегування
        public CourseType CourseType => _item.CourseType;
        public decimal Subtotal() => _item.Price * Count;     // Information Expert
    }

    public sealed class TableReservation
    {//Друге: список страв закрили. Раніше його можна було змінювати напряму ззовні, а тепер зовні його можна тільки читати
        private readonly List<OrderLine> _items = new List<OrderLine>();
        private readonly Guest _guest;
        private readonly PricingPolicySet _pricing;

        public int ReservationId { get; }
        public int GuestsCount { get; }
        public TimeSlot TimeSlot { get; }
        public ReservationStatus Status { get; private set; } = ReservationStatus.Draft;
        public IReadOnlyList<OrderLine> PreOrderedItems => _items.AsReadOnly(); // незмінна колекція
        public string GuestName => _guest.Name;
        public string GuestPhone => _guest.Phone;

        private TableReservation(int id, Guest guest, int guestsCount, TimeSlot slot, PricingPolicySet pricing)
        {
            ReservationId = id; _guest = guest; GuestsCount = guestsCount; TimeSlot = slot; _pricing = pricing;
        }

        // Creator: перевірка guestsCount >= 1, null-перевірки
        public static TableReservation Create(int id, Guest guest, int guestsCount, TimeSlot slot, PricingPolicySet pricing)
        {
            if (guest == null || pricing == null) throw new ArgumentNullException();
            if (guestsCount < 1) throw new ArgumentException("Має бути щонайменше 1 гість.");
            return new TableReservation(id, guest, guestsCount, slot, pricing);
        }

        // Creator: перевірка count > 0 і статусу Draft; сама створює OrderLine
        //Друге:додавати страву можна лише через метод AddItem, який перевіряє, що кількість більша за нуль
        public void AddItem(MenuItem item, int count)
        {
            if (count <= 0) throw new ArgumentException("Кількість має бути більшою за 0.");
            if (Status != ReservationStatus.Draft) throw new InvalidOperationException("Бронювання вже підтверджено.");
            _items.Add(new OrderLine(item, count));
        }
        //Четверте: додали метод Confirm, який не дозволяє підтвердити бронювання без страв, 
        //а після підтвердження нові страви додати не можна
        public void Confirm()
        {
            if (_items.Count == 0) throw new InvalidOperationException("Не можна підтвердити бронювання без страв.");
            Status = ReservationStatus.Reserved;
        }

        // Третє: розрахунки перенесли в сутності. 
        //Сума рядка рахується в самому рядку, а підсумок у самому бронюванні, 
        //і вони більше не зберігаються окремими полями, а обчислюються щоразу
        public decimal FoodSubtotal => _pricing.ApplyDiscount(_items.Sum(l => l.Subtotal()));
        public decimal ServiceCharge => _pricing.ServiceCharge(FoodSubtotal, GuestsCount);
        public decimal TotalBill => FoodSubtotal + ServiceCharge;
        public decimal Deposit => _pricing.Deposit(TotalBill, GuestsCount);
    }
    // ЗАВДАННЯ 4: Polymorphism + Protected Variations
    //Раніше правила знижок і зборів були вшиті в один метод через if. 
    //Щоб додати нове правило, треба було б лізти в основний код і ризикувати його зламати. 
    //Тепер кожне правило окремий клас за спільним інтерфейсом (це Polymorphism). 
    //Бронювання не знає, яка саме знижка діє, воно просто викликає ApplyDiscount (це Protected Variations: зміни правил не зачіпають ядро). Потрібну політику підбирає словник за часовим слотом
    // Було: if (TimeSlot == "HappyHours") та if (GuestsCount >= 5) 
    //Тут і умови за рядком, і числа 0.85 та 0.10 зашиті прямо в код, а депозиту не було взагалі
    //Пункт 1. Protected Variations: стабільні інтерфейси. Те, що може змінюватися (знижка, збір, депозит), ховаємо за інтерфейсами. Ядро знає лише їх.
    public interface IDiscountPolicy { decimal Apply(decimal amount); }
    public interface IServiceChargePolicy { decimal Calculate(decimal subtotal, int guests); }
    public interface IDepositPolicy { decimal Calculate(decimal total, int guests); }

    //Пункт 2. Polymorphism: окремий клас під кожне правило. 
    //Замість if кожен варіант поведінки має свій клас.
    public sealed class NoDiscount : IDiscountPolicy
    {
        public decimal Apply(decimal amount) => amount;
    }

    public sealed class HappyHoursDiscount : IDiscountPolicy
    {
        private readonly decimal _rate;
        public HappyHoursDiscount(decimal rate) { _rate = rate; }
        public decimal Apply(decimal amount) => Math.Round(amount * (1m - _rate), 2);
    }

    // Додаткова плата для великих компаній (використовується і для збору, і для депозиту)
    //поріг (5 гостей) і відсоток передаються через конструктор, тому магічних чисел у класах більше немає
    public sealed class LargeGroupServiceCharge : IServiceChargePolicy
    {
        private readonly int _threshold; private readonly decimal _rate;
        public LargeGroupServiceCharge(int threshold, decimal rate) { _threshold = threshold; _rate = rate; }
        public decimal Calculate(decimal subtotal, int guests) => guests >= _threshold ? Math.Round(subtotal * _rate, 2) : 0m;
    }

    public sealed class LargeGroupDeposit : IDepositPolicy
    {
        private readonly int _threshold; private readonly decimal _rate;
        public LargeGroupDeposit(int threshold, decimal rate) { _threshold = threshold; _rate = rate; }
        public decimal Calculate(decimal total, int guests) => guests >= _threshold ? Math.Round(total * _rate, 2) : 0m;
    }

    // Пункт 3. Набір політик (PricingPolicySet). Об'єднує три політики в одне місце, а бронювання лише делегує йому
    public sealed class PricingPolicySet
    {
        private readonly IDiscountPolicy _discount;
        private readonly IServiceChargePolicy _service;
        private readonly IDepositPolicy _deposit;

        public PricingPolicySet(IDiscountPolicy d, IServiceChargePolicy s, IDepositPolicy dep) { _discount = d; _service = s; _deposit = dep; }

        public decimal ApplyDiscount(decimal amount) => _discount.Apply(amount);
        public decimal ServiceCharge(decimal subtotal, int guests) => _service.Calculate(subtotal, guests);
        public decimal Deposit(decimal total, int guests) => _deposit.Calculate(total, guests);
    }

    public interface IPricingPolicyProvider { PricingPolicySet For(TimeSlot slot); }

    // Dictionary Dispatch замість if/switch: новий слот = новий запис у словнику
    public sealed class PricingPolicyProvider : IPricingPolicyProvider
    {
        private readonly Dictionary<TimeSlot, IDiscountPolicy> _discounts = new Dictionary<TimeSlot, IDiscountPolicy>
        {
            { TimeSlot.HappyHours, new HappyHoursDiscount(0.15m) },
            { TimeSlot.Standard, new NoDiscount() },
            { TimeSlot.Evening, new NoDiscount() }
        };

        public PricingPolicySet For(TimeSlot slot) => new PricingPolicySet(
            _discounts.TryGetValue(slot, out var d) ? d : new NoDiscount(),
            new LargeGroupServiceCharge(5, 0.10m),
            new LargeGroupDeposit(5, 0.20m)); // депозит 20% - припущення команди
    }
    // ЗАВДАННЯ 3: Controller + Indirection + Pure Fabrication
    //Що було: один метод ProcessReservationConsole() робив усе: 
    //читав з консолі, створював об'єкти, рахував, формував чек, «зберігав у JSON», 
    //«слав SMS» і друкував
    // Пункт 2. DTO, тобто «конверти» з даними між UI і контролером.
    public sealed record OrderItemRequest(string Name, decimal Price, CourseType Course, int Count);
    public sealed record BookTableCommand(string GuestName, string Phone, int GuestsCount, TimeSlot TimeSlot, IReadOnlyList<OrderItemRequest> Items);
    public sealed record BookingResult(string GuestReceipt, string KitchenTicket);

    // Пункт 4. Indirection. Інтерфейси між контролером та інфраструктурою: контролер знає лише їх, а не файли чи SMS.
    public interface IReservationRepository { int NextId(); void Save(TableReservation reservation); }
    public interface INotificationSender { void Send(string phone, string message); }
    public interface IReceiptFormatter
    {
        string FormatForGuest(TableReservation r);
        string FormatForKitchen(TableReservation r);
        string FormatSms(TableReservation r);
    }
    public interface IBookingStep { void Execute(BookingContext context); } // "обробник команди"

    public sealed class BookingContext
    {
        public BookTableCommand Command { get; }
        public TableReservation Reservation { get; set; }
        public BookingResult Result { get; set; }
        public BookingContext(BookTableCommand command) { Command = command; }
    }

    // Пункт 3. Controller. Він не містить ні Console, ні розрахунків, лише проганяє кроки по черзі.
    public sealed class ReservationController
    {
        private readonly IReadOnlyList<IBookingStep> _steps;
        public ReservationController(IReadOnlyList<IBookingStep> steps) { _steps = steps; }

        public BookingResult BookTable(BookTableCommand command)
        {
            var context = new BookingContext(command);
            foreach (var step in _steps) step.Execute(context);
            return context.Result;
        }
    }

    // Кроки use-case
    public sealed class CreateReservationStep : IBookingStep
    {
        private readonly IReservationRepository _repository;
        private readonly IPricingPolicyProvider _pricing;
        public CreateReservationStep(IReservationRepository repository, IPricingPolicyProvider pricing)
        { _repository = repository; _pricing = pricing; }

        public void Execute(BookingContext context)
        {
            var cmd = context.Command;
            var reservation = TableReservation.Create(_repository.NextId(), Guest.Create(cmd.GuestName, cmd.Phone),
                cmd.GuestsCount, cmd.TimeSlot, _pricing.For(cmd.TimeSlot));

            foreach (var line in cmd.Items)
                reservation.AddItem(MenuItem.Create(line.Name, line.Price, line.Course), line.Count);

            reservation.Confirm();
            context.Reservation = reservation;
        }
    }

    public sealed class SaveReservationStep : IBookingStep
    {
        private readonly IReservationRepository _repository;
        public SaveReservationStep(IReservationRepository repository) { _repository = repository; }
        public void Execute(BookingContext context) => _repository.Save(context.Reservation);
    }

    public sealed class NotifyGuestStep : IBookingStep
    {
        private readonly INotificationSender _sender;
        private readonly IReceiptFormatter _formatter;
        public NotifyGuestStep(INotificationSender sender, IReceiptFormatter formatter) { _sender = sender; _formatter = formatter; }

        public void Execute(BookingContext context)
        {
            var r = context.Reservation;
            context.Result = new BookingResult(_formatter.FormatForGuest(r), _formatter.FormatForKitchen(r));
            _sender.Send(r.GuestPhone, _formatter.FormatSms(r));
        }
    }

    // Пункт 5. Pure Fabrication. Технічні класи, яких немає в реальному ресторані.
    public sealed class TxtReservationRepository : IReservationRepository
    {
        private readonly string _path;
        public TxtReservationRepository(string path) { _path = path; }

        public int NextId() => File.Exists(_path) ? File.ReadLines(_path).Count() + 1 : 1;

        // один рядок тексту на бронювання
        public void Save(TableReservation r) =>
            File.AppendAllText(_path,
                $"#{r.ReservationId} | {r.GuestName} | {r.GuestPhone} | {r.GuestsCount} осіб | {r.TimeSlot} | {r.Status} | " +
                $"страви: {string.Join(", ", r.PreOrderedItems.Select(l => $"{l.ItemName} x{l.Count}"))} | " +
                $"сума {r.FoodSubtotal:F2} | збір {r.ServiceCharge:F2} | всього {r.TotalBill:F2} | депозит {r.Deposit:F2}" +
                Environment.NewLine);
    }

    public sealed class SmsNotificationSender : INotificationSender
    {
        public void Send(string phone, string message) => Console.WriteLine($"[SMS на {phone}] {message}");
    }

    public sealed class TextReceiptFormatter : IReceiptFormatter
    {
        public string FormatForGuest(TableReservation r) =>
            $"Чек резерву #{r.ReservationId}\nГість: {r.GuestName} ({r.GuestsCount} осіб)\n" +
            $"Страви: {r.FoodSubtotal:F2} грн\nОбслуговування: {r.ServiceCharge:F2} грн\n" +
            $"Всього до сплати: {r.TotalBill:F2} грн\nДепозит (внести зараз): {r.Deposit:F2} грн";

        public string FormatForKitchen(TableReservation r)
        {
            var sb = new StringBuilder($"Для кухні (бронювання #{r.ReservationId}, {r.TimeSlot}):");
            foreach (var line in r.PreOrderedItems)
                sb.Append($"\n - {line.ItemName} x{line.Count} ({line.CourseType})");
            return sb.ToString();
        }

        public string FormatSms(TableReservation r) =>
            $"Бронювання #{r.ReservationId} підтверджено. До сплати {r.TotalBill:F2} грн, депозит {r.Deposit:F2} грн.";
    }

    public sealed class ConsoleCommandParser
    {
        public TimeSlot ParseTimeSlot(string input)
        {
            if (Enum.TryParse(input?.Trim(), true, out TimeSlot slot) && Enum.IsDefined(typeof(TimeSlot), slot)) return slot;
            throw new FormatException("Введіть HappyHours, Standard або Evening.");
        }
    }

    // Пункт 1. Тонкий консольний UI. Він лише питає ім'я, читає слот, викликає контролер і друкує результат, без жодних розрахунків
    public sealed class ConsoleUi
    {
        private readonly ReservationController _controller;
        private readonly ConsoleCommandParser _parser;
        public ConsoleUi(ReservationController controller, ConsoleCommandParser parser) { _controller = controller; _parser = parser; }

        public void Run()
        {
            Console.WriteLine("Бронювання столика в ресторані");
            Console.Write("Введіть ім'я гостя: ");
            string name = Console.ReadLine();

            TimeSlot slot;
            while (true)
            {
                Console.Write("Введіть час відвідування (HappyHours/Standard/Evening): ");
                try { slot = _parser.ParseTimeSlot(Console.ReadLine()); break; }
                catch (FormatException ex) { Console.WriteLine("  ! " + ex.Message); }
            }

            // Готові дані (як у вихідному коді)
            var items = new[]
            {
                new OrderItemRequest("Стейк Рібай", 650m, CourseType.Main, 4),
                new OrderItemRequest("Вино Червоне", 400m, CourseType.Drink, 2)
            };

            try
            {
                var result = _controller.BookTable(new BookTableCommand(name, "+380631234567", 6, slot, items));
                Console.WriteLine($"\n{result.GuestReceipt}\n\n{result.KitchenTicket}");
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                Console.WriteLine("Помилка бронювання: " + ex.Message);
            }
        }
    }

    // Composition root. У Main ми збираємо кроки й передаємо їх контролеру
    //Тут три кроки, і вони йдуть саме в такому порядку: створити бронювання, зберегти, повідомити гостя
    //До цього один метод робив усе. 
    //Тепер консоль тільки спілкується з людиною, контролер тільки керує порядком дій, сутності тільки рахують і перевіряють, а збереження, SMS і чек винесені в окремі класи за інтерфейсами
    public static class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            IReservationRepository repository = new TxtReservationRepository("reservations.txt");
            IReceiptFormatter formatter = new TextReceiptFormatter();

            var controller = new ReservationController(new IBookingStep[]
            {
                new CreateReservationStep(repository, new PricingPolicyProvider()),
                new SaveReservationStep(repository),
                new NotifyGuestStep(new SmsNotificationSender(), formatter)
            });

            new ConsoleUi(controller, new ConsoleCommandParser()).Run();
        }
    }
}