package Task2_ChainOfResponsibility;

import common.Log;

import java.util.ArrayList;
import java.util.List;

/**
 * Канал чату: має власний склад і порядок фільтрів, що налаштовується під час виконання.
 * Перед кожним повідомленням ланцюжок збирається з поточного списку,
 * тому модератор може додавати і вимикати фільтри без перезапуску.
 */
public class ChatChannel {

    private final String name;
    private final List<ChatFilter> filters = new ArrayList<>();

    public ChatChannel(String name, ChatFilter... initialFilters) {
        this.name = name;
        for (ChatFilter filter : initialFilters) {
            filters.add(filter);
        }
    }

    public String getName() {
        return name;
    }

    /** Додає фільтр у кінець ланцюжка каналу. */
    public void addFilter(ChatFilter filter) {
        filters.add(filter);
        Log.log("Moderator", "enabled " + filter.name() + " in channel " + name);
    }

    /** Вимикає фільтр за назвою класу. */
    public void removeFilter(String filterName) {
        boolean removed = filters.removeIf(f -> f.name().equals(filterName));
        Log.log("Moderator", (removed ? "disabled " : "no such filter: ") + filterName
                + " in channel " + name);
    }

    /** Надсилає повідомлення: пропускає його через ланцюжок фільтрів каналу. */
    public void send(Player sender, String text) {
        Log.log(name, sender.getName() + " sends: \"" + text + "\"");
        ChatMessage message = new ChatMessage(sender, name, text);

        ChatFilter.FilterResult result;
        if (filters.isEmpty()) {
            result = ChatFilter.FilterResult.passed(message.getText());
        } else {
            linkChain();
            result = filters.get(0).handle(message);
        }

        if (result.isDelivered()) {
            Log.log(name, "DELIVERED " + sender.getName() + ": " + result.getFinalText());
        } else {
            Log.log(name, "NOT DELIVERED (blocked by " + result.getBlockedBy()
                    + ": " + result.getReason() + ")");
        }
    }

    /** З'єднує фільтри в ланцюжок у поточному порядку. */
    private void linkChain() {
        for (int i = 0; i < filters.size(); i++) {
            ChatFilter next = (i + 1 < filters.size()) ? filters.get(i + 1) : null;
            filters.get(i).setNext(next);
        }
    }
}
