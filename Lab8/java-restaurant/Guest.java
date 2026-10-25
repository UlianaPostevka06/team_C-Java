// Домен (Завдання 2: Expert + Creator)
class Guest {
    private final int id;
    private final String name;
    private final String phone;

    public Guest(int id, String name, String phone) {
        if (name == null || name.isBlank()) throw new IllegalArgumentException("Ім'я гостя не може бути порожнім");
        if (phone == null || phone.isBlank()) throw new IllegalArgumentException("Телефон не може бути порожнім");
        this.id = id;
        this.name = name.trim();
        this.phone = phone;
    }

    public int getId() { return id; }
    public String getName() { return name; }
    public String getPhone() { return phone; }
}
