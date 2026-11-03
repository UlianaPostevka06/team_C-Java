package Task3_Builder;

import common.Log;

import java.util.ArrayList;
import java.util.List;

/** Конкретний білдер: покроково збирає персонажа і перевіряє його на завершальному кроці. */
public class CharacterBuilder implements ICharacterBuilder {

    private static final String DEFAULT_APPEARANCE = "Default";
    private static final String DEFAULT_THEME = "Light";

    private String nickname;
    private String race;
    private String characterClass;
    private String appearance = DEFAULT_APPEARANCE;
    private final List<String> skills = new ArrayList<>();
    private String biography = "";
    private String uiTheme = DEFAULT_THEME;

    @Override
    public ICharacterBuilder setNickname(String nickname) {
        this.nickname = nickname;
        Log.log("CharacterBuilder", "Nickname set: '" + nickname + "'");
        return this;
    }

    @Override
    public ICharacterBuilder setRace(String race) {
        this.race = race;
        Log.log("CharacterBuilder", "Race set: " + race);
        return this;
    }

    @Override
    public ICharacterBuilder setCharacterClass(String characterClass) {
        this.characterClass = characterClass;
        Log.log("CharacterBuilder", "Class set: " + characterClass);
        return this;
    }

    @Override
    public ICharacterBuilder setAppearance(String appearance) {
        this.appearance = appearance;
        Log.log("CharacterBuilder", "Appearance set: " + appearance);
        return this;
    }

    @Override
    public ICharacterBuilder addSkill(String skill) {
        skills.add(skill);
        Log.log("CharacterBuilder", "Skill added: " + skill);
        return this;
    }

    @Override
    public ICharacterBuilder setBiography(String biography) {
        this.biography = biography;
        Log.log("CharacterBuilder", "Biography set");
        return this;
    }

    @Override
    public ICharacterBuilder setUiTheme(String uiTheme) {
        this.uiTheme = uiTheme;
        Log.log("CharacterBuilder", "UI theme set: " + uiTheme);
        return this;
    }

    @Override
    public Character build() {
        // Перевірка обов'язкових частин на завершальному кроці
        if (isBlank(nickname)) {
            fail("Cannot create character: nickname not set");
        }
        if (isBlank(race)) {
            fail("Cannot create character: race not selected");
        }
        if (isBlank(characterClass)) {
            fail("Cannot create character: class not selected");
        }
        Character character = new Character(nickname.trim(), race, characterClass,
                appearance, skills, biography, uiTheme);
        Log.log("CharacterBuilder", "Character '" + character.getNickname() + "' created");
        reset();
        return character;
    }

    @Override
    public void reset() {
        nickname = null;
        race = null;
        characterClass = null;
        appearance = DEFAULT_APPEARANCE;
        skills.clear();
        biography = "";
        uiTheme = DEFAULT_THEME;
    }

    /** Пише помилку в журнал, очищає білдер і перериває створення. */
    private void fail(String message) {
        Log.log("CharacterBuilder", message);
        reset();
        throw new IllegalStateException(message);
    }

    private static boolean isBlank(String value) {
        return value == null || value.trim().isEmpty();
    }
}
