package Task3_Decorator;

import common.Log;

/** Демонстрація завдання 3: ефекти на персонажі (Decorator). */
public class DecoratorDemo {

    public static void main(String[] args) {
        run();
    }

    public static void run() {
        Log.section("Task 3: Decorator - effects on a character");

        ICharacter hero = new Character("Aldric", 20, 10, 10, 100);
        ICharacter dummy = new Character("Training Dummy", 5, 6, 0, 100);
        CombatSystem combat = new CombatSystem();

        // Сценарій 1: характеристики без ефектів
        Log.log("Demo", "Scenario 1: stats without effects");
        Log.log("Demo", "Stats: " + hero.describe());

        // Сценарій 2: зілля сили і благословення
        Log.log("Demo", "Scenario 2: Strength Potion + Blessing");
        hero = new StrengthPotion(hero);
        hero = new Blessing(hero);
        Log.log("Demo", "Stats: " + hero.describe());

        // Сценарій 3: додатково отрута і прокляття повільності, потім атака
        Log.log("Demo", "Scenario 3: + Poison + Slowness Curse, then one attack");
        hero = new Poison(hero);
        hero = new SlownessCurse(hero);
        Log.log("Demo", "Stats: " + hero.describe());
        combat.attack(hero, dummy);

        // Сценарій 4: однакові ефекти діють одночасно, ефект знімається під час гри
        Log.log("Demo", "Scenario 4: duplicate effect, new effect and removal");
        hero = new StrengthPotion(hero);
        hero = new Shield(hero);
        Log.log("Demo", "Stats with a second Strength Potion and Shield: " + hero.describe());
        hero = hero.removeEffect("Poison");
        Log.log("Demo", "Stats after Poison is removed: " + hero.describe());
    }
}
