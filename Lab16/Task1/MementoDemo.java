package Task1_Memento;

import common.Log;

/** Раннер завдання 1: збереження прогресу (Memento). Сценарії йдуть на одному персонажі. */
public class MementoDemo {

    public static void main(String[] args) {
        run();
    }

    public static void run() {
        Hero aldric = new Hero("Aldric", 100, 10, 5);
        SaveManager saves = new SaveManager();

        scenario1SaveBeforeBattle(aldric, saves);
        scenario2BattleAndSaveSlot2(aldric, saves);
        scenario3RestoreSlots(aldric, saves);
    }

    /** Сценарій 1: підготувати персонажа і зберегти його в слот 1 перед боєм. */
    static void scenario1SaveBeforeBattle(Hero hero, SaveManager saves) {
        Log.section("Task 1 (Memento), scenario 1: save before the battle");
        hero.addItem("Iron Sword");
        hero.addItem("Healing Potion");
        hero.addItem("Silver Key");
        hero.acceptQuest("Hunt wolves");
        saves.save(1, hero);
    }

    /** Сценарій 2: бій, герой втрачає здоров'я й предмети, зберегти в слот 2. */
    static void scenario2BattleAndSaveSlot2(Hero hero, SaveManager saves) {
        Log.section("Task 1 (Memento), scenario 2: battle and save to slot 2");
        hero.moveTo(12, 7);
        hero.takeDamage(65);
        hero.removeItem("Healing Potion");
        hero.removeItem("Silver Key");
        saves.save(2, hero);
    }

    /** Сценарій 3: відновити слот 1 і вивести стан, потім відновити слот 2. */
    static void scenario3RestoreSlots(Hero hero, SaveManager saves) {
        Log.section("Task 1 (Memento), scenario 3: restore slot 1, then slot 2");
        saves.load(1, hero);
        saves.load(2, hero);
        saves.load(3, hero);
    }
}
