package task2_singleton;

import common.Log;

/** Singleton: єдиний ігровий час світу для всіх підсистем. */
public final class WorldTime {

    // Ледача потокобезпечна ініціалізація (Initialization-on-demand holder)
    private static final class Holder {
        private static final WorldTime INSTANCE = new WorldTime();
    }

    private int hour = 12;
    private int minute = 0;
    private String season = "Autumn";

    /** Приватний конструктор: створити ще один екземпляр ззовні неможливо. */
    private WorldTime() {
        Log.log("WorldTime", "World time initialized");
    }

    public static WorldTime getInstance() {
        return Holder.INSTANCE;
    }

    public synchronized void setTime(int hour, int minute) {
        this.hour = hour;
        this.minute = minute;
        Log.log("WorldTime", String.format("Time set to %02d:%02d (%s)", hour, minute,
                isNight() ? "night" : "day"));
    }

    public synchronized boolean isNight() {
        return hour >= 20 || hour < 6;
    }

    public synchronized String getTimeString() {
        return String.format("%02d:%02d", hour, minute);
    }

    public String getSeason() {
        return season;
    }

    /** Ідентифікатор об'єкта для перевірки, що це один і той самий екземпляр. */
    public int identity() {
        return System.identityHashCode(this);
    }
}
