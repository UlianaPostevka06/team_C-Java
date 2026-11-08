package Task1_Facade;

/** Гравець, який заходить у світ. Підсистеми входу змінюють його стан. */
public class Player {

    private final String nickname;
    private final int level;
    private String serverName;
    private int inventoryItems;

    public Player(String nickname, int level) {
        this.nickname = nickname;
        this.level = level;
    }

    public String getNickname() {
        return nickname;
    }

    public int getLevel() {
        return level;
    }

    public String getServerName() {
        return serverName;
    }

    public void setServerName(String serverName) {
        this.serverName = serverName;
    }

    public int getInventoryItems() {
        return inventoryItems;
    }

    public void setInventoryItems(int inventoryItems) {
        this.inventoryItems = inventoryItems;
    }
}
