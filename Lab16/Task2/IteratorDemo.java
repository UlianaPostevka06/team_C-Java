package Task2_Iterator;

import common.Log;

import java.util.Iterator;

/** Раннер завдання 2: обхід інвентарю (Iterator). Сценарії йдуть на одному інвентарі. */
public class IteratorDemo {

    public static void main(String[] args) {
        run();
    }

    public static void run() {
        Inventory inventory = buildInventory();

        scenario1AllItems(inventory);
        scenario2ForSale(inventory);
        scenario3QuestsAndMerchantAtOnce(inventory);
    }

    /** Складає інвентар із вкладеними контейнерами: сумка в сумці в сумці. */
    static Inventory buildInventory() {
        Container smallBox = new Container("Small Box")
                .add(new Item("Ruby", Item.Kind.FOR_SALE))
                .add(new Item("Royal Seal", Item.Kind.QUEST));
        Container pouch = new Container("Pouch")
                .add(new Item("Wolf Pelt", Item.Kind.FOR_SALE))
                .add(new Item("Silver Key", Item.Kind.QUEST))
                .add(smallBox);
        Container mainBag = new Container("Main Bag")
                .add(new Item("Iron Sword", Item.Kind.COMMON))
                .add(new Item("Healing Potion", Item.Kind.COMMON))
                .add(new Item("Ancient Letter", Item.Kind.QUEST))
                .add(pouch);
        Container chest = new Container("Chest")
                .add(new Item("Dragon Egg", Item.Kind.FOR_SALE))
                .add(new Item("Family Ring", Item.Kind.QUEST));
        Container bank = new Container("Bank Vault")
                .add(new Item("Gold Bar", Item.Kind.FOR_SALE))
                .add(chest);

        Inventory inventory = new Inventory(mainBag, bank);
        inventory.addExtraBag(new Container("Traveler Backpack")
                .add(new Item("Bread", Item.Kind.COMMON))
                .add(new Item("Old Map", Item.Kind.QUEST))
                .add(new Item("Wolf Fang", Item.Kind.FOR_SALE)));

        Log.section("Task 2 (Iterator): inventory is built");
        Log.log("Inventory", "Main Bag > Pouch > Small Box; Traveler Backpack; Bank Vault > Chest");
        return inventory;
    }

    /** Сценарій 1: інтерфейс виводить усі предмети, зокрема з вкладених контейнерів. */
    static void scenario1AllItems(Inventory inventory) {
        Log.section("Task 2 (Iterator), scenario 1: all items");
        int count = 0;
        for (Item item : inventory) {
            Log.log("Interface", "shows " + item);
            count++;
        }
        Log.log("Interface", "total items: " + count);
    }

    /** Сценарій 2: торговець бачить лише предмети для продажу. */
    static void scenario2ForSale(Inventory inventory) {
        Log.section("Task 2 (Iterator), scenario 2: items for sale");
        Iterator<Item> it = inventory.forSaleItems();
        while (it.hasNext()) {
            Log.log("Merchant", "can buy " + it.next().getName());
        }
    }

    /** Сценарій 3: квестові предмети й торговець одночасно, обходи йдуть по черзі й не заважають. */
    static void scenario3QuestsAndMerchantAtOnce(Inventory inventory) {
        Log.section("Task 2 (Iterator), scenario 3: two traversals at the same time");
        Iterator<Item> quests = inventory.questItems();
        Iterator<Item> merchant = inventory.forSaleItems();
        while (quests.hasNext() || merchant.hasNext()) {
            if (quests.hasNext()) {
                Log.log("QuestSystem", "quest item " + quests.next().getName());
            }
            if (merchant.hasNext()) {
                Log.log("Merchant", "sale item " + merchant.next().getName());
            }
        }
    }
}
