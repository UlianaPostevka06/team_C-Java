package Task3_Decorator;

import java.util.List;

/**
 * Component: загальний інтерфейс персонажа.
 * Бойова система працює лише з ним і не знає, чи на персонажі є ефекти.
 */
public interface ICharacter {

    String getName();

    int getAttack();

    int getDefense();

    int getSpeed();

    int getHealth();

    void takeDamage(int damage);

    /** Назви активних ефектів у порядку накладання (від першого до останнього). */
    List<String> getEffectNames();

    /** Знімає один ефект за назвою і повертає новий верхній об'єкт ланцюжка. */
    ICharacter removeEffect(String effectName);

    /** Опис персонажа. Для обгортки викликає її геттери, тому враховує всі ефекти. */
    default String describe() {
        return getName() + " [attack=" + getAttack() + ", defense=" + getDefense()
                + ", speed=" + getSpeed() + ", hp=" + getHealth() + ", effects=" + getEffectNames() + "]";
    }
}
