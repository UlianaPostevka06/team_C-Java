
namespace Variant6_DocumentApprovalWorkflow
{
    // Опис предметної області: Корпоративна система електронного документообігу та узгодження.
    // Завдання коду: Узгодження документа керівником, перевірка прав підписанта через Active Directory, оновлення пошукового індексу Elasticsearch та надсилання сповіщення у робочий чат Slack.

    public class Document
    {
        public string DocumentId { get; set; }
        public string Title { get; set; }
        public string AuthorUsername { get; set; }
        public string ApproverUsername { get; set; }
    }

    public class ElasticsearchDocumentIndex
    {
        public void IndexDocument(string docId, string title, string status)
        {
            Console.WriteLine($"[Elasticsearch Index] Документ {docId} ('{title}') перевиндексовано зі статусом '{status}'.");
        }
    }

    public class ActiveDirectoryUserValidator
    {
        public bool ValidateUserSignature(string username)
        {
            Console.WriteLine($"[Active Directory LDAP] Користувача '{username}' підтверджено у групі 'Managers'.");
            return true;
        }
    }

    public class SlackWebhookNotifier
    {
        public void PostChannelMessage(string channel, string message)
        {
            Console.WriteLine($"[Slack Webhook] Повідомлення у канал #{channel}: {message}");
        }
    }

    public class DocumentApprovalWorkflow
    {
        private readonly ElasticsearchDocumentIndex _searchIndex;
        private readonly ActiveDirectoryUserValidator _adValidator;
        private readonly SlackWebhookNotifier _slackNotifier;

        public DocumentApprovalWorkflow()
        {
            _searchIndex = new ElasticsearchDocumentIndex();
            _adValidator = new ActiveDirectoryUserValidator();
            _slackNotifier = new SlackWebhookNotifier();
        }

        public void ApproveDocument(Document doc)
        {
            if (doc == null || string.IsNullOrEmpty(doc.DocumentId))
            {
                throw new ArgumentException("Некоректний документ для узгодження.");
            }

            bool isValidApprover = _adValidator.ValidateUserSignature(doc.ApproverUsername);
            if (!isValidApprover)
            {
                throw new InvalidOperationException("Підписант не має повноважень для підпису.");
            }

            _searchIndex.IndexDocument(doc.DocumentId, doc.Title, "APPROVED");
            _slackNotifier.PostChannelMessage("approvals", $"Документ '{doc.Title}' (ID: {doc.DocumentId}) успішно підписано користувачем {doc.ApproverUsername}.");
        }
    }
}