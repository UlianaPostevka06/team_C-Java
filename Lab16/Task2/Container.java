package Task2_Iterator;

import java.util.ArrayList;
import java.util.List;

/**
 * Сумка, скриня або сховище. Містить предмети і вкладені контейнери (будь-якої глибини).
 * Зсередини його читає лише ітератор, тому геттери видимі тільки в пакеті.
 */
public class Container {

    private final String name;
    private final List<Item> items = new ArrayList<>();
    private final List<Container> children = new ArrayList<>();

    public Container(String name) {
        this.name = name;
    }

    public Container add(Item item) {
        items.add(item);
        return this;
    }

    public Container add(Container child) {
        children.add(child);
        return this;
    }

    String getName() {
        return name;
    }

    List<Item> getItems() {
        return items;
    }

    List<Container> getChildren() {
        return children;
    }
}
