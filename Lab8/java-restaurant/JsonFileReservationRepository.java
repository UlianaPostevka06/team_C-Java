class JsonFileReservationRepository implements ReservationRepository {
    @Override
    public void save(TableReservation reservation) {
        // Імітація збереження (як в оригіналі)
        System.out.println("[БД Ресторану] Бронювання #" + reservation.getReservationId()
                + " збережено в reservations.json.");
    }
}
