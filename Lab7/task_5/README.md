# Завдання 5. Dependency Inversion Principle (DIP) — Варіант 6: Document Approval Workflow

## 1. Предметна область

Корпоративна система електронного документообігу: керівник узгоджує документ. Система перевіряє права підписанта (Active Directory), оновлює пошуковий індекс (Elasticsearch) і надсилає сповіщення в робочий чат (Slack).

## 2. Проблеми вихідного («поганого») коду

Клас `DocumentApprovalWorkflow` сам створював усі свої залежності через `new` у конструкторі.

| № | Проблема | Де проявляється | Порушений принцип |
|---|----------|-----------------|-------------------|
| 1 | Високорівневий клас бізнес-логіки напряму залежить від низькорівневих реалізацій (`ElasticsearchDocumentIndex`, `ActiveDirectoryUserValidator`, `SlackWebhookNotifier`) | поля й конструктор `DocumentApprovalWorkflow` | **DIP** (головне порушення завдання) |
| 2 | Конструктор сам виконує `new` для трьох об'єктів: жорстке зв'язування (Tight Coupling) | `DocumentApprovalWorkflow()` | DIP |
| 3 | Підмінити реалізацію (наприклад, Slack → Teams, Elasticsearch → БД) неможливо без правки класу бізнес-логіки | весь клас | DIP, OCP |
| 4 | Модульне тестування неможливе без реальних AD, Elasticsearch і Slack | весь клас | тестованість |
| 5 | Перевірка `string.IsNullOrEmpty` пропускає ідентифікатор із самих пробілів | `ApproveDocument` | fail-fast, коректність |

### Чому це проблема на практиці

- **Неможливість тестувати.** Щоб перевірити логіку затвердження, потрібні робочі AD, пошуковий кластер і Slack.
- **Складна зміна технологій.** Заміна одного сервісу змушує редагувати й повторно тестувати клас бізнес-логіки.
- **Зворотний напрямок залежностей.** Бізнес-правила, які мають бути стабільними, залежать від технічних деталей, що змінюються частіше.
- **Приховані залежності.** За конструктором без параметрів не видно, що клас потребує три зовнішні системи.

## 3. Що було зроблено

### 3.1. Нова структура

| Тип | Роль |
|---|---|
| `IDocumentIndex` | Абстракція пошукового індексу (`IndexDocument`) |
| `IUserValidator` | Абстракція перевірки прав підписанта (`ValidateUserSignature`) |
| `INotificationService` | Абстракція надсилання повідомлень (`PostChannelMessage`) |
| `ElasticsearchDocumentIndex`, `ActiveDirectoryUserValidator`, `SlackWebhookNotifier` | **Production**-реалізації: тепер реалізують інтерфейси |
| `MockDocumentIndex`, `MockUserValidator`, `MockNotificationService` | **Test**-реалізації для демонстрації підміни |
| `DocumentApprovalWorkflow` | Високорівнева бізнес-логіка: залежить лише від інтерфейсів, залежності отримує через конструктор |
| `Program.Main` | **Composition Root**: єдине місце, де створюються конкретні об'єкти й збираються залежності |

### 3.2. Було і стало (ключовий фрагмент)

**Було:**

```csharp
private readonly ElasticsearchDocumentIndex _searchIndex;
private readonly ActiveDirectoryUserValidator _adValidator;
private readonly SlackWebhookNotifier _slackNotifier;

public DocumentApprovalWorkflow()
{
    _searchIndex = new ElasticsearchDocumentIndex();
    _adValidator = new ActiveDirectoryUserValidator();
    _slackNotifier = new SlackWebhookNotifier();
}
```

**Стало:**

```csharp
private readonly IDocumentIndex _searchIndex;
private readonly IUserValidator _adValidator;
private readonly INotificationService _slackNotifier;

public DocumentApprovalWorkflow(
    IDocumentIndex searchIndex,
    IUserValidator adValidator,
    INotificationService slackNotifier)
{
    _searchIndex = searchIndex ?? throw new ArgumentNullException(nameof(searchIndex));
    _adValidator = adValidator ?? throw new ArgumentNullException(nameof(adValidator));
    _slackNotifier = slackNotifier ?? throw new ArgumentNullException(nameof(slackNotifier));
}
```

