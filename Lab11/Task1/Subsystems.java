package Task1_Facade;

import common.Log;

import java.util.HashSet;
import java.util.Set;

/*
 * Усі шість підсистем входу в один файл.
 * Це класи без public: вони видимі в межах пакета Task1_Facade, а в Java
 * дозволено кілька таких класів в одному файлі. Окремі класи лишаються окремими:
 * кожну підсистему можна використовувати самостійно.
 */

/** Підсистема 1: автентифікація гравця. */
class AuthenticationService {

    public boolean authenticate(Player player) {
        Log.log("AuthenticationService", "Player '" + player.getNickname() + "' authenticated");
        return true;
    }
}

/** Підсистема 2: завантаження персонажа. */
class CharacterLoader {

    public void load(Player player) {
        Log.log("CharacterLoader", "Character of '" + player.getNickname()
                + "' loaded (level " + player.getLevel() + ")");
    }
}

/** Підсистема 3: завантаження інвентарю. Доступна і окремо (наприклад, для перезавантаження). */
class InventoryLoader {

    public void load(Player player) {
        player.setInventoryItems(12);
        Log.log("InventoryLoader", "Inventory of '" + player.getNickname()
                + "' loaded (" + player.getInventoryItems() + " items)");
    }

    /** Окреме перезавантаження інвентарю без повторного входу. */
    public void reload(Player player) {
        Log.log("InventoryLoader", "Reloading inventory of '" + player.getNickname() + "'");
        load(player);
    }
}

/** Підсистема 4: вибір сервера локації залежно від рівня гравця. */
class LocationServerSelector {

    public String select(Player player) {
        String server = player.getLevel() < 15 ? "EU-Forest-1" : "EU-Mountains-2";
        player.setServerName(server);
        Log.log("LocationServerSelector", "Server '" + server + "' selected for '" + player.getNickname() + "'");
        return server;
    }
}

/** Підсистема 5: підключення до чату. */
class ChatService {

    private final Set<String> members = new HashSet<>();

    public void join(Player player) {
        if (members.add(player.getNickname())) {
            Log.log("ChatService", "'" + player.getNickname() + "' joined the chat (members: " + members.size() + ")");
        } else {
            Log.log("ChatService", "'" + player.getNickname() + "' is already in the chat");
        }
    }

    public void leave(Player player) {
        if (members.remove(player.getNickname())) {
            Log.log("ChatService", "'" + player.getNickname() + "' left the chat (members: " + members.size() + ")");
        }
    }
}

/** Підсистема 6: нарахування щоденного бонусу (раз на день на гравця). */
class DailyBonusService {

    private static final int BONUS_COINS = 100;
    private final Set<String> grantedToday = new HashSet<>();

    public void grant(Player player) {
        if (grantedToday.add(player.getNickname())) {
            Log.log("DailyBonusService", "Daily bonus granted to '" + player.getNickname()
                    + "': " + BONUS_COINS + " coins");
        } else {
            Log.log("DailyBonusService", "Daily bonus for '" + player.getNickname()
                    + "' already granted today, skipped");
        }
    }
}
