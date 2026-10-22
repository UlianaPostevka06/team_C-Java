using System;
using System.IO;
using System.Net.Mail;

namespace Variant6_LibraryLoanManager
{
    // Доменна модель
    public class BookReturnModel
    {
        public string ReaderTicketNumber { get; set; } = string.Empty;
        public string ReaderName { get; set; } = string.Empty;
        public string ReaderEmail { get; set; } = string.Empty;
        public string BookIsbn { get; set; } = string.Empty;
        public string BookTitle { get; set; } = string.Empty;
        public DateTime ExpectedReturnDate { get; set; }
        public DateTime ActualReturnDate { get; set; }
    }

    // Результат розрахунку: що саме нарахувала бібліотека за повернення
    public record ReturnAssessment(int OverdueDays, decimal FineAmount, int LoyaltyPoints)
    {
        public bool IsOverdue => OverdueDays > 0;
    }

    // Валідація (причина для зміни: правила перевірки даних) 
    public class LoanValidator
    {
        public void Validate(BookReturnModel loan)
        {
            if (string.IsNullOrWhiteSpace(loan.ReaderTicketNumber))
            {
                throw new ArgumentException("Номер читацького квитка відсутній.");
            }

            if (string.IsNullOrWhiteSpace(loan.BookIsbn))
            {
                throw new ArgumentException("Код ISBN книги обов'язковий.");
            }

            if (loan.ActualReturnDate < loan.ExpectedReturnDate.AddYears(-1))
            {
                throw new InvalidOperationException("Неможлива дата повернення книги.");
            }
        }
    }

    // Бізнес-логіка (причина для зміни: тарифи та бонуси) 
    public class FineCalculator
    {
        private const decimal FinePerDay = 15.00m;   // грн за кожен день прострочення
        private const int OnTimeBonusPoints = 10;    // бонус за вчасне повернення

        public ReturnAssessment Calculate(BookReturnModel loan)
        {
            int overdueDays = (loan.ActualReturnDate - loan.ExpectedReturnDate).Days;

            return overdueDays > 0
                ? new ReturnAssessment(overdueDays, overdueDays * FinePerDay, 0)
                : new ReturnAssessment(overdueDays, 0, OnTimeBonusPoints);
        }
    }

    // Форматування (причина для зміни: вигляд і текст повідомлення)

    public class NoticeFormatter
    {
        public string Format(BookReturnModel loan, ReturnAssessment assessment)
        {
            string notice = "БІБЛІОТЕЧНИЙ АКТ ПОВЕРНЕННЯ КНИГИ\n";
            notice += $"Читач: {loan.ReaderName} (Квиток №{loan.ReaderTicketNumber})\n";
            notice += $"Книга: «{loan.BookTitle}» (ISBN: {loan.BookIsbn})\n";
            notice += $"Планова дата: {loan.ExpectedReturnDate:dd.MM.yyyy} | Фактична: {loan.ActualReturnDate:dd.MM.yyyy}\n";

            notice += assessment.IsOverdue
                ? $"УВАГА! Прострочення: {assessment.OverdueDays} днів. Штраф до сплати: {assessment.FineAmount:F2} грн.\n"
                : $"Дякуємо за вчасне повернення! Нараховано бонусних балів: {assessment.LoyaltyPoints}\n";

            return notice;
        }
    }

    // Збереження (причина для зміни: формат і місце зберігання каталогу)
    public class CatalogRepository
    {
        private const string FileName = "library_catalog_history.txt";

        public void SaveReturn(BookReturnModel loan)
        {
            string record = $"{loan.BookIsbn}|RETURNED|{loan.ActualReturnDate:yyyy-MM-dd}|By_{loan.ReaderTicketNumber}\n";
            File.AppendAllText(FileName, record);
        }
    }

    // Сповіщення (причина для зміни: канал і параметри доставки) 
    public class EmailNotifier
    {
        public void Send(BookReturnModel loan, ReturnAssessment assessment, string noticeText)
        {
            try
            {
                using (var mailClient = new SmtpClient("smtp.citylibrary.local"))
                {
                    var mail = new MailMessage("loans@citylibrary.local", loan.ReaderEmail)
                    {
                        Subject = assessment.IsOverdue
                            ? "Повідомлення про штраф за прострочення книги"
                            : "Підтвердження повернення книги",
                        Body = noticeText
                    };
                    mailClient.Send(mail);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не вдалося відправити квитанцію на пошту: {ex.Message}");
            }
        }
    }

    // Оркестратор: лише координує кроки, сам нічого не рахує 

    public class LibraryLoanService
    {
        private readonly LoanValidator _validator;
        private readonly FineCalculator _calculator;
        private readonly NoticeFormatter _formatter;
        private readonly CatalogRepository _repository;
        private readonly EmailNotifier _notifier;

        public LibraryLoanService(
            LoanValidator validator,
            FineCalculator calculator,
            NoticeFormatter formatter,
            CatalogRepository repository,
            EmailNotifier notifier)
        {
            _validator = validator;
            _calculator = calculator;
            _formatter = formatter;
            _repository = repository;
            _notifier = notifier;
        }

        public void ProcessBookReturn(BookReturnModel loan)
        {
            _validator.Validate(loan);

            ReturnAssessment assessment = _calculator.Calculate(loan);
            string notice = _formatter.Format(loan, assessment);
            Console.WriteLine(notice);

            _repository.SaveReturn(loan);
            _notifier.Send(loan, assessment, notice);
        }
    }


    public class Program
    {
        public static void Main()
        {
            var service = new LibraryLoanService(
                new LoanValidator(),
                new FineCalculator(),
                new NoticeFormatter(),
                new CatalogRepository(),
                new EmailNotifier());

            var loan = new BookReturnModel
            {
                ReaderTicketNumber = "12345",
                ReaderName = "Іван Петренко",
                ReaderEmail = "ivan@example.com",
                BookIsbn = "978-966-03-1234-5",
                BookTitle = "Кобзар",
                ExpectedReturnDate = new DateTime(2026, 10, 10),
                ActualReturnDate = new DateTime(2026, 10, 15)
            };

            try
            {
                service.ProcessBookReturn(loan);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
    }
}