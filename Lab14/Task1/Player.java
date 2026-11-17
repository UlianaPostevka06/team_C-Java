import java.util.ArrayList;
import java.util.List;

/**
 * Subject: гравець. Не знає конкретних підписників — лише інтерфейс GameEventListener.
 */
public class Player {

    private static int clock = 0; // умовний час, секунди від 12:00:00

    private final String name;
    private int level = 1;
    private final List<GameEventListener> listeners = new ArrayList<>();

    public Player(String name) {
        this.name = name;
    }

    public String getName() {
        return name;
    }

    public int getLevel() {
        return level;
    }

    // ---- керування підписками (можна змінювати під час роботи програми) ----

    public void subscribe(GameEventListener listener) {
        if (!listeners.contains(listener)) {
            listeners.add(listener);
            log("Player:" + name, "підписано " + listener.getName());
        }
    }

    public void unsubscribe(GameEventListener listener) {
        if (listeners.remove(listener)) {
            log("Player:" + name, "відписано " + listener.getName());
        }
    }

    // ---- дії гравця ----

    public void killMonster(String monster) {
        log("Player:" + name, "вбиває монстра: " + monster);
        notifyListeners(new GameEvent(GameEvent.Type.MONSTER_KILLED, name, monster));
    }

    public void gainLevel() {
        level++;
        log("Player:" + name, "отримує рівень " + level);
        notifyListeners(new GameEvent(GameEvent.Type.LEVEL_UP, name, String.valueOf(level)));
    }

    public void completeQuest(String quest) {
        log("Player:" + name, "завершує квест: " + quest);
        notifyListeners(new GameEvent(GameEvent.Type.QUEST_COMPLETED, name, quest));
    }

    private void notifyListeners(GameEvent event) {
        log("Player:" + name, "сповіщає підписників (" + listeners.size() + ") про " + event.getType());
        // копія списку: підписник може відписатися під час обробки події
        for (GameEventListener l : new ArrayList<>(listeners)) {
            l.onPlayerEvent(event);
        }
    }

    // ---- журнал у форматі [час] [джерело] подія ----

    static void log(String source, String message) {
        int s = clock++;
        System.out.printf("[12:%02d:%02d] [%s] %s%n", s / 60, s % 60, source, message);
    }
}
