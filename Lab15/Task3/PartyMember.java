package Task3_Strategy;

import java.util.ArrayList;
import java.util.List;

/** Учасник загону: має клас, внесок у бій і список отриманих предметів. */
public class PartyMember {

    private final String name;
    private final String characterClass;
    private final int contribution;
    private final List<LootItem> loot = new ArrayList<>();

    public PartyMember(String name, String characterClass, int contribution) {
        this.name = name;
        this.characterClass = characterClass;
        this.contribution = contribution;
    }

    public String getName() {
        return name;
    }

    public String getCharacterClass() {
        return characterClass;
    }

    public int getContribution() {
        return contribution;
    }

    public int getLootCount() {
        return loot.size();
    }

    public void receive(LootItem item) {
        loot.add(item);
    }
}
