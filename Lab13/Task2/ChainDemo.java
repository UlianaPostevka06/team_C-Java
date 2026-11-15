package Task2_ChainOfResponsibility;

import common.Log;

/** Раннер завдання 2: фільтрація повідомлень чату. */
public class ChainDemo {

    public static void main(String[] args) {
        run();
    }

    public static void run() {
        scenario1GeneralChat();
        scenario2ClanChatWithoutProfanityFilter();
        scenario3MutedPlayer();
        scenario4ModeratorChangesFilters();
    }

    /** Загальний чат: усі чотири фільтри, порядок задано під час виконання. */
    private static ChatChannel newGeneralChat() {
        return new ChatChannel("General",
                new MuteFilter(), new SpamFilter(), new ProfanityFilter(), new LinkFilter());
    }

    /** Сценарій 1: 4 повідомлення в загальний чат. */
    static void scenario1GeneralChat() {
        Log.section("Task 2 (Chain of Responsibility), scenario 1: general chat, 4 messages");
        ChatChannel general = newGeneralChat();
        Player aldric = new Player("Aldric");

        general.send(aldric, "Hello everyone, who wants to run the dungeon?");
        general.send(aldric, "BUY gold buy now buy cheap");
        general.send(aldric, "that boss is a damn tough one");
        general.send(aldric, "free gold here http://fr33-g0ld.example/now");
    }

    /** Сценарій 2: повідомлення з нецензурним словом у клановий чат без фільтра лексики. */
    static void scenario2ClanChatWithoutProfanityFilter() {
        Log.section("Task 2 (Chain of Responsibility), scenario 2: clan chat without profanity filter");
        ChatChannel clan = new ChatChannel("Clan",
                new MuteFilter(), new SpamFilter(), new LinkFilter());
        Player aldric = new Player("Aldric");

        clan.send(aldric, "that boss is a damn tough one");
    }

    /** Сценарій 3: заглушений гравець, повідомлення блокує перший фільтр. */
    static void scenario3MutedPlayer() {
        Log.section("Task 2 (Chain of Responsibility), scenario 3: muted player");
        ChatChannel general = newGeneralChat();
        Player troll = new Player("Troll");
        troll.setMuted(true);

        general.send(troll, "hello, can anyone hear me?");
    }

    /** Сценарій 4: модератор вимикає фільтр посилань і додає його назад без змін у коді. */
    static void scenario4ModeratorChangesFilters() {
        Log.section("Task 2 (Chain of Responsibility), scenario 4: moderator changes filters at runtime");
        ChatChannel general = newGeneralChat();
        Player aldric = new Player("Aldric");

        general.removeFilter("LinkFilter");
        general.send(aldric, "free gold here http://fr33-g0ld.example/now");

        general.addFilter(new LinkFilter());
        general.send(aldric, "free gold here http://fr33-g0ld.example/now");
    }
}
