package Task2_Flyweight;

/**
 * Об'єкт рослинності на мапі: зберігає лише власні (зовнішні) дані
 * і посилання на спільний вид.
 */
public class PlantObject {

    private final PlantType type;
    private final double x;
    private final double y;
    private final double scale;
    private final double rotation;

    public PlantObject(PlantType type, double x, double y, double scale, double rotation) {
        this.type = type;
        this.x = x;
        this.y = y;
        this.scale = scale;
        this.rotation = rotation;
    }

    public PlantType getType() {
        return type;
    }

    public double getX() {
        return x;
    }

    public double getY() {
        return y;
    }

    @Override
    public String toString() {
        return String.format("%s at (%.1f, %.1f), scale %.2f, rotation %.0f deg",
                type.getName(), x, y, scale, rotation);
    }
}
