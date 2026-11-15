package Task2_ChainOfResponsibility;

/** Гравець, що пише в чат. Може бути заглушений модератором. */
public class Player {

    private final String name;
    private boolean muted;

    public Player(String name) {
        this.name = name;
    }

    public String getName() {
        return name;
    }

    public boolean isMuted() {
        return muted;
    }

    public void setMuted(boolean muted) {
        this.muted = muted;
    }
}
