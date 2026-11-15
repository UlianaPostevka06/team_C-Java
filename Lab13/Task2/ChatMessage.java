package Task2_ChainOfResponsibility;

/** Повідомлення чату. Текст може змінюватися фільтрами (наприклад, замінюються лайливі слова). */
public class ChatMessage {

    private final Player sender;
    private final String channelName;
    private String text;

    public ChatMessage(Player sender, String channelName, String text) {
        this.sender = sender;
        this.channelName = channelName;
        this.text = text;
    }

    public Player getSender() {
        return sender;
    }

    public String getChannelName() {
        return channelName;
    }

    public String getText() {
        return text;
    }

    public void setText(String text) {
        this.text = text;
    }
}
