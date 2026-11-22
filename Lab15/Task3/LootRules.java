package Task3_Strategy;

import common.Log;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

/*
 * Правила розподілу за чергою і за внеском в одному файлі.
 * Це класи без public: вони видимі в межах пакета Task3_Strategy.
 */

/** ConcreteStrategy: по черзі, кожен учасник отримує предмет за колом. */
class RoundRobinRule implements LootRule {

    private int next; // індекс наступного учасника

    @Override
    public String name() {
        return "RoundRobin";
    }

    @Override
    public PartyMember choose(LootItem item, List<PartyMember> members) {
        PartyMember chosen = members.get(next % members.size());
        next++;
        Log.log(name(), "turn goes to " + chosen.getName());
        return chosen;
    }
}

/**
 * ConcreteStrategy: за внеском. Більший внесок дає більше предметів:
 * обираємо учасника з найбільшим значенням внесок / (отримано + 1).
 */
class ContributionRule implements LootRule {

    private final Map<String, Integer> given = new HashMap<>();

    @Override
    public String name() {
        return "Contribution";
    }

    @Override
    public PartyMember choose(LootItem item, List<PartyMember> members) {
        PartyMember best = null;
        double bestScore = -1;
        for (PartyMember m : members) {
            double score = (double) m.getContribution() / (given.getOrDefault(m.getName(), 0) + 1);
            if (score > bestScore) { // при рівності лишається перший
                bestScore = score;
                best = m;
            }
        }
        given.merge(best.getName(), 1, Integer::sum);
        Log.log(name(), best.getName() + " has the best score " + String.format("%.1f", bestScore));
        return best;
    }
}
