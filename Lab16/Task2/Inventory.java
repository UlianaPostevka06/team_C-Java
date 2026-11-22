package Task2_Iterator;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

/**
 * Aggregate: інвентар персонажа. Складається з основної сумки, додаткових сумок і банківського сховища.
 * Модулі отримують від нього ітератор і не знають, як усе влаштовано всередині.
 */
public class Inventory implements Iterable<Item> {

    private final Container mainBag;
    private final List<Container> extraBags = new ArrayList<>();
    private final Container bank;

    public Inventory(Container mainBag, Container bank) {
        this.mainBag = mainBag;
        this.bank = bank;
    }

    public void addExtraBag(Container bag) {
        extraBags.add(bag);
    }

    /** Корені обходу: основна сумка, додаткові сумки, банк. */
    List<Container> roots() {
        List<Container> roots = new ArrayList<>();
        roots.add(mainBag);
        roots.addAll(extraBags);
        roots.add(bank);
        return roots;
    }

    /** Усі предмети, зокрема у вкладених контейнерах (інтерфейс). */
    @Override
    public Iterator<Item> iterator() {
        return new AllItemsIterator(this);
    }

    /** Лише предмети для продажу (торговець). */
    public Iterator<Item> forSaleItems() {
        return new KindIterator(this, Item.Kind.FOR_SALE);
    }

    /** Лише квестові предмети (система квестів). */
    public Iterator<Item> questItems() {
        return new KindIterator(this, Item.Kind.QUEST);
    }
}
