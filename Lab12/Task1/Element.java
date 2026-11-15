package Task1_Bridge;

/**
 * Implementor: стихія. Визначає, який ефект вона дає цілі.
 * Не знає, яким умінням її застосовують.
 */
public interface Element {

    /** Назва стихії для журналу. */
    String name();

    /** Накладає ефект стихії на ціль. */
    void applyTo(Target target);
}
