package Task3_Strategy;

import common.Log;

import java.util.List;

/**
 * ConcreteStrategy: за потребою класу. Предмет іде учаснику потрібного класу.
 * Якщо предмет універсальний або такого класу в загоні немає,
 * його отримує той, у кого найменше предметів.
 */
public class NeedByClassRule implements LootRule {

    @Override
    public String name() {
        return "NeedByClass";
    }

    @Override
    public PartyMember choose(LootItem item, List<PartyMember> members) {
        if (item.getPreferredClass() != null) {
            for (PartyMember m : members) {
                if (m.getCharacterClass().equals(item.getPreferredClass())) {
                    Log.log(name(), item.getName() + " is made for " + item.getPreferredClass()
                            + " -> " + m.getName());
                    return m;
                }
            }
        }
        PartyMember fewest = members.get(0);
        for (PartyMember m : members) {
            if (m.getLootCount() < fewest.getLootCount()) {
                fewest = m;
            }
        }
        Log.log(name(), item.getName() + " is for anyone -> " + fewest.getName()
                + " (fewest items: " + fewest.getLootCount() + ")");
        return fewest;
    }
}
