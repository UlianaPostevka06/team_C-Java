package task1_prototype;

import common.Log;

import java.util.Arrays;
import java.util.List;

/** Демонстрація завдання 1: спавн монстрів за шаблоном Prototype. */
public class PrototypeDemo {

    public static void main(String[] args) {
        run();
    }

    public static void run() {
        Log.section("Task 1: Prototype - monster spawning");

        // 1-2. Створюємо еталонного вовка (дороге завантаження)
        Monster referenceWolf = new Monster(
                "Wolf", 5, 120, 15,
                new Equipment("Fangs", "Thick fur"),
                Arrays.asList(new Ability("Bite", 10), new Ability("Howl", 5)),
                Arrays.asList(new Loot("Wolf pelt", 80), new Loot("Wolf fang", 30)));

        MonsterFactory factory = new MonsterFactory();
        factory.registerPrototype("Wolf", referenceWolf);

        String referenceBefore = referenceWolf.describe();

        // 3. Створюємо п'ять вовків копіюванням (без повторного завантаження)
        List<Monster> pack = factory.spawnPack("Wolf", 5, "Black Forest");

        // 4-5. Одного вовка робимо ватажком: зброя, рівень, нове вміння
        Monster leader = pack.get(0);
        leader.setRole("pack leader");
        leader.getEquipment().setWeapon("Iron claws");
        leader.setLevel(10);
        leader.addAbility(new Ability("Pack Command", 25));
        Log.log("Demo", leader.getId() + " promoted to pack leader (weapon, level and ability changed)");

        Monster regular = pack.get(1);

        // 6. Виводимо еталон, ватажка і звичайного вовка
        Log.log("Demo", "REFERENCE : " + referenceWolf.describe());
        Log.log("Demo", "LEADER    : " + leader.describe());
        Log.log("Demo", "REGULAR   : " + regular.describe());

        // Перевірка очікуваного результату
        boolean referenceUnchanged = referenceBefore.equals(referenceWolf.describe());
        boolean regularUnchanged = regular.describe().replace(regular.getId(), "X")
                .equals(referenceBefore.replace(referenceWolf.getId(), "X"));
        Log.log("Demo", "Reference unchanged: " + referenceUnchanged);
        Log.log("Demo", "Regular wolf unchanged: " + regularUnchanged);
    }
}
