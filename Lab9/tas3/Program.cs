using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Tas3;
//Проблема з опису: сервер створював нову арену під кожен матч (завантаження мапи, розставлення об'єктів, таймер) і знищував її після. 
//У пікові години арен ставало забагато, і ресурси закінчувалися.

//Рішення: арен рівно 3. Їх один раз підготували, і вони передаються від матчу до матчу, 
//як 3 ігрові кімнати, які просто прибирають між грами, а не будують наново

// Журнал: усі події у форматі [час] [джерело] подія
//Приклад виводу: [14:32:05] [A-1] Підготовка арени: завантаження мапи.... 
//Тут A-1 це джерело, решта це подія.
public static class Logger
{
    public static void Log(string source, string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{source}] {message}");
    }
}


// дві команди, які шукають матч. Це те, що стає в чергу, коли арен немає
//get щоб заявку після створення не змінити. Це для безпеки
public sealed class MatchRequest
{
    public string TeamA { get; }
    public string TeamB { get; }
    public string Name => $"{TeamA} vs {TeamB}";

    public MatchRequest(string teamA, string teamB)
    {
        TeamA = teamA;
        TeamB = teamB;
    }
}

// Arena — ПОВТОРНО ВИКОРИСТОВУВАНИЙ ОБ'ЄКТ (Reusable)
// Створення дороге: завантаження мапи, розставлення об'єктів, таймер.
public sealed class Arena
{
    public string Id { get; } //ідентифікатор(A-1)
    public string MapName { get; private set; } = "";//завантажена мапа("Північна долина")
    public List<string> Players { get; } = new();//гравці в матчі(Вовки_1, Ворони_2..)
    public List<string> DroppedItems { get; } = new();//предмети на арені("Зілля здоров'я")
    public int ScoreA { get; set; }//рахунок команд	(7 і 5)
    public int ScoreB { get; set; }//рахунок команд	(7 і 5)
    public int TimerSeconds { get; private set; }//таймер матчу(300)
    public MatchRequest? CurrentMatch { get; private set; }
    public int MatchesPlayed { get; private set; }//лічильник матчів(для статистики)

    public string Score => $"{ScoreA}:{ScoreB}";

    //підготовка відбувається лише при створенні. 
    //Арена створюється 3 рази за всю програму, тож і дорога підготовка відбудеться рівно 3 рази.
    public Arena(string id)
    {
        Id = id;
        Prepare(); 
    }

    private void Prepare()
    {
        Logger.Log(Id, "Підготовка арени: завантаження мапи...");
        Thread.Sleep(400);//завдання просить емулювати «дорогі» операції затримкою і повідомленням.
        // У грі це було б реальне завантаження мапи з диска, а тут пауза 0,4 с.
        MapName = "Північна долина";
        Logger.Log(Id, $"Мапу «{MapName}» завантажено, об'єкти розставлено, таймер налаштовано");
    }

    public void StartMatch(MatchRequest match)//заселяє гравців, ставить таймер на 300 с (5 хвилин) і пише в журнал
    {
        CurrentMatch = match;
        MatchesPlayed++;
        Players.Add($"{match.TeamA}_1");
        Players.Add($"{match.TeamA}_2");
        Players.Add($"{match.TeamB}_1");
        Players.Add($"{match.TeamB}_2");
        TimerSeconds = 300;
        Logger.Log(Id, $"Матч «{match.Name}» розпочато, рахунок {Score}, таймер {TimerSeconds} с, гравців: {Players.Count}");
    }

    // ключовий метод для вимоги «перед новим матчем стан арени скидається»
    //Приклад: перший матч закінчився 7:5, на землі лежало зілля. 
    //Після Reset() новий матч починається з 0:0 і порожньої арени
    public void Reset()
    {
        Players.Clear();
        DroppedItems.Clear();
        ScoreA = 0;
        ScoreB = 0;
        TimerSeconds = 0;
        CurrentMatch = null;
        // мапа НЕ перезавантажується — у цьому й сенс пулу
    }
}

// ArenaPool — ПУЛ ОБ'ЄКТІВ (Object Pool)
// Стратегія черги: FIFO — справедливо, команда, що чекає найдовше,
// отримує арену першою; проста й передбачувана, без голодування.
public sealed class ArenaPool
{
    private const int MaxArenas = 3;//константа, бо це фізичний ліміт сервера з опису. Її змінюють в одному місці.

    private readonly List<Arena> _all = new();          // усі створені арени, потрібні для статистики і для перевірки ліміту
    private readonly Queue<Arena> _free = new();        // вільні арени, які можна видати
    private readonly HashSet<Arena> _busy = new();      // зайняті арени, швидка перевірка «чи видавали цю»
    private readonly Queue<MatchRequest> _waiting = new(); // черга матчмейкінгу, коли арен немає

    public int TotalCreated => _all.Count;

