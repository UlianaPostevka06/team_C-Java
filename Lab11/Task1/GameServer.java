package Task1_Facade;

import common.Log;

/**
 * Клієнт фасаду: сервер обробляє підключення гравців.
 * Працює лише з WorldEntryFacade, до підсистем напряму не звертається.
 */
public class GameServer {

    private final WorldEntryFacade facade;

    public GameServer(WorldEntryFacade facade) {
        this.facade = facade;
    }

    public void onPlayerConnected(Player player) {
        Log.log("GameServer", "New connection from '" + player.getNickname() + "'");
        facade.login(player);
    }

    public void onConnectionLost(Player player) {
        facade.disconnect(player);
    }

    public void onPlayerReconnected(Player player) {
        Log.log("GameServer", "Reconnection from '" + player.getNickname() + "'");
        facade.reconnect(player);
    }
}
