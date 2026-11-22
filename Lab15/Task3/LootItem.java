package Task3_Strategy;

/** Предмет здобичі. Якщо preferredClass задано, він найкраще підходить цьому класу. */
public class LootItem {

    private final String name;
    private final String preferredClass; // null, якщо предмет підходить будь-кому

    public LootItem(String name, String preferredClass) {
        this.name = name;
        this.preferredClass = preferredClass;
    }

    public String getName() {
        return name;
    }

    public String getPreferredClass() {
        return preferredClass;
    }
}
