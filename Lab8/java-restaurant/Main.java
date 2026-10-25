// Точка входу (композиція залежностей)
public class Main {
    public static void main(String[] args) {
        RestaurantController controller = new RestaurantController(
                new JsonFileReservationRepository(),
                new SmsNotificationService(),
                new ReceiptFormatter(),
                new HappyHoursDiscountPolicy(),
                new LargeGroupServiceChargePolicy()
        );
        new RestaurantConsoleUI(controller).run();
    }
}
