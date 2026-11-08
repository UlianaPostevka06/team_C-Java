package Task3_Decorator;

import common.Log;

import java.util.ArrayList;
import java.util.List;

/**
 * Decorator: базова обгортка ефекту. Реалізує той самий інтерфейс ICharacter
 * і містить посилання на обгорнутий об'єкт (персонажа або інший ефект).
 * Конкретні ефекти лише задають модифікатори характеристик.
 */
public abstract class EffectDecorator implements ICharacter {

    private ICharacter wrapped;
    private final String effectName;

    protected EffectDecorator(ICharacter wrapped, String effectName) {
        this.wrapped = wrapped;
        this.effectName = effectName;
    }

    // Модифікатори за замовчуванням нейтральні; ефекти перевизначають потрібні
    protected int attackModifier() {
        return 0;
    }

    protected int defenseModifier() {
        return 0;
    }

    protected int speedModifier() {
        return 0;
    }

    @Override
    public String getName() {
        return wrapped.getName();
    }

    @Override
    public int getAttack() {
        return apply("attack", wrapped.getAttack(), attackModifier());
    }

    @Override
    public int getDefense() {
        return apply("defense", wrapped.getDefense(), defenseModifier());
    }

    @Override
    public int getSpeed() {
        return apply("speed", wrapped.getSpeed(), speedModifier());
    }

    @Override
    public int getHealth() {
        return wrapped.getHealth();
    }

    @Override
    public void takeDamage(int damage) {
        wrapped.takeDamage(damage);
    }

    @Override
    public List<String> getEffectNames() {
        List<String> names = new ArrayList<>(wrapped.getEffectNames());
        names.add(effectName);
        return names;
    }

    @Override
    public ICharacter removeEffect(String name) {
        if (effectName.equals(name)) {
            Log.log("Effect:" + effectName, "effect removed, wrapped object returned");
            return wrapped;
        }
        wrapped = wrapped.removeEffect(name);
        return this;
    }

    /** Результат обгортки: значення обгорнутого об'єкта плюс власний модифікатор. */
    private int apply(String stat, int inner, int modifier) {
        int result = inner + modifier;
        if (modifier != 0) {
            Log.log("Effect:" + effectName, stat + ": wrapped gives " + inner
                    + ", wrapper adds " + String.format("%+d", modifier) + " -> " + result);
        }
        return result;
    }
}
