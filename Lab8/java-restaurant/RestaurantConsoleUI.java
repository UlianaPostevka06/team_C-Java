import java.util.Scanner;

// UI (Завдання 3): тільки введення/виведення, жодної бізнес-логіки
class RestaurantConsoleUI {
    private final RestaurantController controller;

    public RestaurantConsoleUI(RestaurantController controller) {
        this.controller = controller;
    }

    public void run() {
        Scanner scanner = new Scanner(System.in);
        System.out.println("=== Бронювання столика в ресторані ===");
        System.out.print("Введіть ім'я гостя: ");
        String name = scanner.nextLine();
        System.out.print("Введіть час відвідування (HappyHours/Standard/Evening): ");
        String timeSlot = scanner.nextLine();

        try {
            String receipt = controller.makeReservation(name, timeSlot);
            System.out.println(receipt);
        } catch (IllegalArgumentException | IllegalStateException e) {
            System.out.println("Помилка: " + e.getMessage());
        }
    }
}
