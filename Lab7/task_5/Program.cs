
using System;

namespace Variant6_DocumentApprovalWorkflow
{
    // Сутність документа
    public class Document
    {
        public string DocumentId { get; set; }
        public string Title { get; set; }
        public string AuthorUsername { get; set; }
        public string ApproverUsername { get; set; }
    }

    // 1. АБСТРАКЦІЇ (ІНТЕРФЕЙСИ)

    public interface IDocumentIndex
    {
        void IndexDocument(string docId, string title, string status);
    }

    public interface IUserValidator
    {
        bool ValidateUserSignature(string username);
    }

    public interface INotificationService
    {
        void PostChannelMessage(string channel, string message);
    }

    // 2. КОНКРЕТНІ РЕАЛІЗАЦІЇ СЕРВІСІВ

    // Індексування через Elasticsearch
    public class ElasticsearchDocumentIndex : IDocumentIndex
    {
        public void IndexDocument(string docId, string title, string status)
        {
            Console.WriteLine(
                $"[Elasticsearch Index] Документ {docId} ('{title}') " +
                $"перевиндексовано зі статусом '{status}'.");
        }
    }

    // Перевірка користувача через Active Directory
    public class ActiveDirectoryUserValidator : IUserValidator
    {
        public bool ValidateUserSignature(string username)
        {
            Console.WriteLine(
                $"[Active Directory LDAP] Користувача '{username}' " +
                "підтверджено у групі 'Managers'.");

            return true;
        }
    }

    // Надсилання повідомлення через Slack
    public class SlackWebhookNotifier : INotificationService
    {
        public void PostChannelMessage(string channel, string message)
        {
            Console.WriteLine(
                $"[Slack Webhook] Повідомлення у канал #{channel}: {message}");
        }
    }

    // 3. ТЕСТОВІ РЕАЛІЗАЦІЇ ДЛЯ ДЕМОНСТРАЦІЇ DIP

    public class MockDocumentIndex : IDocumentIndex
    {
        public void IndexDocument(string docId, string title, string status)
        {
            Console.WriteLine(
                $"[Mock Index] Документ {docId} отримав статус '{status}'.");
        }
    }

    public class MockUserValidator : IUserValidator
    {
        public bool ValidateUserSignature(string username)
        {
            Console.WriteLine(
                $"[Mock Validator] Користувача '{username}' перевірено.");

            return true;
        }
    }

    public class MockNotificationService : INotificationService
    {
        public void PostChannelMessage(string channel, string message)
        {
            Console.WriteLine(
                $"[Mock Notification] Канал #{channel}: {message}");
        }
    }

    // 4. ВИСОКОРІВНЕВИЙ КЛАС БІЗНЕС-ЛОГІКИ

    public class DocumentApprovalWorkflow
    {
        private readonly IDocumentIndex _searchIndex;
        private readonly IUserValidator _adValidator;
        private readonly INotificationService _slackNotifier;

        // Constructor Injection — залежності передаються через конструктор
        public DocumentApprovalWorkflow(
            IDocumentIndex searchIndex,
            IUserValidator adValidator,
            INotificationService slackNotifier)
        {
            _searchIndex = searchIndex
                ?? throw new ArgumentNullException(nameof(searchIndex));

            _adValidator = adValidator
                ?? throw new ArgumentNullException(nameof(adValidator));

            _slackNotifier = slackNotifier
                ?? throw new ArgumentNullException(nameof(slackNotifier));
        }

        public void ApproveDocument(Document doc)
        {
            if (doc == null || string.IsNullOrWhiteSpace(doc.DocumentId))
            {
                throw new ArgumentException(
                    "Некоректний документ для узгодження.");
            }

            bool isValidApprover =
                _adValidator.ValidateUserSignature(doc.ApproverUsername);

            if (!isValidApprover)
            {
                throw new InvalidOperationException(
                    "Підписант не має повноважень для підпису.");
            }

            _searchIndex.IndexDocument(
                doc.DocumentId, doc.Title, "APPROVED");

            _slackNotifier.PostChannelMessage(
                "approvals",
                $"Документ '{doc.Title}' (ID: {doc.DocumentId}) " +
                $"успішно підписано користувачем {doc.ApproverUsername}.");
        }
    }

    // 5. COMPOSITION ROOT — ТОЧКА ЗБІРКИ ЗАЛЕЖНОСТЕЙ

    public class Program
    {
        public static void Main(string[] args)
        {
            Document document = new Document
            {
                DocumentId = "DOC-001",
                Title = "Договір про співпрацю",
                AuthorUsername = "ivan.petrenko",
                ApproverUsername = "manager.smith"
            };

            Console.WriteLine("Production-режим");

            // Створюємо реальні реалізації сервісів
            IDocumentIndex productionIndex =
                new ElasticsearchDocumentIndex();

            IUserValidator productionValidator =
                new ActiveDirectoryUserValidator();

            INotificationService productionNotifier =
                new SlackWebhookNotifier();

            // Передаємо залежності у бізнес-логіку
            DocumentApprovalWorkflow productionWorkflow =
                new DocumentApprovalWorkflow(
                    productionIndex,
                    productionValidator,
                    productionNotifier);

            productionWorkflow.ApproveDocument(document);

            Console.WriteLine();
            Console.WriteLine("Test-режим");

            // Замінюємо реальні сервіси тестовими
            IDocumentIndex mockIndex = new MockDocumentIndex();
            IUserValidator mockValidator = new MockUserValidator();
            INotificationService mockNotifier =
                new MockNotificationService();

            DocumentApprovalWorkflow testWorkflow =
                new DocumentApprovalWorkflow(
                    mockIndex,
                    mockValidator,
                    mockNotifier);

            testWorkflow.ApproveDocument(document);
        }
    }
}
