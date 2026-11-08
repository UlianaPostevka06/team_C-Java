package Task1_Facade;

import common.Log;

/** Демонстрація завдання 1: вхід гравця у світ (Facade). */
public class FacadeDemo {

    public static void main(String[] args) {
        run();
    }

    public static void run() {
        Log.section("Task 1: Facade - player entering the world");

        // Підсистеми створюються окремо, тому залишаються доступними для самостійного використання
        InventoryLoader inventoryLoader = new InventoryLoader();
        WorldEntryFacade facade = new WorldEntryFacade(
                new AuthenticationService(), new CharacterLoader(), inventoryLoader,
                new LocationServerSelector(), new ChatService(), new DailyBonusService());
        GameServer server = new GameServer(facade);

        Player player = new Player("Aldric", 12);

        // Сценарій 1: вхід однією операцією, у журналі видно всі підсистеми по порядку
        Log.log("Demo", "Scenario 1: login with one operation");
        server.onPlayerConnected(player);

        // Сценарій 2: розрив з'єднання і перепідключення тією самою послідовністю
        Log.log("Demo", "Scenario 2: connection lost, then reconnect with one operation");
        server.onConnectionLost(player);
        server.onPlayerReconnected(player);

        // Сценарій 3: окреме використання підсистеми без повторного входу
        Log.log("Demo", "Scenario 3: reload inventory separately, without login");
        inventoryLoader.reload(player);
    }
}
