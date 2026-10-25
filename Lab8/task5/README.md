## Завдання 5. Порівняльна таблиця метрик

| Клас | CBO до | CBO після | LCOM4 до | LCOM4 після |
|---|---|---|---|---|
| `LegacyRestaurantProcessor` | 4 | не застосовується | 1 | не застосовується |
| `ReservationController` | не застосовується | 4 | не застосовується | 1 |
| `TableReservation` | 2 | 4 | не визначений (методів немає) | 1 |
| `TextReceiptFormatter` | не застосовується | 2 | не застосовується | 3 |

«Не застосовується» означає, що класу не було до рефакторингу або він зник після нього. `LegacyRestaurantProcessor` розділено на контролер, кроки, репозиторій і форматер. У `TableReservation` до рефакторингу були лише `get; set;`, методів немає, тому LCOM4 для неї не визначений. Enum у CBO не рахуємо; з enum `TableReservation` має 6.

Додаткові класи, які з'явилися після рефакторингу:

| Клас | CBO | CBO (з enum) | LCOM4 | Ціль досягнута |
|---|---|---|---|---|
| `OrderLine` | 1 | 2 | 1 | так |
| `MenuItem` | 0 | 1 | 1 | так |
| `Guest` | 0 | 0 | 1 | так |
| `HappyHoursDiscount`, `LargeGroupServiceCharge`, `LargeGroupDeposit` | 1 | 1 | 1 | так |
| `PricingPolicySet` | 3 | 3 | 3 | ні (свідомий компроміс) |
| `TextReceiptFormatter` | 2 | 2 | 3 | ні (немає спільного стану) |
