package Task1_Bridge;

/**
 * Abstraction: вид уміння (як воно застосовується).
 * Містить посилання на стихію (місток) і делегує їй накладання ефекту,
 * тому будь-яке уміння можна поєднати з будь-якою стихією під час виконання.
 */
public abstract class Skill {

    protected final Element element;

    protected Skill(Element element) {
        this.element = element;
    }

    /** Назва виду уміння для журналу (без стихії). */
    protected abstract String skillName();

    /** Застосовує уміння від імені персонажа до однієї або кількох цілей. */
    public abstract void use(String caster, Target... targets);

    /** Повна назва для журналу, наприклад "Fire Single-Target Strike". */
    protected String title() {
        return element.name() + " " + skillName();
    }
}
