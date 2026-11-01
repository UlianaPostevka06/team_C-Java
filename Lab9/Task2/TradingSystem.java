package task2_singleton;

import common.Log;

/** Підсистема торгівлі: крамниці працюють лише вдень. */
public class TradingSystem {

    private final WorldTime worldTime = WorldTime.getInstance();

    public void updateShops() {
        if (worldTime.isNight()) {
            Log.log("TradingSystem", "Night time -> shops are closed");
        } else {
            Log.log("TradingSystem", "Day time -> shops are open");
        }
    }

    public WorldTime getWorldTime() {
        return worldTime;
    }
}
