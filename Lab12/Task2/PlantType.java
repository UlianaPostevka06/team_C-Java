package Task2_Flyweight;

/**
 * Flyweight: вид рослинності зі спільними (внутрішніми) даними.
 * Модель, текстура й анімація вітру зберігаються один раз на вид.
 * Об'єкт незмінний, тому його безпечно ділити між тисячами об'єктів.
 */
public final class PlantType {

    private final String name;
    private final String model3d;
    private final String texture;
    private final String windAnimation;

    public PlantType(String name, String model3d, String texture, String windAnimation) {
        this.name = name;
        this.model3d = model3d;
        this.texture = texture;
        this.windAnimation = windAnimation;
    }

    public String getName() {
        return name;
    }

    public String getModel3d() {
        return model3d;
    }

    public String getTexture() {
        return texture;
    }

    public String getWindAnimation() {
        return windAnimation;
    }

    @Override
    public String toString() {
        return name + " [model=" + model3d + ", texture=" + texture
                + ", wind=" + windAnimation + "]";
    }
}
