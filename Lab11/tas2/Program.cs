using System;
using System.Text;

namespace Lab11.Task2;
//Завдання 2 — ADAPTER (підключення сторонньої платіжної системи)
//як розпізнати шаблон за описом:
//-«Є ГОТОВИЙ сторонній об'єкт зі СВОЇМ інтерфейсом, який змінити
//неможливо, а наш код очікує ІНШИЙ інтерфейс» -> Adapter.
//-Платформа приймає центи й код валюти, повертає відповідь у власному
//форматі. Магазин гри працює з гривнями та власним результатом покупки.
//-Рішення: клас-«перекладач» (адаптер) між ними. Він реалізує інтерфейс
//магазину, а всередині викликає платформу і перетворює суму та відповідь.
//
//  ролі в шаблоні:
//   IPaymentProcessor                — Target (інтерфейс, який очікує магазин)
//   CurrencyShop                     — Client (знає ЛИШЕ Target)
//   PayFlowPlatform, GlobalPayGateway — Adaptee (сторонні класи, їх не чіпаємо)
//   PayFlowAdapter, GlobalPayAdapter — Adapter (перетворюють виклики й відповіді)
//
//  чим Adapter відрізняється від інших структурних шаблонів):
//   Adapter  — ЗМІНЮЄ інтерфейс (перекладач: «ваш формат -> наш формат»).

//
//де в журналі видно, хто що робить:
//[PayFlowAdapter]— обгортка (адаптер): перетворює суму й відповідь;
//[PayFlow (зовнішня)]— обгорнутий об'єкт: сама платформа виконує платіж.


public static class Logger
{
    public static void Log(string source, string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{source}] {message}");
    }
}

//Гравець
public class Player
{
    public string Name { get; }
    public int Balance { get; set; }   // кількість ігрової валюти

    public Player(string name)
    {
        Name = name;
    }
}

//  TARGET: власний інтерфейс магазину гри (гривні + ігрова валюта)

// Результат покупки у ФОРМАТІ МАГАЗИНУ
public class PurchaseResult
{
    public bool Success { get; }
    public string TransactionId { get; }
    public string Message { get; }

    public PurchaseResult(bool success, string transactionId, string message)
    {
        Success = success;
        TransactionId = transactionId;
        Message = message;
    }

    public override string ToString() =>
        Success ? $"УСПІХ (транзакція {TransactionId})" : $"ВІДМОВА ({Message})";
}

// Target: єдине, що знає магазин про оплату
public interface IPaymentProcessor
{
    // сума в гривнях і кількість ігрової валюти — як у вимогах лаби
    PurchaseResult Purchase(string playerName, decimal amountUah, int gameCurrencyAmount);
}

//  ADAPTEE №1: стороння платформа PayFlow ( КОД ЗМІНЮВАТИ НЕМОЖЛИВО)
//  Приймає суму в ЦЕНТАХ і ВАЛЮТУ КОДОМ, відповідь — у власному форматі.
public class PayFlowResponse
{
    public string Status { get; set; } = "";      // "APPROVED" або "DECLINED"
    public string Reference { get; set; } = "";   // номер платежу в системі PayFlow
    public int ErrorCode { get; set; }            // 0 = без помилки
}

public class PayFlowPlatform
{
    private int _counter = 1000;

    // Чужа назва методу й чужі параметри
    public PayFlowResponse ExecuteCharge(long amountInCents, string currencyCode, string customerRef)
    {
        Logger.Log("PayFlow (зовнішня)",
            $"ExecuteCharge(amountInCents={amountInCents}, currencyCode={currencyCode}, customerRef={customerRef})");

        // Ліміт платформи: не більше 2000 грн за один платіж
        if (amountInCents > 200_000)
        {
            Logger.Log("PayFlow (зовнішня)", "Відповідь: Status=DECLINED, ErrorCode=51");
            return new PayFlowResponse { Status = "DECLINED", ErrorCode = 51 };
        }

        string reference = $"PF-{++_counter}";
        Logger.Log("PayFlow (зовнішня)", $"Відповідь: Status=APPROVED, Reference={reference}");
        return new PayFlowResponse { Status = "APPROVED", Reference = reference };
    }
}
//  ADAPTER №1: перекладає з мови магазину на мову PayFlow і назад
//  уся логіка перетворення (гривні -> центи, відповідь -> результат)
//  живе ТУТ, в одному місці. Магазин про це нічого не знає.
public class PayFlowAdapter : IPaymentProcessor
{
    private readonly PayFlowPlatform _platform;   // обгорнутий об'єкт (Adaptee)

    public PayFlowAdapter(PayFlowPlatform platform)
    {
        _platform = platform;
    }

    public PurchaseResult Purchase(string playerName, decimal amountUah, int gameCurrencyAmount)
    {
        // 1. Перетворюємо ЗАПИТ: гривні -> центи, валюта -> код
        long cents = (long)Math.Round(amountUah * 100m);
        Logger.Log("PayFlowAdapter",
            $"Перетворюю запит: {amountUah:F2} грн -> {cents} центів, валюта \"UAH\"");

        // 2. Викликаємо сторонню платформу в ЇЇ форматі
        PayFlowResponse response = _platform.ExecuteCharge(cents, "UAH", playerName);

        // 3. Перетворюємо ВІДПОВІДЬ у формат магазину
        PurchaseResult result = response.Status == "APPROVED"
            ? new PurchaseResult(true, response.Reference, "")
            : new PurchaseResult(false, "", $"платформа відхилила платіж, код {response.ErrorCode}");

        Logger.Log("PayFlowAdapter", $"Перетворюю відповідь: Status={response.Status} -> {result}");
        return result;
    }
}

