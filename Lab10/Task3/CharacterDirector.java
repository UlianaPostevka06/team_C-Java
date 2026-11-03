package Task3_Builder;

import common.Log;

/**
 * Директор: описує в одному місці послідовність кроків для типових персонажів швидкого старту.
 * Працює лише з інтерфейсом ICharacterBuilder.
 */
public class CharacterDirector {

    private final ICharacterBuilder builder;

    public CharacterDirector(ICharacterBuilder builder) {
        this.builder = builder;
    }

    /** Типовий персонаж "Воїн". */
    public Character createWarrior(String nickname) {
        return quickStart(nickname, "Human", "Warrior",
                "Broad-shouldered, scarred face",
                new String[]{"Sword Mastery", "Shield Bash"});
    }

    /** Типовий персонаж "Маг". */
    public Character createMage(String nickname) {
        return quickStart(nickname, "Elf", "Mage",
                "Slim, silver hair, blue robe",
                new String[]{"Fireball", "Frost Nova", "Arcane Shield"});
    }

    /** Єдина послідовність кроків для всіх типових персонажів. */
    private Character quickStart(String nickname, String race, String characterClass,
                                 String appearance, String[] skills) {
        Log.log("CharacterDirector", "Quick start: " + characterClass);
        builder.reset();
        builder.setNickname(nickname)
                .setRace(race)
                .setCharacterClass(characterClass)
                .setAppearance(appearance);
        for (String skill : skills) {
            builder.addSkill(skill);
        }
        return builder.build();
    }
}
