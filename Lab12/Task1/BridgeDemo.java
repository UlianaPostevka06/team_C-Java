package Task1_Bridge;

import common.Log;

/** Раннер завдання 1: кожен сценарій у власному методі. */
public class BridgeDemo {

    public static void main(String[] args) {
        run();
    }

    public static void run() {
        scenario1FireAndIceStrikes();
        scenario2LightningBarrier();
        scenario3NewPoisonElement();
    }

    /** Сценарій 1: вогняний удар по одній цілі і крижаний удар по площі. */
    static void scenario1FireAndIceStrikes() {
        Log.section("Task 1 (Bridge), scenario 1: fire strike and ice area strike");
        Target goblin = new Target("Goblin", 100);
        Target wolf = new Target("Wolf", 100);
        Target bandit = new Target("Bandit", 100);

        Skill fireStrike = new SingleTargetStrike(new Fire());
        fireStrike.use("Aldric", goblin);

        Skill iceAreaStrike = new AreaStrike(new Ice());
        iceAreaStrike.use("Aldric", goblin, wolf, bandit);

        Log.log("Demo", "goblin effects: " + goblin.getEffects()
                + ", wolf effects: " + wolf.getEffects());
    }

    /** Сценарій 2: бар'єр блискавки. */
    static void scenario2LightningBarrier() {
        Log.section("Task 1 (Bridge), scenario 2: lightning barrier");
        Target aldric = new Target("Aldric", 100);
        Target bandit = new Target("Bandit", 100);

        ProtectiveBarrier barrier = new ProtectiveBarrier(new Lightning());
        barrier.use("Aldric", aldric);
        barrier.onAttackerHit(bandit);

        Log.log("Demo", "bandit effects: " + bandit.getEffects());
    }

    /**
     * Сценарій 3: нова стихія (клас Poison з Elements.java) без змін в уміннях.
     * Той самий AreaStrike, інша стихія.
     */
    static void scenario3NewPoisonElement() {
        Log.section("Task 1 (Bridge), scenario 3: new Poison element + area strike");
        Target goblin = new Target("Goblin", 100);
        Target wolf = new Target("Wolf", 100);
        Target bandit = new Target("Bandit", 100);

        Skill poisonAreaStrike = new AreaStrike(new Poison());
        poisonAreaStrike.use("Aldric", goblin, wolf, bandit);

        Log.log("Demo", "bandit effects: " + bandit.getEffects());
    }
}
