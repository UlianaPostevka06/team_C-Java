package Task2_Flyweight;

import common.Log;

import java.util.Random;

/** Раннер завдання 2: рослинність на мапі. */
public class FlyweightDemo {

    private static final int PLANT_COUNT = 100_000;

    public static void main(String[] args) {
        run();
    }

    public static void run() {
        Log.section("Task 2 (Flyweight): vegetation on the map");
        PlantTypeFactory factory = new PlantTypeFactory();
        Location location = new Location("Northern Forest", factory);

        populate(location);
        printCounts(location, factory);
        showTwoOaks(location);
    }

    /** Розміщує 100 000 об'єктів рослинності 6 видів. */
    private static void populate(Location location) {
        Log.log("Demo", "placing " + PLANT_COUNT + " plant objects on " + location.getName());
        String[] names = PlantTypeFactory.allTypeNames();
        Random random = new Random(42);
        for (int i = 0; i < PLANT_COUNT; i++) {
            String typeName = names[random.nextInt(names.length)];
            location.plant(typeName,
                    random.nextDouble() * 5000,
                    random.nextDouble() * 5000,
                    0.5 + random.nextDouble(),
                    random.nextInt(360));
        }
    }

    /** Виводить кількість об'єктів рослинності і кількість об'єктів видів. */
    private static void printCounts(Location location, PlantTypeFactory factory) {
        Log.log("Demo", "plant objects on the map: " + location.getObjectCount());
        Log.log("Demo", "plant type objects created: " + factory.getCreatedTypesCount()
                + " (type lookups: " + factory.getLookupsCount() + ")");
    }

    /** Показує два дуби в різних місцях з посиланням на той самий об'єкт виду. */
    private static void showTwoOaks(Location location) {
        PlantObject first = null;
        PlantObject second = null;
        for (PlantObject plant : location.getPlants()) {
            if (plant.getType().getName().equals("Oak")) {
                if (first == null) {
                    first = plant;
                } else {
                    second = plant;
                    break;
                }
            }
        }
        Log.log("Demo", "oak #1: " + first);
        Log.log("Demo", "oak #2: " + second);
        Log.log("Demo", "same type object (oak #1 == oak #2): "
                + (first.getType() == second.getType())
                + ", type identity hash: " + System.identityHashCode(first.getType()));
    }
}
