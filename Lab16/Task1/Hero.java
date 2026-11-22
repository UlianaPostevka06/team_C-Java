package Task1_Memento;

import common.Log;

import java.util.ArrayList;
import java.util.List;

/**
 * Originator: персонаж. Усі поля приватні, а знімок створює і читає лише він сам.
 * Знімок зберігає копії списків, тому подальші зміни персонажа його не псують.
 */
public class Hero {

    private final String name;
    private final int maxHealth;
    private int health;
    private int x;
    private int y;
    private final List<String> inventory = new ArrayList<>();
    private final List<String> quests = new ArrayList<>();

    public Hero(String name, int maxHealth, int x, int y) {
        this.name = name;
        this.maxHealth = maxHealth;
        this.health = maxHealth;
        this.x = x;
        this.y = y;
    }

    private String source() {
        return "Hero:" + name;
    }

    public void addItem(String item) {
        inventory.add(item);
        Log.log(source(), "gets item " + item);
    }

    public void removeItem(String item) {
        inventory.remove(item);
        Log.log(source(), "loses item " + item);
    }

    public void acceptQuest(String quest) {
        quests.add(quest);
        Log.log(source(), "accepts quest \"" + quest + "\"");
    }

    public void moveTo(int newX, int newY) {
        x = newX;
        y = newY;
        Log.log(source(), "moves to (" + x + "," + y + ")");
    }

    public void takeDamage(int damage) {
        health = Math.max(0, health - damage);
        Log.log(source(), "takes " + damage + " damage");
    }

    /** Поточний стан у вигляді рядка для журналу. */
    public String describe() {
        return "HP " + health + "/" + maxHealth + ", position (" + x + "," + y + "), items "
                + inventory + ", quests " + quests;
    }

    /** Створює знімок поточного стану. */
    public Memento save() {
        Log.log(source(), "creates snapshot of state: " + describe());
        return new Snapshot(health, x, y, inventory, quests);
    }

    /** Повертає стан зі знімка. */
    public void restore(Memento memento) {
        Snapshot s = (Snapshot) memento; // чужих знімків не буває, знімок створює лише Hero
        Log.log(source(), "state before restore: " + describe());
        health = s.health;
        x = s.x;
        y = s.y;
        inventory.clear();
        inventory.addAll(s.inventory);
        quests.clear();
        quests.addAll(s.quests);
        Log.log(source(), "state after restore:  " + describe());
    }

    /** Знімок видимий лише всередині Hero, зовні про нього відомо тільки як про Memento. */
    private static class Snapshot implements Memento {
        private final int health;
        private final int x;
        private final int y;
        private final List<String> inventory;
        private final List<String> quests;

        Snapshot(int health, int x, int y, List<String> inventory, List<String> quests) {
            this.health = health;
            this.x = x;
            this.y = y;
            this.inventory = List.copyOf(inventory);
            this.quests = List.copyOf(quests);
        }
    }
}
