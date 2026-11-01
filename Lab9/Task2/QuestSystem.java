package task2_singleton;

import common.Log;

/** Підсистема квестів: нічні квести доступні лише вночі. */
public class QuestSystem {

    private final WorldTime worldTime = WorldTime.getInstance();

    public void updateQuests() {
        if (worldTime.isNight()) {
            Log.log("QuestSystem", "Night time -> night quests available");
        } else {
            Log.log("QuestSystem", "Day time -> daily quests available");
        }
    }

    public WorldTime getWorldTime() {
        return worldTime;
    }
}
