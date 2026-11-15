package Task2_Flyweight;

import common.Log;

import java.util.HashMap;
import java.util.Map;

/**
 * FlyweightFactory: видає вид за назвою.
 * Якщо вид уже створювався, повертає той самий об'єкт, а новий не створює.
 */
public class PlantTypeFactory {

    // Довідник видів: назва, модель, текстура, анімація вітру
    private static final String[][] CATALOG = {
            {"Oak", "oak.obj", "oak_bark.png", "sway_slow"},
            {"Pine", "pine.obj", "pine_needles.png", "sway_medium"},
            {"Birch", "birch.obj", "birch_bark.png", "sway_fast"},
            {"Hazel Bush", "hazel.obj", "hazel_leaves.png", "rustle_light"},
            {"Wild Rose Bush", "rose.obj", "rose_flowers.png", "rustle_light"},
            {"Boulder", "boulder.obj", "granite.png", "none"}
    };

    // Кеш уже створених видів
    private final Map<String, PlantType> cache = new HashMap<>();
    private int lookups = 0;

    /** Повертає спільний об'єкт виду, створюючи його лише при першому запиті. */
    public PlantType getType(String name) {
        lookups++;
        PlantType type = cache.get(name);
        if (type == null) {
            type = createFromCatalog(name);
            cache.put(name, type);
            Log.log("PlantTypeFactory", "created new plant type: " + type);
        }
        return type;
    }

    private PlantType createFromCatalog(String name) {
        for (String[] row : CATALOG) {
            if (row[0].equals(name)) {
                return new PlantType(row[0], row[1], row[2], row[3]);
            }
        }
        throw new IllegalArgumentException("Unknown plant type: " + name);
    }

    /** Кількість фактично створених об'єктів видів. */
    public int getCreatedTypesCount() {
        return cache.size();
    }

    /** Скільки разів вид запитували. */
    public int getLookupsCount() {
        return lookups;
    }

    /** Назви всіх видів із довідника. */
    public static String[] allTypeNames() {
        String[] names = new String[CATALOG.length];
        for (int i = 0; i < CATALOG.length; i++) {
            names[i] = CATALOG[i][0];
        }
        return names;
    }
}
