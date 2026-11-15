package Task2_ChainOfResponsibility;

import common.Log;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/*
 * Усі фільтри в одному файлі.
 * Це класи без public: вони видимі в межах пакета Task2_ChainOfResponsibility.
 * Кожен фільтр лишається окремим класом, а новий додається ще одним класом
 * без змін в існуючих фільтрах і в каналі.
 */

/** ConcreteHandler: блокує повідомлення заглушеного гравця. */
class MuteFilter extends ChatFilter {

    @Override
    protected String check(ChatMessage message) {
        if (message.getSender().isMuted()) {
            return "player " + message.getSender().getName() + " is muted";
        }
        return null;
    }
}

/** ConcreteHandler: блокує спам (те саме слово повторюється три й більше разів). */
class SpamFilter extends ChatFilter {

    private static final int MAX_REPEATS = 3;

    @Override
    protected String check(ChatMessage message) {
        Map<String, Integer> counts = new HashMap<>();
        for (String word : message.getText().toLowerCase().split("\\s+")) {
            String clean = word.replaceAll("[^\\p{L}\\p{N}]", "");
            if (clean.isEmpty()) {
                continue;
            }
            int count = counts.merge(clean, 1, Integer::sum);
            if (count >= MAX_REPEATS) {
                return "spam: word \"" + clean + "\" repeated " + count + " times";
            }
        }
        return null;
    }
}

/** ConcreteHandler: не блокує, а замінює нецензурні слова зірочками і передає далі. */
class ProfanityFilter extends ChatFilter {

    private static final List<String> BANNED = Arrays.asList("damn", "idiot");

    @Override
    protected String check(ChatMessage message) {
        String text = message.getText();
        String cleaned = text;
        for (String word : BANNED) {
            cleaned = cleaned.replaceAll("(?i)\\b" + word + "\\b", "*".repeat(word.length()));
        }
        if (!cleaned.equals(text)) {
            message.setText(cleaned);
            Log.log(name(), "profanity replaced with stars: \"" + cleaned + "\"");
        }
        return null;
    }
}

/** ConcreteHandler: блокує підозрілі посилання (дозволено лише домен гри). */
class LinkFilter extends ChatFilter {

    private static final String TRUSTED_DOMAIN = "seversky-lands.example";

    @Override
    protected String check(ChatMessage message) {
        String text = message.getText().toLowerCase();
        if (text.contains("http") && !text.contains(TRUSTED_DOMAIN)) {
            return "suspicious link (domain is not " + TRUSTED_DOMAIN + ")";
        }
        return null;
    }
}
