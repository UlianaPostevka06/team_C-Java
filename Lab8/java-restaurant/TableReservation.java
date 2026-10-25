import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

class TableReservation {
    private final int reservationId;
    private final Guest guest;
    private final int guestsCount;
    private final String timeSlot; // "HappyHours", "Standard", "Evening"
    private final List<OrderLine> preOrderedItems = new ArrayList<>();
    private double foodSubtotal;
    private double serviceCharge;
    private double totalBill;
    private String status;

    public TableReservation(int reservationId, Guest guest, int guestsCount, String timeSlot) {
        if (guest == null) throw new IllegalArgumentException("Гість обов'язковий");
        if (guestsCount <= 0) throw new IllegalArgumentException("Кількість гостей має бути додатною");
        if (timeSlot == null || timeSlot.isBlank()) throw new IllegalArgumentException("Часовий слот обов'язковий");
        this.reservationId = reservationId;
        this.guest = guest;
        this.guestsCount = guestsCount;
        this.timeSlot = timeSlot;
        this.status = "Draft";
    }

    // Creator: бронювання створює і контролює свої позиції замовлення
    public void addItem(MenuItem item, int count) {
        if (!"Draft".equals(status)) throw new IllegalStateException("Не можна змінювати підтверджене бронювання");
        preOrderedItems.add(new OrderLine(item, count));
    }

    // Information Expert: бронювання володіє даними для розрахунку.
    // Правила знижки та збору приходять через абстракції (Завдання 4).
    public void confirm(DiscountPolicy discountPolicy, ServiceChargePolicy serviceChargePolicy) {
        if (!"Draft".equals(status)) throw new IllegalStateException("Бронювання вже підтверджено");
        if (preOrderedItems.isEmpty()) throw new IllegalStateException("Попереднє замовлення порожнє");

        double rawSubtotal = 0.0;
        for (OrderLine line : preOrderedItems) {
            rawSubtotal += line.getLineTotal();
        }
        this.foodSubtotal = rawSubtotal * (1.0 - discountPolicy.getDiscountRate(timeSlot));
        this.serviceCharge = foodSubtotal * serviceChargePolicy.getChargeRate(guestsCount);
        this.totalBill = foodSubtotal + serviceCharge;
        this.status = "Reserved";
    }

    public int getReservationId() { return reservationId; }
    public Guest getGuest() { return guest; }
    public int getGuestsCount() { return guestsCount; }
    public String getTimeSlot() { return timeSlot; }
    public List<OrderLine> getPreOrderedItems() { return Collections.unmodifiableList(preOrderedItems); }
    public double getFoodSubtotal() { return foodSubtotal; }
    public double getServiceCharge() { return serviceCharge; }
    public double getTotalBill() { return totalBill; }
    public String getStatus() { return status; }
}
