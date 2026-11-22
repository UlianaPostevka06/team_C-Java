package Task3_Strategy;

import common.Log;

import java.util.Arrays;

/** Раннер завдання 3: розподіл здобичі в загоні (Strategy). Сценарії йдуть на одному загоні. */
public class StrategyDemo {

    public static void main(String[] args) {
        run();
    }

    public static void run() {
        PartyMember aldric = new PartyMember("Aldric", "WARRIOR", 40);
        PartyMember mira = new PartyMember("Mira", "MAGE", 30);
        PartyMember torin = new PartyMember("Torin", "ARCHER", 20);
        PartyMember lysa = new PartyMember("Lysa", "HEALER", 10);

        Log.section("Task 3 (Strategy): party setup");
        Party party = new Party(aldric, new RoundRobinRule());
        party.addMember(mira);
        party.addMember(torin);
        party.addMember(lysa);

        scenario1RoundRobin(party);
        scenario2ContributionByLeader(party, aldric, torin);
        scenario3NeedByClass(party, aldric);
    }

    /** Сценарій 1: п'ять предметів за чергою між чотирма гравцями. */
    static void scenario1RoundRobin(Party party) {
        Log.section("Task 3 (Strategy), scenario 1: round robin");
        party.distribute(Arrays.asList(
                new LootItem("Wolf Pelt", null), new LootItem("Iron Sword", null),
                new LootItem("Herb Bundle", null), new LootItem("Old Coin", null),
                new LootItem("Leather Boots", null)));
    }

    /** Сценарій 2: лідер змінює правило на «за внеском» (спроба не лідера відхиляється). */
    static void scenario2ContributionByLeader(Party party, PartyMember leader, PartyMember other) {
        Log.section("Task 3 (Strategy), scenario 2: contribution rule");
        party.changeRule(other, new ContributionRule());
        party.changeRule(leader, new ContributionRule());
        party.distribute(Arrays.asList(
                new LootItem("Gold Pouch", null), new LootItem("Steel Dagger", null),
                new LootItem("Healing Potion", null), new LootItem("Mana Crystal", null),
                new LootItem("Bronze Shield", null)));
    }

    /** Сценарій 3: нове правило «за потребою класу», предмети мають потрібний клас. */
    static void scenario3NeedByClass(Party party, PartyMember leader) {
        Log.section("Task 3 (Strategy), scenario 3: need by class");
        party.changeRule(leader, new NeedByClassRule());
        party.distribute(Arrays.asList(
                new LootItem("Plate Armor", "WARRIOR"), new LootItem("Arcane Orb", "MAGE"),
                new LootItem("Long Bow", "ARCHER"), new LootItem("Holy Relic", "HEALER"),
                new LootItem("Silver Ring", null)));
    }
}
