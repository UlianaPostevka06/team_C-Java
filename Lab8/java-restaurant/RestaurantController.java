// Controller (Завдання 3): без Scanner / System.out — тільки координація сценарію
class RestaurantController {
    private final ReservationRepository repository;
    private final NotificationService notificationService;
    private final ReceiptFormatter receiptFormatter;
    private final DiscountPolicy discountPolicy;
    private final ServiceChargePolicy serviceChargePolicy;

    public RestaurantController(ReservationRepository repository,
                                NotificationService notificationService,
                                ReceiptFormatter receiptFormatter,
                                DiscountPolicy discountPolicy,
                                ServiceChargePolicy serviceChargePolicy) {
        this.repository = repository;
        this.notificationService = notificationService;
        this.receiptFormatter = receiptFormatter;
        this.discountPolicy = discountPolicy;
        this.serviceChargePolicy = serviceChargePolicy;
    }

    // Повертає текст чека для відображення
    public String makeReservation(String guestName, String timeSlot) {
        Guest guest = new Guest(3, guestName, "+380631234567");
        TableReservation res = new TableReservation(404, guest, 6, timeSlot);

        MenuItem steak = new MenuItem("Стейк Рібай", 650.0, "Main");
        MenuItem wine = new MenuItem("Вино Червоне", 400.0, "Drink");
        res.addItem(steak, 4);
        res.addItem(wine, 2);

        res.confirm(discountPolicy, serviceChargePolicy);

        repository.save(res);
        notificationService.notifyGuest(guest, res);

        return receiptFormatter.format(res);
    }
}