//  ADAPTEE №2: ще одна стороння платформа GlobalPay (інший формат!)
//  Приймає суму ЧИСЛОМ З ПЛАВАЮЧОЮ КРАПКОЮ в основних одиницях,
//  відповідь — РЯДОК виду "SUCCESS|GP-9001" або "FAIL|причина".
public class GlobalPayGateway
{
    private int _counter = 9000;

    public string Pay(string currency, double value, string account)
    {
        Logger.Log("GlobalPay (зовнішня)", $"Pay(currency={currency}, value={value:F2}, account={account})");

        if (value > 5000)
        {
            Logger.Log("GlobalPay (зовнішня)", "Відповідь: \"FAIL|Ліміт платежу перевищено\"");
            return "FAIL|Ліміт платежу перевищено";
        }

        string answer = $"SUCCESS|GP-{++_counter}";
        Logger.Log("GlobalPay (зовнішня)", $"Відповідь: \"{answer}\"");
        return answer;
    }
}
//  ADAPTER №2: додаємо другу платформу ЛИШЕ новим класом.
//  Клас CurrencyShop (магазин) НЕ змінюється — це вимога лаби.
public class GlobalPayAdapter : IPaymentProcessor
{
    private readonly GlobalPayGateway _gateway;

    public GlobalPayAdapter(GlobalPayGateway gateway)
    {
        _gateway = gateway;
    }

    public PurchaseResult Purchase(string playerName, decimal amountUah, int gameCurrencyAmount)
    {
        // 1. Перетворюємо запит: decimal -> double
        Logger.Log("GlobalPayAdapter", $"Перетворюю запит: {amountUah:F2} грн -> double, валюта \"UAH\"");

        // 2. Викликаємо платформу в її форматі
        string answer = _gateway.Pay("UAH", (double)amountUah, playerName);

        // 3. Розбираємо відповідь-рядок "СТАТУС|значення" у результат магазину
        string[] parts = answer.Split('|');
        PurchaseResult result = parts[0] == "SUCCESS"
            ? new PurchaseResult(true, parts[1], "")
            : new PurchaseResult(false, "", parts[1]);

        Logger.Log("GlobalPayAdapter", $"Перетворюю відповідь \"{answer}\" -> {result}");
        return result;
    }
}

//  CLIENT: магазин ігрової валюти
//  у цьому класі НЕМАЄ слів PayFlow/GlobalPay, центів чи перетворень.
//  Він працює ЛИШЕ з IPaymentProcessor, тому нова платформа = новий адаптер,
//  а цей клас лишається без змін.
public class CurrencyShop
{
    private const decimal PricePerUnitUah = 0.5m;   // 1 одиниця валюти = 0,50 грн
    private readonly IPaymentProcessor _payments;

    public CurrencyShop(IPaymentProcessor payments)
    {
        _payments = payments;
    }

    public bool BuyCurrency(Player player, int amount)
    {
        decimal price = amount * PricePerUnitUah;
        Logger.Log("Магазин", $"{player.Name} купує {amount} од. ігрової валюти за {price:F2} грн");

        PurchaseResult result = _payments.Purchase(player.Name, price, amount);
        Logger.Log("Магазин", $"Результат у форматі магазину: {result}");

        if (!result.Success)
        {
            Logger.Log("Магазин", "Валюту не нараховано");
            return false;
        }

        player.Balance += amount;
        Logger.Log("Магазин", $"Нараховано {amount} од. гравцю {player.Name}, баланс: {player.Balance}");
        return true;
    }
}

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;   // щоб кирилиця виводилась коректно
        Situation2_Adapter();
    }

    private static void Situation2_Adapter()
    {
        Console.WriteLine("Завдання 2: стороння платіжна система (Adapter)");

        var player = new Player("Storm");

        // Магазин №1: платформа PayFlow через адаптер
        Logger.Log("Server", "Магазин підключено до платформи PayFlow (через PayFlowAdapter)");
        var shop = new CurrencyShop(new PayFlowAdapter(new PayFlowPlatform()));

        // Крок 1-2: купити 500 одиниць, у журналі виклик платформи в її форматі
        shop.BuyCurrency(player, 500);

        // Додатково: платіж, який платформа відхиляє (2500 грн > ліміт 2000 грн)
        Logger.Log("Server", "Додатково: покупка, яку платформа відхиляє");
        shop.BuyCurrency(player, 5000);

        // Крок 3: друга платформа, той самий код магазину
        Logger.Log("Server", "Підключаємо другу платформу GlobalPay новим класом GlobalPayAdapter");
        var shop2 = new CurrencyShop(new GlobalPayAdapter(new GlobalPayGateway()));
        shop2.BuyCurrency(player, 500);

        Logger.Log("Server", $"Підсумковий баланс гравця {player.Name}: {player.Balance}");
    }
}