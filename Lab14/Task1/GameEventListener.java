/**
 * Observer: інтерфейс підписника на дії гравця.
 */
public interface GameEventListener {

    /** Викликається гравцем при кожній його дії. Реакцію обирає сам підписник. */
    void onPlayerEvent(GameEvent event);

    /** Назва підписника для журналу. */
    String getName();
}