**Підміна реалізацій у точці збірки:**

```csharp
// Production
var productionWorkflow = new DocumentApprovalWorkflow(
    new ElasticsearchDocumentIndex(),
    new ActiveDirectoryUserValidator(),
    new SlackWebhookNotifier());

// Test: той самий клас бізнес-логіки, інші залежності
var testWorkflow = new DocumentApprovalWorkflow(
    new MockDocumentIndex(),
    new MockUserValidator(),
    new MockNotificationService());
```

## 4. Застосування принципів SOLID

### D: Dependency Inversion (основне завдання)

- `DocumentApprovalWorkflow` залежить від абстракцій `IDocumentIndex`, `IUserValidator` і `INotificationService`, а не від конкретних класів.
- Низькорівневі класи також залежать від абстракцій: вони реалізують ці інтерфейси. Напрямок залежностей інвертовано.
- Залежності передаються через конструктор (**Constructor Injection**) і перевіряються на `null` (fail-fast).
- Конкретні об'єкти створюються лише в `Main` (Composition Root). Service Locator не використовується.
- Реалізації підміняються без жодної зміни в `DocumentApprovalWorkflow`, що демонструють режими Production і Test.

### S: Single Responsibility

Клас бізнес-логіки лише координує узгодження. Індексація, перевірка прав і сповіщення ізольовані у своїх сервісах.

### O: Open/Closed

Новий канал сповіщень (наприклад, Teams) додається новою реалізацією `INotificationService` без змін в існуючому коді.

### L: Liskov Substitution

Production- та Mock-реалізації взаємозамінні: клас бізнес-логіки працює з будь-якою з них однаково.

### I: Interface Segregation

Кожен інтерфейс має один метод і відповідає одній ролі, тому порожніх методів-заглушок немає.

## 5. Прагматичні принципи

### DRY

- Кожна залежність має єдину абстракцію-контракт. Бізнес-логіка знає про сервіс лише через один інтерфейс.
- Перевірка вхідних залежностей зосереджена в одному місці: конструкторі класу.

### KISS

- Метод `ApproveDocument` має лінійний вигляд: перевірка документа → перевірка підписанта → індексація → сповіщення. Guard Clauses виключають вкладеність.
- Збірка виконується вручну (Manual DI) без IoC-контейнера: для трьох залежностей цього достатньо.

### YAGNI

- Інтерфейси необхідні: вони вимога DIP і дають змогу підставляти Mock-об'єкти. Кожен має реальну реалізацію в двох варіантах (Production і Test).
- Не додано фабрик, IoC-контейнера, конфігураційних файлів та Service Locator.
- Немає заглушок «на майбутнє» й методів із `NotImplementedException`.

## 6. Відмінності від вихідної поведінки

Послідовність кроків і тексти виводу не змінилися. Свідомо змінено таке:

1. Конструктор приймає залежності й кидає `ArgumentNullException`, якщо якусь не передано. Конструктора без параметрів більше немає.
2. Перевірку `string.IsNullOrEmpty(doc.DocumentId)` замінено на `string.IsNullOrWhiteSpace`: тепер ідентифікатор із самих пробілів вважається некоректним.
3. Додано Mock-реалізації та `Main`, яка демонструє підміну (Production-режим і Test-режим).
4. Імена полів `_adValidator` і `_slackNotifier` залишені з вихідного коду, хоча тип поля вже абстрактний. За бажанням їх можна перейменувати (наприклад, `_userValidator`, `_notifier`), щоб імена не вказували на конкретні технології.
5. Метод інтерфейсу `PostChannelMessage` із параметром `channel` відображає термінологію Slack. Це простий компроміс: інтерфейс залишено сумісним із вихідним кодом, щоб не змінювати його сигнатури.
