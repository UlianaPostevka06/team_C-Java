package Task2_ChainOfResponsibility;

import common.Log;

/**
 * Handler: фільтр чату в ланцюжку.
 * Кожен фільтр або блокує повідомлення з причиною, або передає його наступному
 * (за потреби змінивши текст). Сам фільтр не знає, які ще фільтри є в ланцюжку.
 */
public abstract class ChatFilter {

    private ChatFilter next;

    /** Назва фільтра для журналу. */
    public String name() {
        return getClass().getSimpleName();
    }

    public void setNext(ChatFilter next) {
        this.next = next;
    }

    /** Обробляє повідомлення: блокує або передає далі. */
    public final FilterResult handle(ChatMessage message) {
        Log.log(name(), "received message from " + message.getSender().getName()
                + ": \"" + message.getText() + "\"");

        String blockReason = check(message);
        if (blockReason != null) {
            Log.log(name(), "BLOCKED: " + blockReason);
            return FilterResult.blocked(name(), blockReason);
        }
        if (next == null) {
            Log.log(name(), "passed (last filter in the chain)");
            return FilterResult.passed(message.getText());
        }
        Log.log(name(), "passes the message to " + next.name());
        return next.handle(message);
    }

    /**
     * Перевіряє повідомлення.
     * Повертає причину блокування або null, якщо повідомлення можна передати далі.
     * Може змінити текст через message.setText(...).
     */
    protected abstract String check(ChatMessage message);

    /** Результат проходження ланцюжка. */
    public static final class FilterResult {

        private final boolean delivered;
        private final String blockedBy;
        private final String reason;
        private final String finalText;

        private FilterResult(boolean delivered, String blockedBy, String reason, String finalText) {
            this.delivered = delivered;
            this.blockedBy = blockedBy;
            this.reason = reason;
            this.finalText = finalText;
        }

        static FilterResult passed(String finalText) {
            return new FilterResult(true, null, null, finalText);
        }

        static FilterResult blocked(String filterName, String reason) {
            return new FilterResult(false, filterName, reason, null);
        }

        public boolean isDelivered() {
            return delivered;
        }

        public String getBlockedBy() {
            return blockedBy;
        }

        public String getReason() {
            return reason;
        }

        public String getFinalText() {
            return finalText;
        }
    }
}
