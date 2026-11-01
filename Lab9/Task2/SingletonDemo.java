package task2_singleton;

import common.Log;

/** Демонстрація завдання 2: єдиний ігровий час світу (Singleton). */
public class SingletonDemo {

    public static void main(String[] args) {
        run();
    }

    public static void run() {
        Log.section("Task 2: Singleton - world time");

        // 1-2. Підсистема погоди отримує ігровий час і встановлює 22:00
        WeatherSystem weather = new WeatherSystem();
        weather.advanceTo(22, 0);

        // 3-6. Підсистема спавну отримує той самий екземпляр, бачить ніч і спавнить нічних монстрів
        MonsterSpawner spawner = new MonsterSpawner();
        spawner.spawn();

        // Решта підсистем теж читають той самий час
        TradingSystem trading = new TradingSystem();
        trading.updateShops();
        QuestSystem quests = new QuestSystem();
        quests.updateQuests();

        // 7. Порівнюємо ідентифікатори об'єктів у всіх підсистемах
        int weatherId = weather.getWorldTime().identity();
        int spawnerId = spawner.getWorldTime().identity();
        int tradingId = trading.getWorldTime().identity();
        int questId = quests.getWorldTime().identity();
        Log.log("Demo", "WorldTime id (WeatherSystem)  : " + weatherId);
        Log.log("Demo", "WorldTime id (MonsterSpawner) : " + spawnerId);
        Log.log("Demo", "WorldTime id (TradingSystem)  : " + tradingId);
        Log.log("Demo", "WorldTime id (QuestSystem)    : " + questId);
        boolean same = weatherId == spawnerId && spawnerId == tradingId && tradingId == questId;
        Log.log("Demo", "All subsystems use the same instance: " + same);
    }
}
