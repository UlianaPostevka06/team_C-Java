package Task3_Strategy;

import java.util.List;

/**
 * Strategy: правило розподілу здобичі.
 * Загін знає лише цей інтерфейс, а кого обрати, вирішує конкретне правило.
 */
public interface LootRule {

    /** Назва правила для журналу. */
    String name();

    /** Обирає, кому дістанеться предмет. */
    PartyMember choose(LootItem item, List<PartyMember> members);
}
