package Task3_Strategy;

import common.Log;

import java.util.ArrayList;
import java.util.List;

/**
 * Context: загін. Розподіл здобичі делегує поточному правилу,
 * правило можна змінити під час виконання, але лише лідер.
 */
public class Party {

    private final List<PartyMember> members = new ArrayList<>();
    private final PartyMember leader;
    private LootRule rule;

    public Party(PartyMember leader, LootRule initialRule) {
        this.leader = leader;
        this.rule = initialRule;
        members.add(leader);
        Log.log("Party", "created, leader " + leader.getName() + ", rule " + rule.name());
    }

    public void addMember(PartyMember member) {
        members.add(member);
        Log.log("Party", member.getName() + " (" + member.getCharacterClass() + ") joined");
    }

    public List<PartyMember> getMembers() {
        return members;
    }

    /** Змінює правило, лише якщо запитує лідер. */
    public boolean changeRule(PartyMember requester, LootRule newRule) {
        if (requester != leader) {
            Log.log("Party", requester.getName() + " tries to change rule to " + newRule.name()
                    + ": denied, only leader " + leader.getName() + " can");
            return false;
        }
        Log.log("Party", leader.getName() + " changes rule " + rule.name() + " -> " + newRule.name());
        rule = newRule;
        return true;
    }

    /** Розподіляє предмети за поточним правилом. */
    public void distribute(List<LootItem> items) {
        for (LootItem item : items) {
            Log.log("Party", "loot dropped: " + item.getName() + " (active rule " + rule.name() + ")");
            PartyMember winner = rule.choose(item, members);
            winner.receive(item);
            Log.log("Party", winner.getName() + " receives " + item.getName()
                    + " (items: " + winner.getLootCount() + ")");
        }
    }
}
