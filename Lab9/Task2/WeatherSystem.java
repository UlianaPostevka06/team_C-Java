package task2_singleton;

import common.Log;

/** Підсистема погоди й освітлення: керує ігровим часом. */
public class WeatherSystem {

    private final WorldTime worldTime = WorldTime.getInstance();

    public void advanceTo(int hour, int minute) {
        Log.log("WeatherSystem", "Switching world to " + String.format("%02d:%02d", hour, minute));
        worldTime.setTime(hour, minute);
    }

    public WorldTime getWorldTime() {
        return worldTime;
    }
}
