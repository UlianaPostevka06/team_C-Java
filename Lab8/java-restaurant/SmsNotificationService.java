class SmsNotificationService implements NotificationService {
    @Override
    public void notifyGuest(Guest guest, TableReservation reservation) {
        System.out.println("[SMS] Деталі резерву надіслано на номер " + guest.getPhone());
    }
}
