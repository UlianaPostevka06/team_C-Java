// Pure Fabrication: форматування чека
class ReceiptFormatter {
    public String format(TableReservation res) {
        return "Чек резерву #" + res.getReservationId() +
                "\nГість: " + res.getGuest().getName() + " (" + res.getGuestsCount() + " осіб)" +
                "\nСтрави: " + res.getFoodSubtotal() + " грн" +
                "\nОбслуговування: " + res.getServiceCharge() + " грн" +
                "\nВсього до сплати: " + res.getTotalBill() + " грн";
    }
}
