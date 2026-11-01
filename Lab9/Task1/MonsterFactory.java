package task1_prototype;

import common.Log;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/** Реєстр еталонів (прототипів) і фабрика, що створює монстрів копіюванням. */
public class MonsterFactory {

    // Еталони для кожного виду монстрів
    private final Map<String, Monster> prototypes = new HashMap<>();
    private final Map<String, Integer> counters = new HashMap<>();

    public void registerPrototype(String species, Monster prototype) {
        prototypes.put(species, prototype);
        Log.log("MonsterFactory", "Prototype registered for species '" + species + "'");
    }

    /** Створює одного монстра копіюванням еталона. */
    public Monster spawn(String species) {
        Monster prototype = prototypes.get(species);
        if (prototype == null) {
            throw new IllegalArgumentException("No prototype for species: " + species);
        }
        Monster clone = prototype.copy();
        int n = counters.merge(species, 1, Integer::sum);
        clone.setId(species + "-" + n);
        Log.log("MonsterFactory", "Cloned " + clone.getId() + " from prototype (no reloading)");
        return clone;
    }

    /** Створює кількох монстрів і "розміщує" їх на локації. */
    public List<Monster> spawnPack(String species, int count, String location) {
        List<Monster> pack = new ArrayList<>();
        for (int i = 0; i < count; i++) {
            pack.add(spawn(species));
        }
        Log.log("MonsterFactory", count + " x " + species + " placed at location '" + location + "'");
        return pack;
    }

    public Monster getPrototype(String species) {
        return prototypes.get(species);
    }
}
