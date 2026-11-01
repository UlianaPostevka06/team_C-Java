package task1_prototype;

/** Запис таблиці луту: предмет і шанс випадіння. */
public class Loot {

    private final String item;
    private int dropChancePercent;

    public Loot(String item, int dropChancePercent) {
        this.item = item;
        this.dropChancePercent = dropChancePercent;
    }

    /** Глибока копія запису луту. */
    public Loot copy() {
        return new Loot(item, dropChancePercent);
    }

    public String getItem() {
        return item;
    }

    public int getDropChancePercent() {
        return dropChancePercent;
    }

    public void setDropChancePercent(int dropChancePercent) {
        this.dropChancePercent = dropChancePercent;
    }

    @Override
    public String toString() {
        return item + "(" + dropChancePercent + "%)";
    }
}
