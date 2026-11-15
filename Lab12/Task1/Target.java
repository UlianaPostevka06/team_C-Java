package Task1_Bridge;

import java.util.ArrayList;
import java.util.List;

/** Ціль умінь: персонаж або монстр зі здоров'ям і списком накладених ефектів. */
public class Target {

    private final String name;
    private int health;
    private final List<String> effects = new ArrayList<>();

    public Target(String name, int health) {
        this.name = name;
        this.health = health;
    }

    public String getName() {
        return name;
    }

    public int getHealth() {
        return health;
    }

    public List<String> getEffects() {
        return effects;
    }

    /** Завдає шкоди, здоров'я не опускається нижче нуля. */
    public void takeDamage(int damage) {
        health = Math.max(0, health - damage);
    }

    /** Додає ефект, якщо такого ще немає. */
    public void addEffect(String effect) {
        if (!effects.contains(effect)) {
            effects.add(effect);
        }
    }
}
