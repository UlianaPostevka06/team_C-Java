package task1_prototype;

/** Уміння монстра (змінний об'єкт, тому копіюється окремо для кожного монстра). */
public class Ability {

    private final String name;
    private int power;

    public Ability(String name, int power) {
        this.name = name;
        this.power = power;
    }

    /** Глибока копія уміння. */
    public Ability copy() {
        return new Ability(name, power);
    }

    public String getName() {
        return name;
    }

    public int getPower() {
        return power;
    }

    public void setPower(int power) {
        this.power = power;
    }

    @Override
    public String toString() {
        return name + "(" + power + ")";
    }
}
