package task1_prototype;

import common.Log;

import java.util.ArrayList;
import java.util.List;

/** Монстр. Конкретний прототип: створення з нуля дороге, копіювання дешеве. */
public class Monster implements MonsterPrototype {

    private String id;
    private final String species;
    private String role = "regular";
    private int level;
    private int health;
    private int attack;
    private Equipment equipment;
    private final List<Ability> abilities = new ArrayList<>();
    private final List<Loot> lootTable = new ArrayList<>();

    /** Дорогий конструктор: імітує завантаження моделі, анімацій і характеристик. */
    public Monster(String species, int level, int health, int attack,
                   Equipment equipment, List<Ability> abilities, List<Loot> loot) {
        Log.expensive("Monster", "Loading model, animations and stats for '" + species + "'", 400);
        this.id = species + "-REFERENCE";
        this.species = species;
        this.level = level;
        this.health = health;
        this.attack = attack;
        this.equipment = equipment;
        this.abilities.addAll(abilities);
        this.lootTable.addAll(loot);
    }

    /** Приватний конструктор копіювання: без затримки, з глибоким копіюванням вкладених даних. */
    private Monster(Monster other) {
        this.id = other.id;
        this.species = other.species;
        this.role = other.role;
        this.level = other.level;
        this.health = other.health;
        this.attack = other.attack;
        this.equipment = other.equipment.copy();
        for (Ability a : other.abilities) {
            this.abilities.add(a.copy());
        }
        for (Loot l : other.lootTable) {
            this.lootTable.add(l.copy());
        }
    }

    @Override
    public Monster copy() {
        return new Monster(this);
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getId() {
        return id;
    }

    public void setRole(String role) {
        this.role = role;
    }

    public void setLevel(int level) {
        this.level = level;
    }

    public Equipment getEquipment() {
        return equipment;
    }

    public void addAbility(Ability ability) {
        abilities.add(ability);
    }

    /** Текстовий опис стану монстра для журналу. */
    public String describe() {
        return id + " [role=" + role + ", level=" + level + ", hp=" + health + ", atk=" + attack
                + ", " + equipment + ", abilities=" + abilities + ", loot=" + lootTable + "]";
    }
}
