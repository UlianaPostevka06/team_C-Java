// Конкретні Observer'и. Класи без public — в одному файлі, у межах одного пакета.

/** Видає досягнення за першу перемогу, п'ятий рівень і перший квест. */
class AchievementSystem implements GameEventListener {
    private boolean firstKillGiven = false;
    private boolean firstQuestGiven = false;

    @Override
    public void onPlayerEvent(GameEvent e) {
        switch (e.getType()) {
            case MONSTER_KILLED:
                if (!firstKillGiven) {
                    firstKillGiven = true;
                    Player.log(getName(), "досягнення «Перша кров» для " + e.getPlayerName());
                } else {
                    Player.log(getName(), "нових досягнень за вбивство немає");
                }
                break;
            case LEVEL_UP:
                if (Integer.parseInt(e.getDetails()) >= 5) {
                    Player.log(getName(), "досягнення «Ветеран» для " + e.getPlayerName());
                } else {
                    Player.log(getName(), "рівень " + e.getDetails() + " — досягнення ще не відкрито");
                }
                break;
            case QUEST_COMPLETED:
                if (!firstQuestGiven) {
                    firstQuestGiven = true;
                    Player.log(getName(), "досягнення «Шукач пригод» для " + e.getPlayerName());
                }
                break;
        }
    }

    @Override
    public String getName() {
        return "AchievementSystem";
    }
}

/** Веде лічильники дій гравця. */
class PlayerStatistics implements GameEventListener {
    private int kills = 0;
    private int levels = 0;
    private int quests = 0;

    @Override
    public void onPlayerEvent(GameEvent e) {
        switch (e.getType()) {
            case MONSTER_KILLED: kills++; break;
            case LEVEL_UP: levels++; break;
            case QUEST_COMPLETED: quests++; break;
        }
        Player.log(getName(), "оновлено: вбивств=" + kills + ", рівнів=" + levels + ", квестів=" + quests);
    }

    @Override
    public String getName() {
        return "PlayerStatistics";
    }
}

/** Просуває активний квест «Полювання на вовків» і видає нагороду за завершення. */
class QuestSystem implements GameEventListener {
    private static final String QUEST = "Полювання на вовків";
    private int wolves = 0;

    @Override
    public void onPlayerEvent(GameEvent e) {
        switch (e.getType()) {
            case MONSTER_KILLED:
                if (e.getDetails().equals("Вовк")) {
                    wolves++;
                    Player.log(getName(), "квест «" + QUEST + "»: " + wolves + "/3");
                } else {
                    Player.log(getName(), "монстр «" + e.getDetails() + "» до квесту не належить");
                }
                break;
            case QUEST_COMPLETED:
                Player.log(getName(), "квест «" + e.getDetails() + "» закрито, нагороду видано");
                break;
            default:
                break; // рівень квестів не стосується
        }
    }

    @Override
    public String getName() {
        return "QuestSystem";
    }
}

/** Щоденне завдання «Вбити 5 монстрів». Реагує лише на вбивства. */
class DailyTaskSystem implements GameEventListener {
    private int killed = 0;

    @Override
    public void onPlayerEvent(GameEvent e) {
        if (e.getType() == GameEvent.Type.MONSTER_KILLED) {
            killed++;
            Player.log(getName(), "щоденне завдання «Вбити 5 монстрів»: " + killed + "/5");
        }
    }

    @Override
    public String getName() {
        return "DailyTaskSystem";
    }
}

/** Тимчасова сезонна подія: нараховує бонусні очки за будь-яку дію, поки підписана. */
class SeasonalEvent implements GameEventListener {
    private int points = 0;

    @Override
    public void onPlayerEvent(GameEvent e) {
        int bonus = (e.getType() == GameEvent.Type.MONSTER_KILLED) ? 10 : 5;
        points += bonus;
        Player.log(getName(), "+" + bonus + " сезонних очок за " + e.getType() + " (разом " + points + ")");
    }

    @Override
    public String getName() {
        return "SeasonalEvent";
    }
}
