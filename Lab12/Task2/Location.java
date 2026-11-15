package Task2_Flyweight;

import java.util.ArrayList;
import java.util.List;

/** Client: локація світу, на якій розміщено об'єкти рослинності. */
public class Location {

    private final String name;
    private final PlantTypeFactory factory;
    private final List<PlantObject> plants = new ArrayList<>();

    public Location(String name, PlantTypeFactory factory) {
        this.name = name;
        this.factory = factory;
    }

    public String getName() {
        return name;
    }

    /** Розміщує рослину: вид береться з фабрики, власні дані зберігаються в об'єкті. */
    public PlantObject plant(String typeName, double x, double y, double scale, double rotation) {
        PlantObject plant = new PlantObject(factory.getType(typeName), x, y, scale, rotation);
        plants.add(plant);
        return plant;
    }

    /** Кількість об'єктів рослинності на локації. */
    public int getObjectCount() {
        return plants.size();
    }

    /** Усі об'єкти рослинності (для пошуку в демонстрації). */
    public List<PlantObject> getPlants() {
        return plants;
    }
}
