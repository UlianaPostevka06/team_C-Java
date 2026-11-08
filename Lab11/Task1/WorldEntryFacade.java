package Task1_Facade;

import common.Log;

/**
 * Facade: уся послідовність входу у світ описана в одному місці (enterWorld).
 * Звичайний вхід і перепідключення викликають ту саму послідовність,
 * тому порядок дій більше не дублюється і не може розійтися.
 */
public class WorldEntryFacade {

    private final AuthenticationService authentication;
    private final CharacterLoader characterLoader;
    private final InventoryLoader inventoryLoader;
    private final LocationServerSelector serverSelector;
    private final ChatService chatService;
    private final DailyBonusService dailyBonusService;

    public WorldEntryFacade(AuthenticationService authentication, CharacterLoader characterLoader,
                            InventoryLoader inventoryLoader, LocationServerSelector serverSelector,
                            ChatService chatService, DailyBonusService dailyBonusService) {
        this.authentication = authentication;
        this.characterLoader = characterLoader;
        this.inventoryLoader = inventoryLoader;
        this.serverSelector = serverSelector;
        this.chatService = chatService;
        this.dailyBonusService = dailyBonusService;
    }

    /** Звичайний вхід: одна проста операція. */
    public boolean login(Player player) {
        Log.log("WorldEntryFacade", "Login requested: '" + player.getNickname() + "'");
        return enterWorld(player);
    }

    /** Перепідключення після розриву: та сама послідовність, що й при вході. */
    public boolean reconnect(Player player) {
        Log.log("WorldEntryFacade", "Reconnect requested: '" + player.getNickname() + "'");
        return enterWorld(player);
    }

    /** Обробка розриву з'єднання: гравець виходить із чату. */
    public void disconnect(Player player) {
        Log.log("WorldEntryFacade", "Connection lost: '" + player.getNickname() + "'");
        chatService.leave(player);
    }

    /** Єдина послідовність кроків входу (порядок важливий). */
    private boolean enterWorld(Player player) {
        if (!authentication.authenticate(player)) {
            Log.log("WorldEntryFacade", "Authentication failed, entry cancelled");
            return false;
        }
        characterLoader.load(player);
        inventoryLoader.load(player);
        serverSelector.select(player);
        chatService.join(player);
        dailyBonusService.grant(player);
        Log.log("WorldEntryFacade", "'" + player.getNickname() + "' entered the world");
        return true;
    }
}
