package Task3_Builder;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

/**
 * Продукт: персонаж гравця. Незмінний (immutable): усі поля final, сеттерів немає,
 * конструктор доступний лише білдеру. Тому після створення персонаж не змінюється.
 * Примітка: клас у цьому пакеті затіняє java.lang.Character, але тут він не використовується.
 */
public final class Character {

    private final String nickname;
    private final String race;
    private final String characterClass;
    private final String appearance;
    private final List<String> skills;
    private final String biography;
    private final String uiTheme;

    /** Конструктор для пакета: викликає лише CharacterBuilder після перевірки даних. */
    Character(String nickname, String race, String characterClass, String appearance,
              List<String> skills, String biography, String uiTheme) {
        this.nickname = nickname;
        this.race = race;
        this.characterClass = characterClass;
        this.appearance = appearance;
        this.skills = Collections.unmodifiableList(new ArrayList<>(skills));
        this.biography = biography;
        this.uiTheme = uiTheme;
    }

    public String getNickname() {
        return nickname;
    }

    public String getRace() {
        return race;
    }

    public String getCharacterClass() {
        return characterClass;
    }

    public String getAppearance() {
        return appearance;
    }

    public List<String> getSkills() {
        return skills;
    }

    public String getBiography() {
        return biography;
    }

    public String getUiTheme() {
        return uiTheme;
    }

    /** Читабельний опис персонажа з усіма частинами. */
    public String describe() {
        return nickname + " [race=" + race + ", class=" + characterClass
                + ", appearance=" + appearance + ", skills=" + skills
                + ", biography=" + (biography.isEmpty() ? "<none>" : "\"" + biography + "\"")
                + ", uiTheme=" + uiTheme + "]";
    }
}
