/**
 * Раннер: три демонстраційні сценарії Завдання 1 (Observer).
 */
public class ObserverDemo {

    public static void main(String[] args) throws java.io.UnsupportedEncodingException {
        // щоб українські літери коректно виводилися в будь-якій консолі
        System.setOut(new java.io.PrintStream(System.out, true, "UTF-8"));

        Player hero = new Player("Hero");
        AchievementSystem achievements = new AchievementSystem();
        PlayerStatistics statistics = new PlayerStatistics();
        QuestSystem quests = new QuestSystem();
        DailyTaskSystem daily = new DailyTaskSystem();
        SeasonalEvent seasonal = new SeasonalEvent();

        scenario1(hero, achievements, statistics, quests, daily);
        scenario2(hero, seasonal);
        scenario3(hero, seasonal);
    }

    /** Підписати досягнення, статистику і квести; гравець вбиває вовка. */
    private static void scenario1(Player hero, AchievementSystem a, PlayerStatistics s,
                                  QuestSystem q, DailyTaskSystem d) {
        System.out.println("=== Сценарій 1: вбивство вовка ===");
        hero.subscribe(a);
        hero.subscribe(s);
        hero.subscribe(q);
        hero.subscribe(d);
        hero.killMonster("Вовк");
    }

    /** Підписати сезонну подію; гравець вбиває ще одного монстра. */
    private static void scenario2(Player hero, SeasonalEvent seasonal) {
        System.out.println("\n=== Сценарій 2: підключення сезонної події ===");
        hero.subscribe(seasonal);
        hero.killMonster("Кабан");
    }

    /** Сезонна подія завершилась і відписалась; гравець отримує рівень. */
    private static void scenario3(Player hero, SeasonalEvent seasonal) {
        System.out.println("\n=== Сценарій 3: сезонна подія завершилась ===");
        hero.unsubscribe(seasonal);
        hero.gainLevel();
        hero.completeQuest("Полювання на вовків");
    }
}
