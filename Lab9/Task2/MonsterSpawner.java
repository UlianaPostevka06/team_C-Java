package task2_singleton;

import common.Log;

/** Підсистема спавну монстрів: правила залежать від часу доби. */
public class MonsterSpawner {

    private final WorldTime worldTime = WorldTime.getInstance();

    public void spawn() {
        if (worldTime.isNight()) {
            Log.log("MonsterSpawner", "It is night (" + worldTime.getTimeString() + ") -> Spawning night monsters");
        } else {
            Log.log("MonsterSpawner", "It is day (" + worldTime.getTimeString() + ") -> Spawning day monsters");
        }
    }

    public WorldTime getWorldTime() {
        return worldTime;
    }
}
