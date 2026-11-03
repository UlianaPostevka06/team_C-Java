package Task3_Builder;

/**
 * Абстрактний білдер: кожен крок задає одну частину персонажа окремою зрозумілою дією.
 * Директор працює лише з цим інтерфейсом.
 */
public interface ICharacterBuilder {

    // Обов'язкові частини
    ICharacterBuilder setNickname(String nickname);

    ICharacterBuilder setRace(String race);

    ICharacterBuilder setCharacterClass(String characterClass);

    // Необов'язкові частини (можна пропускати)
    ICharacterBuilder setAppearance(String appearance);

    ICharacterBuilder addSkill(String skill);

    ICharacterBuilder setBiography(String biography);

    ICharacterBuilder setUiTheme(String uiTheme);

    /** Завершальний крок: перевіряє обов'язкові частини і створює персонажа. */
    Character build();

    /** Скидає білдер до початкового стану. */
    void reset();
}
