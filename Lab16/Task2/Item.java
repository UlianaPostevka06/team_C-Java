package Task2_Iterator;

/** Предмет інвентарю. Вид визначає, кому він цікавий: торговцю чи системі квестів. */
public class Item {

    /** Вид предмета. */
    public enum Kind {
        COMMON,
        FOR_SALE,
        QUEST
    }

    private final String name;
    private final Kind kind;

    public Item(String name, Kind kind) {
        this.name = name;
        this.kind = kind;
    }

    public String getName() {
        return name;
    }

    public Kind getKind() {
        return kind;
    }

    @Override
    public String toString() {
        return name + " [" + kind + "]";
    }
}
