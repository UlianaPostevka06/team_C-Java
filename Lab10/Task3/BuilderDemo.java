package Task3_Builder;

import common.Log;

import java.util.ArrayList;
import java.util.List;

/** Демонстрація завдання 3: створення персонажа гравця (Builder). */
public class BuilderDemo {

    // Імітація бази даних: сюди потрапляють лише повністю створені персонажі
    private static final List<Character> database = new ArrayList<>();

    public static void main(String[] args) {
        run();
    }

    public static void run() {
        Log.section("Task 3: Builder - player character creation");
        database.clear();

        ICharacterBuilder builder = new CharacterBuilder();

        // Сценарій 1: персонаж з усіма параметрами
        Log.log("Demo", "Scenario 1: character with all parameters");
        Character full = builder
                .setNickname("Stormrider")
                .setRace("Elf")
                .setCharacterClass("Ranger")
                .setAppearance("Tall, silver hair, green cloak")
                .addSkill("Archery")
                .addSkill("Tracking")
                .addSkill("Stealth")
                .setBiography("Born in the northern forests of Siver.")
                .setUiTheme("Dark")
                .build();
        save(full);

        // Сценарій 2: типові персонажі швидкого старту через директора
        Log.log("Demo", "Scenario 2: quick start characters via director");
        CharacterDirector director = new CharacterDirector(builder);
        save(director.createMage("Arcanus"));
        save(director.createWarrior("Ironwall"));

        // Сценарій 3: спроба створити персонажа без класу
        Log.log("Demo", "Scenario 3: finish creation without a class");
        tryBuild(builder.setNickname("Nameless").setRace("Human"));

        // Сценарій 4: спроба створити персонажа з порожнім нікнеймом
        Log.log("Demo", "Scenario 4: finish creation with an empty nickname");
        tryBuild(builder.setNickname("  ").setRace("Orc").setCharacterClass("Warrior"));

        Log.log("Demo", "Characters saved to database: " + database.size() + " (incomplete ones rejected)");
    }

    private static void tryBuild(ICharacterBuilder b) {
        try {
            save(b.build());
        } catch (IllegalStateException e) {
            Log.log("Demo", "Creation rejected, nothing saved");
        }
    }

    private static void save(Character character) {
        database.add(character);
        Log.log("Demo", "Saved: " + character.describe());
    }
}
