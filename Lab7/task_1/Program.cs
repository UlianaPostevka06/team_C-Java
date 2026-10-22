<![CDATA[ 
    using System;
    using System.IO;
    using System.Net.Mail;

    namespace Variant6_LibraryLoanManager
    {
        // Опис предметної області: Автоматизована система міської бібліотеки.
        // Завдання коду: Обробка повернення та видачі книг — перевірка читацького квитка, розрахунок пені за прострочення та накопичувальних балів читача, формування нагадування або чека штрафу, оновлення статусу книги в каталозі та надсилання сповіщення боржнику електронною поштою.

        public class BookReturnModel
        {
            public string ReaderTicketNumber { get; set; }
            public string ReaderName { get; set; }
            public string ReaderEmail { get; set; }
            public string BookIsbn { get; set; }
            public string BookTitle { get; set; }
            public DateTime ExpectedReturnDate { get; set; }
            public DateTime ActualReturnDate { get; set; }
        }

        public class LibraryLoanService
        {
            public void ProcessBookReturn(BookReturnModel loan)
            {
                // 1. Перевірка даних читацького квитка та видачі
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

                // 2. Розрахунок прострочення, штрафних санкцій та бонусних балів
                int overdueDays = (loan.ActualReturnDate - loan.ExpectedReturnDate).Days;
                decimal fineAmount = 0;
                int loyaltyPointsEarned = 0;

                if (overdueDays > 0)
                {
                    // Тариф штрафу: 15 грн за кожен день прострочення
                    fineAmount = overdueDays * 15.00m;
                }
                else
                {
                    // Бонус за вчасне повернення
                    loyaltyPointsEarned = 10;
                }

                // 3. Генерація чека / офіційного повідомлення бібліотеки
                string noticeText = "БІБЛІОТЕЧНИЙ АКТ ПОВЕРНЕННЯ КНИГИ\n";
                noticeText += $"Читач: {loan.ReaderName} (Квиток №{loan.ReaderTicketNumber})\n";
                noticeText += $"Книга: "{loan.BookTitle}" (ISBN: {loan.BookIsbn})\n";
                noticeText += $"Планова дата: {loan.ExpectedReturnDate:dd.MM.yyyy} | Фактична: {loan.ActualReturnDate:dd.MM.yyyy}\n";

                if (overdueDays > 0)
                {
                    noticeText += $"УВАГА! Прострочення: {overdueDays} днів. Штраф до сплати: {fineAmount:F2} грн.\n";
                }
                else
                {
                    noticeText += $"Дякуємо за вчасне повернення! Нараховано бонусних балів: {loyaltyPointsEarned}\n";
                }

                // 4. Оновлення електронного каталогу (запис у файлову БД)
                string catalogRecord = $"{loan.BookIsbn}|RETURNED|{loan.ActualReturnDate:yyyy-MM-dd}|By_{loan.ReaderTicketNumber}\n";
                File.AppendAllText("library_catalog_history.txt", catalogRecord);

                // 5. Відправка квитанції читачеві
                try
                {
                    using (var mailClient = new SmtpClient("smtp.citylibrary.local"))
                    {
                        var mail = new MailMessage("loans@citylibrary.local", loan.ReaderEmail)
                        {
                            Subject = overdueDays > 0 ? "Повідомлення про штраф за прострочення книги" : "Підтвердження повернення книги",
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
    }
     ]]>