    // Запит арени для матчу
    //Приклад: 5 пар шукають матч. 
    //Перші три створюють A-1, A-2, A-3, 
    //а четверта («Тури vs Соколи») і п'ята («Рисі vs Круки») йдуть у чергу
    public void RequestMatch(MatchRequest request)
    {
        Arena? arena = null;

        //Є вільна? Беремо з _free (повторне використання, і це головна економія)
        if (_free.Count > 0)
        {
            arena = _free.Dequeue();                    // повторне використання
        }
        //Немає вільної, але створено менше 3? Створюємо нову арену. Тому в нашому демо створюється саме 3: 
        //при перших трьох запитах вільних немає, а ліміт ще не досягнуто.
        else if (_all.Count < MaxArenas)
        {
            arena = new Arena($"A-{_all.Count + 1}");   // створюємо лише поки не досягнуто ліміту
            _all.Add(arena);
        }
        //Ні те, ні те? Ставимо заявку в чергу _waiting і пишемо в журнал позицію в черзі.
        if (arena == null)
        {
            _waiting.Enqueue(request);
            Logger.Log("Matchmaking",
                $"Вільних арен немає. «{request.Name}» у черзі (позиція {_waiting.Count})");
            return;
        }

        Assign(arena, request);
    }

    // Повернення арени в пул
    public void Release(Arena arena)
    {   //якщо арену не видавали, або її повертають двічі, це помилка. Захист від багів
        if (!_busy.Remove(arena))
            throw new InvalidOperationException($"Арена {arena.Id} не була видана");
        //Reset() викликається саме тут, а не при видачі. 
        //Так арена в пулі завжди «чиста», і неможливо забути її скинути.
        arena.Reset();
        Logger.Log("ArenaPool", $"{arena.Id} повернено, стан скинуто");
        //Далі розвилка: якщо в черзі хтось чекає, береться перший (Dequeue) і арена одразу видається йому. 
        //Якщо нікого немає, арена йде у _free
        if (_waiting.Count > 0)
        {
            var next = _waiting.Dequeue();              // FIFO(першим прийшов, першим пішов)
            Logger.Log("Matchmaking", $"З черги береться «{next.Name}»");
            Assign(arena, next);
        }
        else
        {
            _free.Enqueue(arena);
        }
    }

    private void Assign(Arena arena, MatchRequest request)
    {
        _busy.Add(arena);
        Logger.Log("ArenaPool", $"Арена {arena.Id} → матч {request.Name}");
        arena.StartMatch(request);
    }

    public Arena GetBusyArena(string id)
    {
        foreach (var a in _busy)
            if (a.Id == id) return a;
        throw new InvalidOperationException($"Арена {id} не зайнята");
    }

    public void PrintStatistics()
    {
        Logger.Log("ArenaPool", "-Статистика");
        foreach (var a in _all)
            Logger.Log("ArenaPool", $"{a.Id}: матчів проведено — {a.MatchesPlayed}");
        Logger.Log("ArenaPool", $"Загальна кількість створених арен: {TotalCreated}");
    }
}

// Раннер: окремий метод на кожну ситуацію
public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Situation3_ArenaPool();
    }

    private static void Situation3_ArenaPool()
    {
        Console.WriteLine("Завдання 3: розподіл арен для матчів (Object Pool)");
        var pool = new ArenaPool();

        var requests = new[]
        {
            new MatchRequest("Вовки", "Леви"),
            new MatchRequest("Ведмеді", "Яструби"),
            new MatchRequest("Соколи", "Орли"),
            new MatchRequest("Тигр", "Соколи"),
            new MatchRequest("Рисі", "Пантера"),
        };

        // 5 пар команд шукають матч одночасно: 3 отримують арени, 2 у черзі
        Logger.Log("Server", "5 пар команд одночасно шукають матч");
        foreach (var r in requests)
            pool.RequestMatch(r);

        // Перший матч грається: змінюємо стан арени A-1
        var a1 = pool.GetBusyArena("A-1");
        a1.ScoreA = 7;
        a1.ScoreB = 5;
        a1.DroppedItems.Add("Зілля здоров'я");
        Logger.Log("A-1", $"Матч завершується з рахунком {a1.Score}");

        // Арена A-1 повертається, стан скидається, її одразу отримує наступна пара
        pool.Release(a1);
        Logger.Log("A-1", $"Рахунок нового матчу {pool.GetBusyArena("A-1").Score}, " +
                          $"предметів на арені: {pool.GetBusyArena("A-1").DroppedItems.Count}");

        // Завершуємо A-2 — її отримує остання пара з черги
        pool.Release(pool.GetBusyArena("A-2"));

        // Завершуємо решту матчів
        pool.Release(pool.GetBusyArena("A-3"));
        pool.Release(pool.GetBusyArena("A-1"));
        pool.Release(pool.GetBusyArena("A-2"));

        pool.PrintStatistics();
    }
}