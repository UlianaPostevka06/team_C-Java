package task1_prototype;

/** Екіпірування монстра (вкладений об'єкт, який потребує глибокого копіювання). */
public class Equipment {

    private String weapon;
    private String armor;

    public Equipment(String weapon, String armor) {
        this.weapon = weapon;
        this.armor = armor;
    }

    /** Глибока копія екіпірування. */
    public Equipment copy() {
        return new Equipment(weapon, armor);
    }

    public String getWeapon() {
        return weapon;
    }

    public void setWeapon(String weapon) {
        this.weapon = weapon;
    }

    public String getArmor() {
        return armor;
    }

    @Override
    public String toString() {
        return "weapon=" + weapon + ", armor=" + armor;
    }
}
