package Task2_Iterator;

import java.util.ArrayDeque;
import java.util.Deque;
import java.util.Iterator;
import java.util.NoSuchElementException;

/*
 * Ітератори інвентарю в одному файлі.
 * Це класи без public: видимі в пакеті Task2_Iterator, а зовні доступні через Inventory.
 * Кожен ітератор має власний стан, тому кілька обходів однієї колекції не заважають одному.
 */

/** ConcreteIterator: усі предмети, обхід углиб без рекурсії, на власному стеку. */
class AllItemsIterator implements Iterator<Item> {

    // Стек контейнерів, які ще не обійдено повністю, і позиція в предметах кожного
    private final Deque<Container> pending = new ArrayDeque<>();
    private Container current;
    private int position;

    AllItemsIterator(Inventory inventory) {
        // Кладемо корені у зворотному порядку, щоб першою була основна сумка
        for (int i = inventory.roots().size() - 1; i >= 0; i--) {
            pending.push(inventory.roots().get(i));
        }
    }

    /** Знаходить контейнер, у якому ще лишились необійдені предмети. */
    private void advance() {
        while ((current == null || position >= current.getItems().size()) && !pending.isEmpty()) {
            current = pending.pop();
            position = 0;
            // Вкладені контейнери стануть у чергу після предметів поточного
            for (int i = current.getChildren().size() - 1; i >= 0; i--) {
                pending.push(current.getChildren().get(i));
            }
        }
    }

    @Override
    public boolean hasNext() {
        advance();
        return current != null && position < current.getItems().size();
    }

    @Override
    public Item next() {
        if (!hasNext()) {
            throw new NoSuchElementException();
        }
        return current.getItems().get(position++);
    }
}

/** ConcreteIterator: предмети одного виду, обгортка над повним обходом. */
class KindIterator implements Iterator<Item> {

    private final Iterator<Item> source;
    private final Item.Kind kind;
    private Item ready; // наступний підходящий предмет, знайдений наперед

    KindIterator(Inventory inventory, Item.Kind kind) {
        this.source = new AllItemsIterator(inventory);
        this.kind = kind;
    }

    @Override
    public boolean hasNext() {
        while (ready == null && source.hasNext()) {
            Item candidate = source.next();
            if (candidate.getKind() == kind) {
                ready = candidate;
            }
        }
        return ready != null;
    }

    @Override
    public Item next() {
        if (!hasNext()) {
            throw new NoSuchElementException();
        }
        Item result = ready;
        ready = null;
        return result;
    }
}
