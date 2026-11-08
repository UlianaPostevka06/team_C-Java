package Task3_Decorator;

import common.Log;

import java.util.ArrayList;
import java.util.List;

/** ConcreteComponent: персонаж без ефектів. Повертає свої базові характеристики. */
public class Character implements ICharacter {

    private final String name;
    private final int attack;
    private final int defense;
    private final int speed;
    private int health;

    public Character(String name, int attack, int defense, int speed, int health) {
        this.name = name;
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
        this.health = health;
    }

    @Override
    public String getName() {
        return name;
    }

    @Override
    public int getAttack() {
        Log.log("Character:" + name, "base attack = " + attack);
        return attack;
    }

    @Override
    public int getDefense() {
        Log.log("Character:" + name, "base defense = " + defense);
        return defense;
    }

    @Override
    public int getSpeed() {
        Log.log("Character:" + name, "base speed = " + speed);
        return speed;
    }

    @Override
    public int getHealth() {
        return health;
    }

    @Override
    public void takeDamage(int damage) {
        health = Math.max(0, health - damage);
    }

    @Override
    public List<String> getEffectNames() {
        return new ArrayList<>();
    }

    @Override
    public ICharacter removeEffect(String effectName) {
        // Ефектів немає: нічого знімати
        return this;
    }
}
