/**
 * Подія гравця. Незмінний об'єкт, який Subject (Player) передає всім підписникам.
 */
public class GameEvent {

    public enum Type {
        MONSTER_KILLED,
        LEVEL_UP,
        QUEST_COMPLETED
    }

    private final Type type;
    private final String playerName;
    /** Назва монстра, новий рівень або назва квесту — залежно від типу. */
    private final String details;

    public GameEvent(Type type, String playerName, String details) {
        this.type = type;
        this.playerName = playerName;
        this.details = details;
    }

    public Type getType() {
        return type;
    }

    public String getPlayerName() {
        return playerName;
    }

    public String getDetails() {
        return details;
    }

    @Override
    public String toString() {
        return type + " (" + details + ") від гравця " + playerName;
    }
}
