import java.util.List;

class MenuItem {
    private final String name;
    private final double price;
    private final String courseType; // "Main", "Drink", "Dessert"

    public MenuItem(String name, double price, String courseType) {
        if (name == null || name.isBlank()) throw new IllegalArgumentException("Назва страви не може бути порожньою");
        if (price <= 0) throw new IllegalArgumentException("Ціна має бути додатною");
        if (!List.of("Main", "Drink", "Dessert").contains(courseType))
            throw new IllegalArgumentException("Невідомий тип страви: " + courseType);
        this.name = name;
        this.price = price;
        this.courseType = courseType;
    }

    public String getName() { return name; }
    public double getPrice() { return price; }
    public String getCourseType() { return courseType; }
}
